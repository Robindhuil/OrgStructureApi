public class CompanyReadDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public int? DirectorId { get; set; }
    public int DivisionsCount { get; set; }
    public int EmployeesCount { get; set; }
}