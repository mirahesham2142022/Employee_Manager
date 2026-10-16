using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManager.Domain.Entities
{
    internal abstract class BaseEntity
    {
        public int Id { get; set; }
      
        public DateTime CreatedAt { get; set; }
    }
}
