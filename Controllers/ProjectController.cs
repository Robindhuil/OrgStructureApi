using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrgStructureApi.Data;
using OrgStructureApi.Models;
using OrgStructureApi.Dtos;

namespace OrgStructureApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProjectController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectReadDto>>> GetProjects()
    {
        var projects = await _context.Projects
            .Select(p => new ProjectReadDto
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code,
                DivisionId = p.DivisionId,
                LeaderId = p.LeaderId,
                DepartmentsCount = p.Departments != null ? p.Departments.Count : 0
            })
            .ToListAsync();

        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectReadDto>> GetProject(int id)
    {
        var project = await _context.Projects
            .Where(p => p.Id == id)
            .Select(p => new ProjectReadDto
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code,
                DivisionId = p.DivisionId,
                LeaderId = p.LeaderId,
                DepartmentsCount = p.Departments != null ? p.Departments.Count : 0
            })
            .FirstOrDefaultAsync();

        if (project == null)
            return NotFound();

        return Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<Project>> CreateProject(ProjectCreateDto dto)
    {
        var project = new Project
        {
            Name = dto.Name,
            Code = dto.Code,
            DivisionId = dto.DivisionId,
            LeaderId = dto.LeaderId
        };
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        if (project.LeaderId.HasValue)
        {
            var leader = await _context.Employees.FindAsync(project.LeaderId.Value);
            if (leader == null)
                return BadRequest("Leader not found.");

            var division = await _context.Divisions.FindAsync(project.DivisionId);
            if (division == null)
                return BadRequest("Division not found.");

            if (leader.CompanyId != division.CompanyId)
                return BadRequest("Leader must be an employee of the company.");

            await _context.SaveChangesAsync();
        }

        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, ProjectUpdateDto dto)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
            return NotFound();

        // determine resulting values without persisting
        var targetDivisionId = dto.DivisionId;
        var resultingLeaderId = dto.LeaderId;

        if (targetDivisionId == 0)
            targetDivisionId = project.DivisionId;

        if (!dto.LeaderId.HasValue)
            resultingLeaderId = project.LeaderId;

        if (resultingLeaderId.HasValue)
        {
            var leader = await _context.Employees.FindAsync(resultingLeaderId.Value);
            if (leader == null)
                return BadRequest("Leader not found.");

            var division = await _context.Divisions.FindAsync(targetDivisionId);
            if (division == null)
                return BadRequest("Division not found.");

            if (leader.CompanyId != division.CompanyId)
                return BadRequest("Leader must be an employee of the company.");
        }

        project.Name = dto.Name;
        project.Code = dto.Code;
        project.DivisionId = dto.DivisionId;
        project.LeaderId = dto.LeaderId;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchProject(int id, ProjectPatchDto dto)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
            return NotFound();

        // determine resulting values without persisting
        var targetDivisionId = dto.DivisionId.HasValue ? dto.DivisionId.Value : project.DivisionId;
        var resultingLeaderId = dto.LeaderId.HasValue ? dto.LeaderId : project.LeaderId;

        if (resultingLeaderId.HasValue)
        {
            var leader = await _context.Employees.FindAsync(resultingLeaderId.Value);
            if (leader == null)
                return BadRequest("Leader not found.");

            var division = await _context.Divisions.FindAsync(targetDivisionId);
            if (division == null)
                return BadRequest("Division not found.");

            if (leader.CompanyId != division.CompanyId)
                return BadRequest("Leader must be an employee of the company.");
        }

        if (dto.Name != null)
            project.Name = dto.Name;
        if (dto.Code != null)
            project.Code = dto.Code;
        if (dto.DivisionId.HasValue)
            project.DivisionId = dto.DivisionId.Value;
        if (dto.LeaderId.HasValue)
            project.LeaderId = dto.LeaderId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
            return NotFound();

        if (project.LeaderId.HasValue)
        {
            var leader = await _context.Employees.FindAsync(project.LeaderId.Value);
            if (leader != null)
            {
                // leave leader reference intact; business rule allows same employee to hold multiple roles
            }
        }

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
