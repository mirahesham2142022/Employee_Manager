using EmployeeManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManager.Domain.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<(List<Employee> Items, int TotalCount)> 
            GetPageAsync(
        int page, int pageSize, int? departmentId,string searchTerm, CancellationToken cancellationToken = default);
        Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task AddAsync(Employee employee, CancellationToken cancellationToken = default);
        Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> EmailExistsAsync(string email, int employeeId, CancellationToken cancellationToken = default);
        Task<bool> AnyInDepartmentAsync(int departmentId, CancellationToken cancellationToken = default);
    }
}
