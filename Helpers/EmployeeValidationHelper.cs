
using OrgStructureApi.Models;
using OrgStructureApi.Data;
using Microsoft.EntityFrameworkCore;

namespace OrgStructureApi.Helpers
{

    public static class EmployeeValidationHelper
    {
        public static async Task<bool> IsEmployeeLeaderOrDirector(AppDbContext context, int employeeId, int companyId)
        {
            return await context.Companies.AnyAsync(c => c.DirectorId == employeeId && c.Id == companyId)
                || await context.Divisions.AnyAsync(d => d.LeaderId == employeeId && d.CompanyId == companyId)
                || await context.Projects.AnyAsync(p => p.LeaderId == employeeId && p.Division.CompanyId == companyId)
                || await context.Departments.AnyAsync(d => d.LeaderId == employeeId && d.Project.Division.CompanyId == companyId);
        }

        public static async Task<bool> IsEmployeeInCompany(AppDbContext context, int employeeId, int companyId)
        {
            var employee = await context.Employees.FindAsync(employeeId);
            return employee != null && employee.CompanyId == companyId;
        }
    }
}
