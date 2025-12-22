using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrgStructureApi.Data;
using OrgStructureApi.Models;
using OrgStructureApi.Helpers;

namespace OrgStructureApi.Controllers;


[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _context;

    public EmployeesController(AppDbContext context)
    {
        _context = context;
    }

    private async Task<bool> IsLeaderOrDirectorInCompany(int employeeId, int? companyId)
    {
        if (!companyId.HasValue) return false;
        var cid = companyId.Value;

        var isDirector = await _context.Companies.AnyAsync(c => c.DirectorId == employeeId && c.Id == cid);
        if (isDirector) return true;

        var isDivisionLeader = await _context.Divisions.AnyAsync(d => d.LeaderId == employeeId && d.CompanyId == cid);
        if (isDivisionLeader) return true;

        var isProjectLeader = await _context.Projects
            .Include(p => p.Division)
            .AnyAsync(p => p.LeaderId == employeeId && p.Division != null && p.Division.CompanyId == cid);
        if (isProjectLeader) return true;

        var isDepartmentLeader = await _context.Departments
            .Include(d => d.Project)
            .ThenInclude(p => p.Division)
            .AnyAsync(d => d.LeaderId == employeeId && d.Project != null && d.Project.Division != null && d.Project.Division.CompanyId == cid);
        return isDepartmentLeader;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees(
        string? lastName,
        int page = 1,
        int pageSize = 20)
    {
        var query = _context.Employees
            .AsQueryable();

        if (!string.IsNullOrEmpty(lastName))
            query = query.Where(e => e.LastName.Contains(lastName));

        return await query
            .OrderBy(e => e.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> GetEmployeeById(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
            return NotFound();

        return employee;
    }


    [HttpPost]
    public async Task<ActionResult<Employee>> CreateEmployee(EmployeeCreateDto dto)
    {
        var employee = new Employee
        {
            Title = dto.Title,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Phone = dto.Phone,
            Email = dto.Email,
            CompanyId = dto.CompanyId
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, employee);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, EmployeeUpdateDto dto)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
            return NotFound();

        if (employee.CompanyId.HasValue && employee.CompanyId != dto.CompanyId)
        {
            if (await IsLeaderOrDirectorInCompany(id, employee.CompanyId))
                return BadRequest("Cannot change company for an employee who is a leader or director.");
        }

        employee.Title = dto.Title;
        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Phone = dto.Phone;
        employee.Email = dto.Email;
        employee.CompanyId = dto.CompanyId;

        await _context.SaveChangesAsync();
        return NoContent();
    }


    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchEmployee(int id, [FromBody] EmployeePatchDto updatedFields)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null) return NotFound();

        if (updatedFields.CompanyId.HasValue && employee.CompanyId.HasValue && employee.CompanyId != updatedFields.CompanyId.Value)
        {
            if (await IsLeaderOrDirectorInCompany(id, employee.CompanyId))
                return BadRequest("Cannot change company for an employee who is a leader or director.");
        }

        if (!string.IsNullOrEmpty(updatedFields.Title))
            employee.Title = updatedFields.Title;

        if (!string.IsNullOrEmpty(updatedFields.FirstName))
            employee.FirstName = updatedFields.FirstName;

        if (!string.IsNullOrEmpty(updatedFields.LastName))
            employee.LastName = updatedFields.LastName;

        if (!string.IsNullOrEmpty(updatedFields.Phone))
            employee.Phone = updatedFields.Phone;

        if (!string.IsNullOrEmpty(updatedFields.Email))
            employee.Email = updatedFields.Email;

        if (updatedFields.CompanyId.HasValue)
            employee.CompanyId = updatedFields.CompanyId.Value;


        await _context.SaveChangesAsync();
        return NoContent();
    }



    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        var company = await _context.Companies.FirstOrDefaultAsync(c => c.DirectorId == id);
        if (company != null)
        {
            company.DirectorId = null;
        }

        var division = await _context.Divisions.FirstOrDefaultAsync(d => d.LeaderId == id);
        if (division != null)
        {
            division.LeaderId = null;
        }

        var project = await _context.Projects.FirstOrDefaultAsync(p => p.LeaderId == id);
        if (project != null)
        {
            project.LeaderId = null;
        }

        var department = await _context.Departments.FirstOrDefaultAsync(dep => dep.LeaderId == id);
        if (department != null)
        {
            department.LeaderId = null;
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return NoContent();
    }

}
