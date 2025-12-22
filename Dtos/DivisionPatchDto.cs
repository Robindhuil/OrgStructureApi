namespace OrgStructureApi.Dtos;

public class DivisionPatchDto
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public int? CompanyId { get; set; }
    public int? LeaderId { get; set; }
}