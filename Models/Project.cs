using System.ComponentModel.DataAnnotations;

namespace OrgStructureApi.Models;

public class Project
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = null!;

    [Required, StringLength(20)]
    public string Code { get; set; } = null!;

    public int DivisionId { get; set; }
    public Division Division { get; set; } = null!;

    public int? LeaderId { get; set; }
    public Employee? Leader { get; set; }

    public ICollection<Department>? Departments { get; set; }
}
