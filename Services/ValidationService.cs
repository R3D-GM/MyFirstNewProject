using MyFirstNewProject.Models;

namespace MyFirstNewProject.Services;

public class ValidationService
{
    private readonly ILogger<ValidationService> _logger;

    public ValidationService(ILogger<ValidationService> logger)
    {
        _logger = logger;
    }

    public List<string> ValidateConsignee(Consignee consignee)
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(consignee.FirstName))
        {
            errors.Add("First Name is required.");
        }

        if (string.IsNullOrEmpty(consignee.Code))
        {
            errors.Add("Code is required.");
        }

        if (consignee.FirstName?.Length > 100)
        {
            errors.Add("First Name cannot exceed 100 characters.");
        }

        if (consignee.Code?.Length > 50)
        {
            errors.Add("Code cannot exceed 50 characters.");
        }

        if (consignee.BusinessType?.Length > 100)
        {
            errors.Add("Business Type cannot exceed 100 characters.");
        }

        return errors;
    }

    public bool IsValidEmail(string? email)
    {
        if (string.IsNullOrEmpty(email)) return false;
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}