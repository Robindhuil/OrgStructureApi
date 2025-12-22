using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrgStructureApi.Data;
using OrgStructureApi.Models;
using OrgStructureApi.Dtos;

namespace OrgStructureApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DivisionController : ControllerBase
{
    private readonly AppDbContext _context;

    public DivisionController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DivisionReadDto>>> GetDivisions()
    {
        var divisions = await _context.Divisions
            .Select(d => new DivisionReadDto
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                CompanyId = d.CompanyId,
                LeaderId = d.LeaderId,
                ProjectsCount = d.Projects != null ? d.Projects.Count : 0
            })
            .ToListAsync();

        return Ok(divisions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DivisionReadDto>> GetDivision(int id)
    {
        var division = await _context.Divisions
            .Where(d => d.Id == id)
            .Select(d => new DivisionReadDto
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                CompanyId = d.CompanyId,
                LeaderId = d.LeaderId,
                ProjectsCount = d.Projects != null ? d.Projects.Count : 0
            })
            .FirstOrDefaultAsync();

        if (division == null)
            return NotFound();

        return Ok(division);
    }

    [HttpPost]
    public async Task<ActionResult<Division>> CreateDivision(DivisionCreateDto dto)
    {
        var division = new Division
        {
            Name = dto.Name,
            Code = dto.Code,
            CompanyId = dto.CompanyId,
            LeaderId = dto.LeaderId
        };
        _context.Divisions.Add(division);
        await _context.SaveChangesAsync();

        if (division.LeaderId.HasValue)
        {
            var leader = await _context.Employees.FindAsync(division.LeaderId.Value);
            if (leader == null)
                return BadRequest("Leader not found.");
            if (leader.CompanyId != division.CompanyId)
            {
                leader.CompanyId = division.CompanyId;
            }
            // Check if leader is already leading another division
            var otherDivision = await _context.Divisions.FirstOrDefaultAsync(d => d.Id != division.Id && d.LeaderId == leader.Id);
            if (otherDivision != null)
            {
                otherDivision.LeaderId = null;
            }
            // Check if leader is director of a company
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.DirectorId == leader.Id);
            if (company != null)
            {
                company.DirectorId = null;
            }
            // Check if leader is leading a project
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.LeaderId == leader.Id);
            if (project != null)
            {
                project.LeaderId = null;
            }
            // Check if leader is leading a department
            var department = await _context.Departments.FirstOrDefaultAsync(dep => dep.LeaderId == leader.Id);
            if (department != null)
            {
                department.LeaderId = null;
            }
            leader.Role = EmployeeRole.DivisionLeader;
            await _context.SaveChangesAsync();
        }
        return CreatedAtAction(nameof(GetDivision), new { id = division.Id }, division);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDivision(int id, DivisionUpdateDto dto)
    {
        var division = await _context.Divisions.FindAsync(id);
        if (division == null)
            return NotFound();

        if (division.LeaderId.HasValue && division.LeaderId != dto.LeaderId)
        {
            var oldLeader = await _context.Employees.FindAsync(division.LeaderId.Value);
            if (oldLeader != null)
            {
                oldLeader.Role = EmployeeRole.RegularEmployee;
            }
        }

        division.Name = dto.Name;
        division.Code = dto.Code;
        division.CompanyId = dto.CompanyId;
        division.LeaderId = dto.LeaderId;
        await _context.SaveChangesAsync();

        if (division.LeaderId.HasValue)
        {
            var leader = await _context.Employees.FindAsync(division.LeaderId.Value);
            if (leader == null)
                return BadRequest("Leader not found.");
            if (leader.CompanyId != division.CompanyId)
                return BadRequest("Leader must be an employee of the company.");
            // Check if leader is already leading another division
            var otherDivision = await _context.Divisions.FirstOrDefaultAsync(d => d.Id != division.Id && d.LeaderId == leader.Id);
            if (otherDivision != null)
            {
                otherDivision.LeaderId = null;
            }
            // Check if leader is director of a company
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.DirectorId == leader.Id);
            if (company != null)
            {
                company.DirectorId = null;
            }
            // Check if leader is leading a project
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.LeaderId == leader.Id);
            if (project != null)
            {
                project.LeaderId = null;
            }
            // Check if leader is leading a department
            var department = await _context.Departments.FirstOrDefaultAsync(dep => dep.LeaderId == leader.Id);
            if (department != null)
            {
                department.LeaderId = null;
            }
            leader.Role = EmployeeRole.DivisionLeader;
            await _context.SaveChangesAsync();
        }
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchDivision(int id, DivisionPatchDto dto)
    {
        var division = await _context.Divisions.FindAsync(id);
        if (division == null)
            return NotFound();

        if (dto.LeaderId.HasValue && division.LeaderId.HasValue && division.LeaderId != dto.LeaderId)
        {
            var oldLeader = await _context.Employees.FindAsync(division.LeaderId.Value);
            if (oldLeader != null)
            {
                oldLeader.Role = EmployeeRole.RegularEmployee;
            }
        }

        if (dto.Name != null)
            division.Name = dto.Name;
        if (dto.Code != null)
            division.Code = dto.Code;
        if (dto.CompanyId.HasValue)
            division.CompanyId = dto.CompanyId.Value;
        if (dto.LeaderId.HasValue)
            division.LeaderId = dto.LeaderId;
        await _context.SaveChangesAsync();

        if (division.LeaderId.HasValue)
        {
            var leader = await _context.Employees.FindAsync(division.LeaderId.Value);
            if (leader == null)
                return BadRequest("Leader not found.");
            if (leader.CompanyId != division.CompanyId)
                return BadRequest("Leader must be an employee of the company.");
            // Check if leader is already leading another division
            var otherDivision = await _context.Divisions.FirstOrDefaultAsync(d => d.Id != division.Id && d.LeaderId == leader.Id);
            if (otherDivision != null)
            {
                otherDivision.LeaderId = null;
            }
            // Check if leader is director of a company
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.DirectorId == leader.Id);
            if (company != null)
            {
                company.DirectorId = null;
            }
            // Check if leader is leading a project
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.LeaderId == leader.Id);
            if (project != null)
            {
                project.LeaderId = null;
            }
            // Check if leader is leading a department
            var department = await _context.Departments.FirstOrDefaultAsync(dep => dep.LeaderId == leader.Id);
            if (department != null)
            {
                department.LeaderId = null;
            }
            leader.Role = EmployeeRole.DivisionLeader;
            await _context.SaveChangesAsync();
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDivision(int id)
    {
        var division = await _context.Divisions.FindAsync(id);
        if (division == null)
            return NotFound();

        if (division.LeaderId.HasValue)
        {
            var leader = await _context.Employees.FindAsync(division.LeaderId.Value);
            if (leader != null)
            {
                leader.Role = EmployeeRole.RegularEmployee;
            }
        }

        _context.Divisions.Remove(division);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}