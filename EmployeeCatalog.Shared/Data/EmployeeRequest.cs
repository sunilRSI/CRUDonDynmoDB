using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCatalog.Shared.Data
{
    public class EmployeeRequest
    {
        [Required(ErrorMessage = "Please provide Name")]
        public string? Name { get; set; }  
        [Required(ErrorMessage = "Please provid Designation")]

        public string? Designation { get; set; } 
        [Required(ErrorMessage = "Please provid Age")]

       
        [Range(18, 60)]
        public int? Age { get; set; }
    }
}
