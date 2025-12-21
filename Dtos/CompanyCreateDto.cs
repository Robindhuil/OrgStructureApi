using System.ComponentModel.DataAnnotations;

namespace OrgStructureApi.Dtos;

public class CompanyCreateDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(20)]
    public string Code { get; set; } = null!;

    public int? DirectorId { get; set; }
}
