using EmployeeManager.Domain.Entities;
using EmployeeManager.Domain.Interfaces;
using EmployeeManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManager.Infrastructure.Implementation
{
    internal class DepartmentRepository : IDepartmentRepository
    {
        private readonly AppDbContext _db;

        public DepartmentRepository(AppDbContext db)
        {
            _db = db;
        }
        public async Task<List<Department>> GetAllAsync(CancellationToken cancellationToken = default) 
           { 
            return await _db.Departments
                .AsNoTracking()
                .OrderBy(d => d.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _db.Departments.FindAsync(id, cancellationToken);
        }


        public async Task AddAsync(Department department, CancellationToken cancellationToken = default)
        {
            _db.Departments.Add(department);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Department department, CancellationToken cancellationToken = default)
        {
            _db.Departments.Update(department);
            await _db.SaveChangesAsync(cancellationToken);
        }



        public async Task<bool> NameExistsAsync(string name, int departmentId, CancellationToken cancellationToken = default)
        {
            return await _db.Departments.AnyAsync(d => d.Name == name && d.Id != departmentId, cancellationToken);
        }
        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var department = await GetByIdAsync(id, cancellationToken);
            if (department is null)
                return false;

            _db.Departments.Remove(department);
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
