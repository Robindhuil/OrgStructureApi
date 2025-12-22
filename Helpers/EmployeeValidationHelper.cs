
using OrgStructureApi.Models;
using OrgStructureApi.Data;
using Microsoft.EntityFrameworkCore;

namespace OrgStructureApi.Helpers
{

    public static class EmployeeValidationHelper
    {
        public static async Task<bool> IsEmployeeLeaderOrDirector(AppDbContext context, int employeeId)
        {
            return await context.Companies.AnyAsync(c => c.DirectorId == employeeId)
                || await context.Divisions.AnyAsync(d => d.LeaderId == employeeId)
                || await context.Projects.AnyAsync(p => p.LeaderId == employeeId)
                || await context.Departments.AnyAsync(d => d.LeaderId == employeeId);
        }

        public static async Task<bool> IsEmployeeInCompany(AppDbContext context, int employeeId, int companyId)
        {
            var employee = await context.Employees.FindAsync(employeeId);
            return employee != null && employee.CompanyId == companyId;
        }
    }
}
