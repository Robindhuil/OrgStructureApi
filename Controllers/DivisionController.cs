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

        // Compute resulting values without persisting yet
        var resultingCompanyId = dto.CompanyId;
        var resultingLeaderId = dto.LeaderId;

        if (resultingCompanyId == 0)
            resultingCompanyId = division.CompanyId;
        if (!dto.LeaderId.HasValue)
            resultingLeaderId = division.LeaderId;

        if (resultingLeaderId.HasValue)
        {
            var leader = await _context.Employees.FindAsync(resultingLeaderId.Value);
            if (leader == null)
                return BadRequest("Leader not found.");
            if (leader.CompanyId != resultingCompanyId)
                return BadRequest("Leader must be an employee of the company.");
        }

        division.Name = dto.Name;
        division.Code = dto.Code;
        division.CompanyId = dto.CompanyId;
        division.LeaderId = dto.LeaderId;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchDivision(int id, DivisionPatchDto dto)
    {
        var division = await _context.Divisions.FindAsync(id);
        if (division == null)
            return NotFound();

        // Compute resulting values without persisting yet
        var resultingCompanyId = dto.CompanyId.HasValue ? dto.CompanyId.Value : division.CompanyId;
        var resultingLeaderId = dto.LeaderId.HasValue ? dto.LeaderId : division.LeaderId;

        if (resultingLeaderId.HasValue)
        {
            var leader = await _context.Employees.FindAsync(resultingLeaderId.Value);
            if (leader == null)
                return BadRequest("Leader not found.");
            if (leader.CompanyId != resultingCompanyId)
                return BadRequest("Leader must be an employee of the company.");
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
            }
        }

        _context.Divisions.Remove(division);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}