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
    public string? BusinessType { get; set; }
    public string? Tin { get; set; }
    public string? Gender { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime LastModified { get; set; }
}