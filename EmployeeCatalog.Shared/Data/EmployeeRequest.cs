using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCatalog.Shared.Data
{
    public class EmployeeRequest
    {
        public string? Name { get; set; }

        public string? Designation { get; set; }
        //[Range(18, 120)]
        public int? Age { get; set; }
    }
}
