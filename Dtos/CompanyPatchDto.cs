namespace OrgStructureApi.Dtos;

public class CompanyPatchDto
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public int? DirectorId { get; set; }
}
