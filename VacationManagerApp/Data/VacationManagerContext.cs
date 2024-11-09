using Microsoft.EntityFrameworkCore;
using VacationManagerApp.Entities;

namespace VacationManagerApp.Data;

public class VacationManagerContext : DbContext
{
    public VacationManagerContext(DbContextOptions<VacationManagerContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Vacation> Vacations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>().HasData(
            new Employee
                { Id = 1, FirstName = "John", LastName = "Doe", TotalVacationDays = 25, RemainingVacationDays = 25 },
            new Employee
                { Id = 2, FirstName = "Jane", LastName = "Doe", TotalVacationDays = 30, RemainingVacationDays = 30 },
            new Employee
                { Id = 2, FirstName = "Anthony", LastName = "Hopkins", TotalVacationDays = 20, RemainingVacationDays = 20 }
        );

        modelBuilder.Entity<Vacation>()
            .HasOne(v => v.Employee)
            .WithMany(e => e.Vacations)
            .HasForeignKey(v => v.EmployeeId);
    }
}