using System.ComponentModel.DataAnnotations;

namespace OrgStructureApi.Models;

public class Employee
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(20)]
    public string Title { get; set; } = null!;

    [Required, StringLength(50)]
    public string FirstName { get; set; } = null!;

    [Required, StringLength(50)]
    public string LastName { get; set; } = null!;

    [Required, Phone]
    public string Phone { get; set; } = null!;

    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    public int? CompanyId { get; set; }
    public Company? Company { get; set; }

    public ICollection<Division>? DivisionsLed { get; set; }
    public ICollection<Project>? ProjectsLed { get; set; }
    public ICollection<Department>? DepartmentsLed { get; set; }
}
