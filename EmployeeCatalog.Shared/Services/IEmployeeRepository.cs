using EmployeeCatalog.Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCatalog.Shared.Services
{
    public interface IEmployeeRepository
    {
        public Task<Employee> GetEmployeeById(Guid id, CancellationToken cancellationToken);
        public Task<IEnumerable<Employee>> GetAllEmployee(CancellationToken cancellationToken);
        public Task<Employee> CreateEmployee(EmployeeRequest employee, CancellationToken cancellationToken);
        public Task UpdateEmployee(Employee employee, CancellationToken cancellationToken);
        public Task DeleteEmployee(Guid id, CancellationToken cancellationToken);
    }
}
