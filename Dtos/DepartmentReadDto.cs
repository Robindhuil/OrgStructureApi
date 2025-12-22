namespace OrgStructureApi.Dtos;

public class DepartmentReadDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public int ProjectId { get; set; }
    public int? LeaderId { get; set; }
}
