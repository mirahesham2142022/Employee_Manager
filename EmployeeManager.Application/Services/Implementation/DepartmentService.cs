using EmployeeManager.Application.Dtos.DepartmentDtos;
using EmployeeManager.Application.Exceptions;
using EmployeeManager.Application.Services.Interfaces;
using EmployeeManager.Domain.Entities;
using EmployeeManager.Domain.Interfaces;

namespace EmployeeManager.Application.Services.Implementation;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departments;
    private readonly IEmployeeRepository _employees;

    public DepartmentService(
        IDepartmentRepository departments,
        IEmployeeRepository employees)
    {
        _departments = departments;
        _employees = employees;
    }

    public async Task<List<DepartmentDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var departments = await _departments.GetAllAsync(cancellationToken);
        return departments.Select(ToDto).ToList();
    }

    public async Task<DepartmentDto> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var department = await _departments.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Department {id} not found.");

        return ToDto(department);
    }

    public async Task<DepartmentDto> CreateAsync(
        CreateDepartmentDto dto,
        CancellationToken cancellationToken = default)
    {
        // 0 means there is no existing department ID to exclude.
        if (await _departments.NameExistsAsync(dto.Name, 0, cancellationToken))
            throw new ConflictException("Department name already exists.");

        var department = new Department
        {
            Name = dto.Name,
            CreatedAt = DateTime.UtcNow
        };

        await _departments.AddAsync(department, cancellationToken);
        return ToDto(department);
    }

    public async Task<DepartmentDto> UpdateAsync(
        int id,
        UpdateDepartmentDto dto,
        CancellationToken cancellationToken = default)
    {
        var department = await _departments.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Department {id} not found.");

        if (await _departments.NameExistsAsync(dto.Name, id, cancellationToken))
            throw new ConflictException("Department name already exists.");

        department.Name = dto.Name;
        await _departments.UpdateAsync(department, cancellationToken);

        return ToDto(department);
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var department = await _departments.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Department {id} not found.");

        if (await _employees.AnyInDepartmentAsync(id, cancellationToken))
            throw new ConflictException("Cannot delete a department that has employees.");

        await _departments.DeleteAsync(id, cancellationToken);
    }

    private static DepartmentDto ToDto(Department department)
    {
        return new DepartmentDto
        {
            Id = department.Id,
            Name = department.Name,
            CreatedAt = department.CreatedAt
        };
    }
}