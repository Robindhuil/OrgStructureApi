using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrgStructureApi.Data;
using OrgStructureApi.Models;

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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees(
        string? lastName,
        OrgStructureApi.Models.EmployeeRole? role = null,
        int page = 1,
        int pageSize = 20)
    {
        var query = _context.Employees.AsQueryable();

        if (!string.IsNullOrEmpty(lastName))
            query = query.Where(e => e.LastName.Contains(lastName));
        if (role.HasValue)
            query = query.Where(e => e.Role == role.Value);

        return await query
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
            Role = dto.Role
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

        employee.Title = dto.Title;
        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Phone = dto.Phone;
        employee.Email = dto.Email;
        employee.Role = dto.Role;

        await _context.SaveChangesAsync();
        return NoContent();
    }


    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchEmployee(int id, [FromBody] EmployeePatchDto updatedFields)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null) return NotFound();

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

        if (updatedFields.Role.HasValue)
            employee.Role = updatedFields.Role.Value;

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

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return NoContent();
    }

}
