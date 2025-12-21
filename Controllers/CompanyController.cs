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
    public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
    {
        var companies = await _context.Companies
            .Include(c => c.Divisions)
            .Include(c => c.Employees)
            .ToListAsync();
        return companies;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Company>> GetCompany(int id)
    {
        var company = await _context.Companies
            .Include(c => c.Divisions)
            .Include(c => c.Employees)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (company == null)
            return NotFound();
        return company;
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
        return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, company);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCompany(int id, CompanyUpdateDto dto)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company == null)
            return NotFound();
        company.Name = dto.Name;
        company.Code = dto.Code;
        company.DirectorId = dto.DirectorId;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchCompany(int id, CompanyPatchDto dto)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company == null)
            return NotFound();
        if (dto.Name != null)
            company.Name = dto.Name;
        if (dto.Code != null)
            company.Code = dto.Code;
        if (dto.DirectorId.HasValue)
            company.DirectorId = dto.DirectorId;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCompany(int id)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company == null)
            return NotFound();
        _context.Companies.Remove(company);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
