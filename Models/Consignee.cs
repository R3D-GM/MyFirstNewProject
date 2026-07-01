using System.ComponentModel.DataAnnotations;

namespace MyFirstNewProject.Models;

public class Consignee
{
    public int Id { get; set; }

    [Display(Name = "Code")]
    [StringLength(50, ErrorMessage = "Code cannot exceed 50 characters.")]
    public string? Code { get; set; }

    [Display(Name = "First Name")]
    [Required(ErrorMessage = "First Name is required.")]
    [StringLength(100, ErrorMessage = "First Name cannot exceed 100 characters.")]
    public string? FirstName { get; set; }

    [Display(Name = "Second Name")]
    [StringLength(100, ErrorMessage = "Second Name cannot exceed 100 characters.")]
    public string? SecondName { get; set; }

    [Display(Name = "Third Name")]
    [StringLength(100, ErrorMessage = "Third Name cannot exceed 100 characters.")]
    public string? ThirdName { get; set; }

    [Display(Name = "Type")]
    public bool IsPerson { get; set; }

    [Display(Name = "Status")]
    public bool IsActive { get; set; }

    [Display(Name = "Business Type")]
    [StringLength(100, ErrorMessage = "Business Type cannot exceed 100 characters.")]
    public string? BusinessType { get; set; }

    [Display(Name = "TIN")]
    [StringLength(50, ErrorMessage = "TIN cannot exceed 50 characters.")]
    public string? Tin { get; set; }

    [Display(Name = "Gender")]
    [StringLength(20, ErrorMessage = "Gender cannot exceed 20 characters.")]
    public string? Gender { get; set; }

    [Display(Name = "Created On")]
    public DateTime CreatedOn { get; set; }

    [Display(Name = "Last Modified")]
    public DateTime LastModified { get; set; }
}