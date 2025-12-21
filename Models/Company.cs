using System.ComponentModel.DataAnnotations;

namespace OrgStructureApi.Models;

public class Company
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = null!;

    [Required, StringLength(20)]
    public string Code { get; set; } = null!;

    public int? DirectorId { get; set; }
    public Employee? Director { get; set; }

    public ICollection<Division>? Divisions { get; set; }
    public ICollection<Employee>? Employees { get; set; }
}
