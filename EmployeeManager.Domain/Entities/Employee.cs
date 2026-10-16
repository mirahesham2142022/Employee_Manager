using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManager.Domain.Entities
{
    internal class Employee
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateOnly HireDate { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!; 
    }
}
