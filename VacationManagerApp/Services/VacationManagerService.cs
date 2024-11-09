using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VacationManagerApp.Data;
using VacationManagerApp.Entities;

namespace VacationManagerApp.Services;

public interface IVacationManagerService
{
    Task<List<Employee>> GetEmployeesAsync(CancellationToken cancellationToken = default);
    Task<List<Vacation>> GetEmployeeVacationsAsync(int employeeId, CancellationToken cancellationToken = default);
    Task<Employee> GetEmployeeAsync(int employeeId, CancellationToken cancellationToken = default);
    Task AddVacationAsync(Vacation vacation, CancellationToken cancellationToken = default);
    Task UpdateVacationAsync(Vacation vacation, CancellationToken cancellationToken = default);
    Task DeleteVacationAsync(int vacationId, CancellationToken cancellationToken = default);
}

public class VacationManagerService : IVacationManagerService
{
    private readonly VacationManagerContext _context;

    public VacationManagerService(VacationManagerContext context)
    {
        _context = context;
    }

    public async Task<List<Employee>> GetEmployeesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Employees.Include(x => x.Vacations).ToListAsync(cancellationToken);
    }

    public async Task<Employee> GetEmployeeAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        var result = await _context.Employees.Include(x => x.Vacations)
            .FirstOrDefaultAsync(x => x.Id == employeeId, cancellationToken);

        if (result == null) throw new KeyNotFoundException("No employee found.");

        return result;
    }

    public async Task<List<Vacation>> GetEmployeeVacationsAsync(int employeeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Vacations.Where(x => x.EmployeeId == employeeId).Include(x => x.Employee)
            .ToListAsync(cancellationToken);
    }

    public async Task AddVacationAsync(Vacation vacation, CancellationToken cancellationToken = default)
    {
        _context.Add(vacation);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateVacationAsync(Vacation vacation, CancellationToken cancellationToken = default)
    {
        _context.Update(vacation);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteVacationAsync(int vacationId, CancellationToken cancellationToken = default)
    {
        var vacation = await _context.Vacations.FirstOrDefaultAsync(x => x.Id == vacationId, cancellationToken);
        _context.Remove(vacation);
        await _context.SaveChangesAsync(cancellationToken);
    }
}