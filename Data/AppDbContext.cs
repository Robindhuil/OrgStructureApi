using Microsoft.EntityFrameworkCore;
using OrgStructureApi.Models;

namespace OrgStructureApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees => Set<Employee>();
    }
}
