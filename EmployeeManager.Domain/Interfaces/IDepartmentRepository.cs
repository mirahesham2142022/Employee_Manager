using EmployeeManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManager.Domain.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<List<Department>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task AddAsync(Department department, CancellationToken cancellationToken = default);
        Task UpdateAsync(Department department, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> NameExistsAsync(string name, int departmentId, CancellationToken cancellationToken = default);
    }
}
