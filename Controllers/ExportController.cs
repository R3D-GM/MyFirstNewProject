using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstNewProject.Data;
using MyFirstNewProject.Models;
using MyFirstNewProject.Services;
using System.Text;

namespace MyFirstNewProject.Controllers;

[Authorize]
public class ExportController : Controller
{
    private readonly ConsigneeService _consigneeService;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ExportController> _logger;

    public ExportController(
        ConsigneeService consigneeService,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger<ExportController> logger)
    {
        _consigneeService = consigneeService;
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    // ============================================
    // EXPORT CUSTOMERS TO EXCEL
    // ============================================
    public async Task<IActionResult> CustomersToExcel()
    {
        try
        {
            var customers = await _consigneeService.GetConsigneesAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Customers");

                // Headers
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Code";
                worksheet.Cell(1, 3).Value = "First Name";
                worksheet.Cell(1, 4).Value = "Second Name";
                worksheet.Cell(1, 5).Value = "Third Name";
                worksheet.Cell(1, 6).Value = "Type";
                worksheet.Cell(1, 7).Value = "Business Type";
                worksheet.Cell(1, 8).Value = "Status";
                worksheet.Cell(1, 9).Value = "TIN";

                // Header styling
                var headerRow = worksheet.Row(1);
                headerRow.Style.Font.Bold = true;
                headerRow.Style.Fill.BackgroundColor = XLColor.LightBlue;
                headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Data
                for (int i = 0; i < customers.Count; i++)
                {
                    var row = i + 2;
                    var customer = customers[i];

                    worksheet.Cell(row, 1).Value = customer.Id;
                    worksheet.Cell(row, 2).Value = customer.Code ?? "";
                    worksheet.Cell(row, 3).Value = customer.FirstName ?? "";
                    worksheet.Cell(row, 4).Value = customer.SecondName ?? "";
                    worksheet.Cell(row, 5).Value = customer.ThirdName ?? "";
                    worksheet.Cell(row, 6).Value = customer.IsPerson ? "Person" : "Company";
                    worksheet.Cell(row, 7).Value = customer.BusinessType ?? "N/A";
                    worksheet.Cell(row, 8).Value = customer.IsActive ? "Active" : "Inactive";
                    worksheet.Cell(row, 9).Value = customer.Tin ?? "N/A";
                }

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                // Add summary row
                var lastRow = customers.Count + 2;
                worksheet.Cell(lastRow, 1).Value = "TOTAL CUSTOMERS:";
                worksheet.Cell(lastRow, 2).Value = customers.Count;
                worksheet.Cell(lastRow, 1).Style.Font.Bold = true;
                worksheet.Cell(lastRow, 2).Style.Font.Bold = true;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"Customers_{DateTime.Now:yyyy-MM-dd}.xlsx"
                    );
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting customers to Excel");
            TempData["Error"] = "Failed to export customers.";
            return RedirectToAction("Index", "Report");
        }
    }

    // ============================================
    // EXPORT CUSTOMERS TO PDF
    // ============================================
    public async Task<IActionResult> CustomersToPdf()
    {
        try
        {
            var customers = await _consigneeService.GetConsigneesAsync();

            using (var stream = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 20, 20, 20, 20);
                PdfWriter.GetInstance(document, stream);

                document.Open();

                // Title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.BLACK);
                var title = new Paragraph("Customer Summary Report", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);

                document.Add(new Paragraph($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}"));
                document.Add(new Paragraph($"Total Customers: {customers.Count}"));
                document.Add(new Paragraph(" "));

                // Summary stats
                var activeCount = customers.Count(c => c.IsActive);
                var inactiveCount = customers.Count(c => !c.IsActive);
                var companiesCount = customers.Count(c => !c.IsPerson);
                var personsCount = customers.Count(c => c.IsPerson);

                document.Add(new Paragraph($"Active: {activeCount} ({Math.Round((double)activeCount / customers.Count * 100)}%)"));
                document.Add(new Paragraph($"Inactive: {inactiveCount} ({Math.Round((double)inactiveCount / customers.Count * 100)}%)"));
                document.Add(new Paragraph($"Companies: {companiesCount} ({Math.Round((double)companiesCount / customers.Count * 100)}%)"));
                document.Add(new Paragraph($"Persons: {personsCount} ({Math.Round((double)personsCount / customers.Count * 100)}%)"));
                document.Add(new Paragraph(" "));

                // Table
                var table = new PdfPTable(6);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 1, 2, 3, 2, 2, 2 });

                // Headers
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
                var headerBg = BaseColor.GRAY;

                string[] headers = { "ID", "Code", "Name", "Type", "Business", "Status" };
                foreach (var h in headers)
                {
                    var cell = new PdfPCell(new Phrase(h, headerFont))
                    {
                        BackgroundColor = headerBg,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 5
                    };
                    table.AddCell(cell);
                }

                // Data
                var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.BLACK);
                foreach (var customer in customers.Take(50))
                {
                    table.AddCell(new PdfPCell(new Phrase(customer.Id.ToString(), dataFont)));
                    table.AddCell(new PdfPCell(new Phrase(customer.Code ?? "", dataFont)));
                    table.AddCell(new PdfPCell(new Phrase($"{customer.FirstName} {customer.SecondName}", dataFont)));
                    table.AddCell(new PdfPCell(new Phrase(customer.IsPerson ? "Person" : "Company", dataFont)));
                    table.AddCell(new PdfPCell(new Phrase(customer.BusinessType ?? "N/A", dataFont)));
                    table.AddCell(new PdfPCell(new Phrase(customer.IsActive ? "Active" : "Inactive", dataFont)));
                }

                document.Add(table);

                document.Add(new Paragraph(" "));
                document.Add(new Paragraph($"* Showing first 50 customers. Total: {customers.Count} customers."));

                document.Close();

                return File(
                    stream.ToArray(),
                    "application/pdf",
                    $"Customers_{DateTime.Now:yyyy-MM-dd}.pdf"
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting customers to PDF");
            TempData["Error"] = "Failed to export PDF.";
            return RedirectToAction("Index", "Report");
        }
    }

    // ============================================
    // PRINT CUSTOMER LIST
    // ============================================
    public async Task<IActionResult> PrintCustomers()
    {
        var customers = await _consigneeService.GetConsigneesAsync();
        return View(customers);
    }
}