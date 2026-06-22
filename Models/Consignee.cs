namespace MyFirstNewProject.Models;

public class Consignee
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? FirstName { get; set; }
    public string? SecondName { get; set; }
    public string? ThirdName { get; set; }
    public bool IsPerson { get; set; }
    public bool IsActive { get; set; }

    // IMPORTANT: must be nullable
    public DateTime? StartDate { get; set; }
}