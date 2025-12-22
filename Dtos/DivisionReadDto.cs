namespace OrgStructureApi.Dtos;

public class DivisionReadDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public int CompanyId { get; set; }
    public int? LeaderId { get; set; }
    public int ProjectsCount { get; set; }
}