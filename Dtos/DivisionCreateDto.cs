using System.ComponentModel.DataAnnotations;

namespace OrgStructureApi.Dtos;

public class DivisionCreateDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(20)]
    public string Code { get; set; } = null!;

    [Required]
    public int CompanyId { get; set; }

    public int? LeaderId { get; set; }
}