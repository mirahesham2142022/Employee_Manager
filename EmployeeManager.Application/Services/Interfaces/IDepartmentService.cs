using EmployeeManager.Application.Dtos.DepartmentDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManager.Application.Services.Interfaces
{
   public interface IDepartmentService
    {
        Task<List<DepartmentDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

        Task<DepartmentDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<DepartmentDto> CreateAsync(
            CreateDepartmentDto dto,
            CancellationToken cancellationToken = default);

        Task<DepartmentDto> UpdateAsync(int id, UpdateDepartmentDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);

     
    }
}
