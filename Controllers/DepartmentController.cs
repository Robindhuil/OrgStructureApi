using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrgStructureApi.Data;
using OrgStructureApi.Models;
using OrgStructureApi.Dtos;

namespace OrgStructureApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentController : ControllerBase
{
    private readonly AppDbContext _context;

    public DepartmentController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentReadDto>>> GetDepartments()
    {
        var departments = await _context.Departments
            .Select(d => new DepartmentReadDto
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                ProjectId = d.ProjectId,
                LeaderId = d.LeaderId
            })
            .ToListAsync();

        return Ok(departments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DepartmentReadDto>> GetDepartment(int id)
    {
        var department = await _context.Departments
            .Where(d => d.Id == id)
            .Select(d => new DepartmentReadDto
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                ProjectId = d.ProjectId,
                LeaderId = d.LeaderId
            })
            .FirstOrDefaultAsync();

        if (department == null)
            return NotFound();

        return Ok(department);
    }

    [HttpPost]
    public async Task<ActionResult<Department>> CreateDepartment(DepartmentCreateDto dto)
    {
        var department = new Department
        {
            Name = dto.Name,
            Code = dto.Code,
            ProjectId = dto.ProjectId,
            LeaderId = dto.LeaderId
        };
        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        if (department.LeaderId.HasValue)
        {
            var leader = await _context.Employees.FindAsync(department.LeaderId.Value);
            if (leader == null)
                return BadRequest("Leader not found.");

            var project = await _context.Projects.FindAsync(department.ProjectId);
            if (project == null)
                return BadRequest("Project not found.");

            var division = await _context.Divisions.FindAsync(project.DivisionId);
            if (division == null)
                return BadRequest("Division not found.");

            if (leader.CompanyId != division.CompanyId)
                return BadRequest("Leader must be an employee of the company.");

            await _context.SaveChangesAsync();
        }

        return CreatedAtAction(nameof(GetDepartment), new { id = department.Id }, department);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDepartment(int id, DepartmentUpdateDto dto)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null)
            return NotFound();

        department.Name = dto.Name;
        department.Code = dto.Code;
        department.ProjectId = dto.ProjectId;
        department.LeaderId = dto.LeaderId;
        await _context.SaveChangesAsync();

        if (department.LeaderId.HasValue)
        {
            var leader = await _context.Employees.FindAsync(department.LeaderId.Value);
            if (leader == null)
                return BadRequest("Leader not found.");

            var project = await _context.Projects.FindAsync(department.ProjectId);
            if (project == null)
                return BadRequest("Project not found.");

            var division = await _context.Divisions.FindAsync(project.DivisionId);
            if (division == null)
                return BadRequest("Division not found.");

            if (leader.CompanyId != division.CompanyId)
                return BadRequest("Leader must be an employee of the company.");

            await _context.SaveChangesAsync();
        }

        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchDepartment(int id, DepartmentPatchDto dto)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null)
            return NotFound();

        if (dto.Name != null)
            department.Name = dto.Name;
        if (dto.Code != null)
            department.Code = dto.Code;
        if (dto.ProjectId.HasValue)
            department.ProjectId = dto.ProjectId.Value;
        if (dto.LeaderId.HasValue)
            department.LeaderId = dto.LeaderId;

        await _context.SaveChangesAsync();

        if (department.LeaderId.HasValue)
        {
            var leader = await _context.Employees.FindAsync(department.LeaderId.Value);
            if (leader == null)
                return BadRequest("Leader not found.");

            var project = await _context.Projects.FindAsync(department.ProjectId);
            if (project == null)
                return BadRequest("Project not found.");

            var division = await _context.Divisions.FindAsync(project.DivisionId);
            if (division == null)
                return BadRequest("Division not found.");

            if (leader.CompanyId != division.CompanyId)
                return BadRequest("Leader must be an employee of the company.");

            await _context.SaveChangesAsync();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null)
            return NotFound();

        if (department.LeaderId.HasValue)
        {
            var leader = await _context.Employees.FindAsync(department.LeaderId.Value);
            if (leader != null)
            {
                // leave leader reference intact; business rule allows same employee to hold multiple roles
            }
        }

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
