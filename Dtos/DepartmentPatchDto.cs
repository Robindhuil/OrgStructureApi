namespace OrgStructureApi.Dtos;

public class DepartmentPatchDto
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public int? ProjectId { get; set; }
    public int? LeaderId { get; set; }
}
