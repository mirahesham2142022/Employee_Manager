using EmployeeManager.Application.Dtos.EmployeeDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManager.Application.Services.Interfaces
{
    public  interface IEmployeeService
    {

        Task<PagedResultDto<EmployeeDto>> GetPageAsync(
            int page,
            int pageSize,
            int? departmentId,
            string searchTerm,
            CancellationToken cancellationToken = default);

        Task<EmployeeDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<EmployeeDto> UpdateAsync(int id, UpdateEmployeeDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto,CancellationToken cancellationToken = default);

    }
}
