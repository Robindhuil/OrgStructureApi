using System.ComponentModel.DataAnnotations;

public class EmployeePatchDto
{

    [StringLength(20)]
    public string? Title { get; set; }
    [StringLength(50)]
    public string? FirstName { get; set; }
    [StringLength(50)]
    public string? LastName { get; set; }
    [Phone]
    public string? Phone { get; set; }
    [EmailAddress]
    public string? Email { get; set; }

    public int? CompanyId { get; set; }
    public OrgStructureApi.Models.EmployeeRole? Role { get; set; } = OrgStructureApi.Models.EmployeeRole.RegularEmployee;
}
