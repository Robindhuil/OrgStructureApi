using System.ComponentModel.DataAnnotations;

namespace OrgStructureApi.Models;

public class Division
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = null!;

    [Required, StringLength(20)]
    public string Code { get; set; } = null!;

    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public int? LeaderId { get; set; }
    public Employee? Leader { get; set; }

    public ICollection<Project>? Projects { get; set; }
}
