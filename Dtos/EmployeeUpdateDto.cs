using System.ComponentModel.DataAnnotations;

public class EmployeeUpdateDto
{
    [Required]
    public string Title { get; set; } = null!;

    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    [Required]
    public string Phone { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
}
