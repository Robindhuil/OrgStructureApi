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
        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Division> Divisions => Set<Division>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<Department> Departments => Set<Department>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>()
                .HasMany(c => c.Divisions)
                .WithOne(d => d.Company)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Division>()
                .HasMany(d => d.Projects)
                .WithOne(p => p.Division)
                .HasForeignKey(p => p.DivisionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.Departments)
                .WithOne(d => d.Project)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Company>()
                .HasOne(c => c.Director)
                .WithMany()
                .HasForeignKey(c => c.DirectorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Division>()
                .HasOne(d => d.Leader)
                .WithMany(e => e.DivisionsLed)
                .HasForeignKey(d => d.LeaderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Leader)
                .WithMany(e => e.ProjectsLed)
                .HasForeignKey(p => p.LeaderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Department>()
                .HasOne(d => d.Leader)
                .WithMany(e => e.DepartmentsLed)
                .HasForeignKey(d => d.LeaderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Company)
                .WithMany(c => c.Employees)
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Phone)
                .IsUnique();

            modelBuilder.Entity<Company>()
                .HasIndex(c => c.Code)
                .IsUnique();

            modelBuilder.Entity<Division>()
                .HasIndex(d => new { d.CompanyId, d.Code })
                .IsUnique();

            modelBuilder.Entity<Project>()
                .HasIndex(p => new { p.DivisionId, p.Code })
                .IsUnique();

            modelBuilder.Entity<Department>()
                .HasIndex(d => new { d.ProjectId, d.Code })
                .IsUnique();
        }
    }
}
