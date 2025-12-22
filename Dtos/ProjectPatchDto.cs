namespace OrgStructureApi.Dtos;

public class ProjectPatchDto
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public int? DivisionId { get; set; }
    public int? LeaderId { get; set; }
}
