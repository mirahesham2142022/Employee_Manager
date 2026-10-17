using EmployeeManager.Application.Dtos.EmployeeDtos;
using EmployeeManager.Application.Exceptions;
using EmployeeManager.Application.Services.Interfaces;
using EmployeeManager.Domain.Entities;
using EmployeeManager.Domain.Interfaces;

namespace EmployeeManager.Application.Services.Implementation;

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
        string searchTerm,
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
            throw new ArgumentOutOfRangeException(nameof(page));

        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        var result = await _employees.GetPageAsync(
            page, pageSize, departmentId, searchTerm, cancellationToken);

        return new PagedResultDto<EmployeeDto>
        {
            Items = result.Items.Select(ToDto).ToList(),
            TotalCount = result.TotalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<EmployeeDto> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var employee = await _employees.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Employee {id} not found.");

        return ToDto(employee);
    }

    public async Task<EmployeeDto> CreateAsync(
        CreateEmployeeDto dto,
        CancellationToken cancellationToken = default)
    {
        var department = await _departments.GetByIdAsync(
            dto.DepartmentId, cancellationToken);

        if (department is null)
            throw new NotFoundException($"Department {dto.DepartmentId} not found.");

        // 0 means there is no existing employee ID to exclude.
        if (await _employees.EmailExistsAsync(dto.Email, 0, cancellationToken))
            throw new ConflictException("Email already exists.");

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

    public async Task<EmployeeDto> UpdateAsync(
        int id,
        UpdateEmployeeDto dto,
        CancellationToken cancellationToken = default)
    {
        var employee = await _employees.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Employee {id} not found.");

        var department = await _departments.GetByIdAsync(
            dto.DepartmentId, cancellationToken);

        if (department is null)
            throw new NotFoundException($"Department {dto.DepartmentId} not found.");

        if (await _employees.EmailExistsAsync(dto.Email, id, cancellationToken))
            throw new ConflictException("Email already exists.");

        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Email = dto.Email;
        employee.HireDate = dto.HireDate;
        employee.DepartmentId = dto.DepartmentId;

        await _employees.UpdateAsync(employee, cancellationToken);

        return ToDto(employee);
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var deleted = await _employees.DeleteAsync(id, cancellationToken);

        if (!deleted)
            throw new NotFoundException($"Employee {id} not found.");
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