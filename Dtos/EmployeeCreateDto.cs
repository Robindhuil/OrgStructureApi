using System.ComponentModel.DataAnnotations;

public class EmployeeCreateDto
{
    [Required]
    [StringLength(20)]
    public string Title { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = null!;

    [Required]
    [Phone]
    public string Phone { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public int CompanyId { get; set; }
}
