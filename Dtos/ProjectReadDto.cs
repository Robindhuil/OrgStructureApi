namespace OrgStructureApi.Dtos;

public class ProjectReadDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public int DivisionId { get; set; }
    public int? LeaderId { get; set; }
    public int DepartmentsCount { get; set; }
}
