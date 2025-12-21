using System.ComponentModel.DataAnnotations;

public class EmployeeCreateDto
{
    [Required]
    public string Title { get; set; } = null!;

    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    [Required]
    [Phone]
    public string Phone { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
}
