using System.ComponentModel.DataAnnotations;

namespace OrgStructureApi.Models;

public class Department
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = null!;

    [Required, StringLength(20)]
    public string Code { get; set; } = null!;

    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public int? LeaderId { get; set; }
    public Employee? Leader { get; set; }
}
