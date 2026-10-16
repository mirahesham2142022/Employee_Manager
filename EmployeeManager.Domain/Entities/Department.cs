using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManager.Domain.Entities
{
    internal class Department
    {
        public string Name { get; set; } = string.Empty;
        public List<Employee> Employees { get; set; } = new();
    }
}
