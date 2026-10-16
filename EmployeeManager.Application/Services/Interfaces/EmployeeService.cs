using EmployeeManager.Application.Dtos.EmployeeDtos;
using EmployeeManager.Application.Services.Implementation;
using EmployeeManager.Domain.Entities;
using EmployeeManager.Domain.Interfaces;

namespace EmployeeManager.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employees;
    private readonly IDepartmentRepository _departments;

    public EmployeeService(
        IEmployeeRepository employees,
        IDepartmentRepository departments)
    {
        _employees = employees;
        _departments = departments;
    }

    public async Task<PagedResultDto<EmployeeDto>> GetPageAsync(
        int page,
        int pageSize,
        int? departmentId,
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
            throw new ArgumentOutOfRangeException(nameof(page));

        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        var result = await _employees.GetPageAsync(
            page, pageSize, departmentId, cancellationToken);

        return new PagedResultDto<EmployeeDto>
        {
            Items = result.Items.Select(e=>ToDto(e)).ToList(),
            TotalCount = result.TotalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id,CancellationToken cancellationToken = default)
    {
        var employee = await _employees.GetByIdAsync(id, cancellationToken);
        return employee is null ? null : ToDto(employee);
    }

    public async Task<EmployeeDto> CreateAsync(
        CreateEmployeeDto dto,
        CancellationToken cancellationToken = default)
    {
        var department = await _departments.GetByIdAsync(
            dto.DepartmentId, cancellationToken);

        if (department is null)
            throw new InvalidOperationException("Department was not found.");

        // 0 means there is no existing employee ID to exclude.
        if (await _employees.EmailExistsAsync(
                dto.Email, 0, cancellationToken))
        {
            throw new InvalidOperationException("Email is already in use.");
        }

        var employee = new Employee
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            HireDate = dto.HireDate,
            DepartmentId = dto.DepartmentId,
            CreatedAt = DateTime.UtcNow
        };

        await _employees.AddAsync(employee, cancellationToken);

        var savedEmployee = await _employees.GetByIdAsync(
            employee.Id, cancellationToken);

        return ToDto(savedEmployee ?? employee);
    }

    public async Task<EmployeeDto?> UpdateAsync(
        int id,
        UpdateEmployeeDto dto,
        CancellationToken cancellationToken = default)
    {
        var employee = await _employees.GetByIdAsync(id, cancellationToken);

        if (employee is null)
            return null;

        var department = await _departments.GetByIdAsync(
            dto.DepartmentId, cancellationToken);

        if (department is null)
            throw new InvalidOperationException("Department was not found.");

        if (await _employees.EmailExistsAsync(
                dto.Email, id, cancellationToken))
        {
            throw new InvalidOperationException("Email is already in use.");
        }

        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Email = dto.Email;
        employee.HireDate = dto.HireDate;
        employee.DepartmentId = dto.DepartmentId;

        await _employees.UpdateAsync(employee, cancellationToken);

        return ToDto(employee);
    }

    public Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _employees.DeleteAsync(id, cancellationToken);
    }

    private static EmployeeDto ToDto(Employee employee)
    {
        return new EmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            HireDate = employee.HireDate,
            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.Department?.Name ?? string.Empty,
            CreatedAt = employee.CreatedAt
        };
    }
}