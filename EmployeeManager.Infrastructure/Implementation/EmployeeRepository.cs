using EmployeeManager.Domain.Entities;
using EmployeeManager.Domain.Interfaces;
using EmployeeManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManager.Infrastructure.Repositories;

internal class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _db;

    public EmployeeRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(List<Employee> Items, int TotalCount)> GetPageAsync(
    int page, int pageSize, int? departmentId, CancellationToken cancellationToken = default)
    {
        var query = _db.Employees.AsNoTracking();

        if (departmentId is not null)
            query = query.Where(e => e.DepartmentId == departmentId);
        var totalCount = await query.CountAsync(cancellationToken);//return count 
       
        var items = await query
            .Include(e => e.Department)
            .OrderBy(e => e.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task AddAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        _db.Employees.Add(employee);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        _db.Employees.Update(employee);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await GetByIdAsync(id, cancellationToken);
        if (employee is null)
            return false;

        _db.Employees.Remove(employee);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<bool> EmailExistsAsync(string email, int employeeId, CancellationToken cancellationToken = default) =>
        _db.Employees.AnyAsync(e => e.Email == email && e.Id != employeeId, cancellationToken);

    public Task<bool> AnyInDepartmentAsync(int departmentId, CancellationToken cancellationToken = default) =>
        _db.Employees.AnyAsync(e => e.DepartmentId == departmentId, cancellationToken);

    
}