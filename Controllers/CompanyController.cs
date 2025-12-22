using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrgStructureApi.Data;
using OrgStructureApi.Models;
using OrgStructureApi.Dtos;

namespace OrgStructureApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly AppDbContext _context;

    public CompanyController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CompanyReadDto>>> GetCompanies()
    {
        var companies = await _context.Companies
            .Select(c => new CompanyReadDto
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                DirectorId = c.DirectorId,
                DivisionsCount = c.Divisions != null ? c.Divisions.Count : 0,
                EmployeesCount = c.Employees != null ? c.Employees.Count : 0
            })
            .ToListAsync();

        return Ok(companies);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CompanyReadDto>> GetCompany(int id)
    {
        var company = await _context.Companies
            .Where(c => c.Id == id)
            .Select(c => new CompanyReadDto
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                DirectorId = c.DirectorId,
                DivisionsCount = c.Divisions != null ? c.Divisions.Count : 0,
                EmployeesCount = c.Employees != null ? c.Employees.Count : 0
            })
            .FirstOrDefaultAsync();

        if (company == null)
            return NotFound();

        return Ok(company);
    }

    [HttpPost]
    public async Task<ActionResult<Company>> CreateCompany(CompanyCreateDto dto)
    {
        var company = new Company
        {
            Name = dto.Name,
            Code = dto.Code,
            DirectorId = dto.DirectorId
        };
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        if (company.DirectorId.HasValue)
        {
            var director = await _context.Employees.FindAsync(company.DirectorId.Value);
            if (director == null)
                return BadRequest("Director not found.");
            if (director.CompanyId != company.Id)
            {
                director.CompanyId = company.Id;
            }
            var otherCompany = await _context.Companies.FirstOrDefaultAsync(c => c.Id != company.Id && c.DirectorId == director.Id);
            if (otherCompany != null)
            {
                otherCompany.DirectorId = null;
            }
            await _context.SaveChangesAsync();
        }
        return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, company);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCompany(int id, CompanyUpdateDto dto)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company == null)
            return NotFound();

        if (company.DirectorId.HasValue && company.DirectorId != dto.DirectorId)
        {
            var oldDirector = await _context.Employees.FindAsync(company.DirectorId.Value);
            if (oldDirector != null)
            {
                var otherCompany = await _context.Companies.FirstOrDefaultAsync(c => c.Id != company.Id && c.DirectorId == oldDirector.Id);
                if (otherCompany != null)
                {
                    otherCompany.DirectorId = null;
                }
            }
        }

        company.Name = dto.Name;
        company.Code = dto.Code;
        company.DirectorId = dto.DirectorId;
        await _context.SaveChangesAsync();

        if (company.DirectorId.HasValue)
        {
            var director = await _context.Employees.FindAsync(company.DirectorId.Value);
            if (director == null)
                return BadRequest("Director not found.");
            if (director.CompanyId != company.Id)
                return BadRequest("Director must be an employee of this company.");
            var otherCompany = await _context.Companies.FirstOrDefaultAsync(c => c.Id != company.Id && c.DirectorId == director.Id);
            if (otherCompany != null)
            {
                otherCompany.DirectorId = null;
            }
            await _context.SaveChangesAsync();
        }
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchCompany(int id, CompanyPatchDto dto)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company == null)
            return NotFound();

        if (dto.DirectorId.HasValue && company.DirectorId.HasValue && company.DirectorId != dto.DirectorId)
        {
            var oldDirector = await _context.Employees.FindAsync(company.DirectorId.Value);
            if (oldDirector != null)
            {
                var otherCompany = await _context.Companies.FirstOrDefaultAsync(c => c.Id != company.Id && c.DirectorId == oldDirector.Id);
                if (otherCompany != null)
                {
                    otherCompany.DirectorId = null;
                }
            }
        }

        if (dto.Name != null)
            company.Name = dto.Name;
        if (dto.Code != null)
            company.Code = dto.Code;
        if (dto.DirectorId.HasValue)
            company.DirectorId = dto.DirectorId;
        await _context.SaveChangesAsync();

        if (company.DirectorId.HasValue)
        {
            var director = await _context.Employees.FindAsync(company.DirectorId.Value);
            if (director == null)
                return BadRequest("Director not found.");
            if (director.CompanyId != company.Id)
                return BadRequest("Director must be an employee of this company.");
            var otherCompany = await _context.Companies.FirstOrDefaultAsync(c => c.Id != company.Id && c.DirectorId == director.Id);
            if (otherCompany != null)
            {
                otherCompany.DirectorId = null;
            }
            await _context.SaveChangesAsync();
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCompany(int id)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company == null)
            return NotFound();

        if (company.DirectorId.HasValue)
        {
            var director = await _context.Employees.FindAsync(company.DirectorId.Value);
            if (director != null)
            {
            }
        }

        _context.Companies.Remove(company);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
