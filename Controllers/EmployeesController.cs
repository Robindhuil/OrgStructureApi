using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrgStructureApi.Data;
using OrgStructureApi.Models;

namespace OrgStructureApi.Controllers;

public class EmployeePatchDto
{
    public string? Title { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}


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
    public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
    {
        return await _context.Employees.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Employee>> CreateEmployee(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEmployees), new { id = employee.Id }, employee);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, Employee updatedEmployee)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        employee.Title = updatedEmployee.Title;
        employee.FirstName = updatedEmployee.FirstName;
        employee.LastName = updatedEmployee.LastName;
        employee.Phone = updatedEmployee.Phone;
        employee.Email = updatedEmployee.Email;

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
