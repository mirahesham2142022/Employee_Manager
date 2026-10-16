using EmployeeManager.Application.Dtos.DepartmentDtos;
using EmployeeManager.Application.Services.Implementation;
using EmployeeManager.Domain.Entities;
using EmployeeManager.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManager.Application.Services.Interfaces
{
    internal class DepartmentService:IDepartmentService
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
            return  departments.Select(d=>(ToDto(d))).ToList();
        }

        public async Task<DepartmentDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var department = await _departments.GetByIdAsync(id, cancellationToken);
            return department is null ? null : ToDto(department);
        }

        public async Task<DepartmentDto> CreateAsync(
            CreateDepartmentDto dto,
            CancellationToken cancellationToken = default)
        {
            var name = dto.Name;

            // 0 means there is no existing department ID to exclude it
            if (await _departments.NameExistsAsync(name, 0, cancellationToken))
                throw new InvalidOperationException("Department name is already in use.");

            var department = new Department
            {
                Name = name,
                CreatedAt = DateTime.UtcNow
            };

            await _departments.AddAsync(department, cancellationToken);
            return ToDto(department);
        }

        public async Task<DepartmentDto?> UpdateAsync(
            int id,
            UpdateDepartmentDto dto,
            CancellationToken cancellationToken = default)
        {
            var department = await _departments.GetByIdAsync(id, cancellationToken);

            if (department is null)
                return null;

            var name = dto.Name;

            if (await _departments.NameExistsAsync(name, id, cancellationToken))
                throw new InvalidOperationException("Department name is already in use.");

            department.Name = name;
            await _departments.UpdateAsync(department, cancellationToken);

            return ToDto(department);
        }

        public async Task<bool> DeleteAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var department = await _departments.GetByIdAsync(id, cancellationToken);

            if (department is null)
                return false;

            if (await _employees.AnyInDepartmentAsync(id, cancellationToken))
                throw new InvalidOperationException(
                    "Cannot delete a department that has employees.");

            return await _departments.DeleteAsync(id, cancellationToken);
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
}
