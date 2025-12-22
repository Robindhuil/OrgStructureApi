using System.ComponentModel.DataAnnotations;

namespace OrgStructureApi.Dtos;

public class DepartmentUpdateDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(20)]
    public string Code { get; set; } = null!;

    [Required]
    public int ProjectId { get; set; }

    public int? LeaderId { get; set; }
}
