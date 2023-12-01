using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using EmployeeCatalog.Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCatalog.Shared.Services
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDynamoDBContext _dynamoDBContext;

        public EmployeeRepository(IDynamoDBContext context)
        {
            _dynamoDBContext = context;
        }
        public async Task<Employee> CreateEmployee(EmployeeRequest Request, CancellationToken cancellationToken)
        {
            Employee employee = new Employee();
            employee.Id = Guid.NewGuid();
            employee.Name = Request.Name;
            employee.Designation = Request.Designation;
            employee.Age = Request.Age;
            await _dynamoDBContext.SaveAsync<Employee>(employee, cancellationToken);
            return employee;
        }

        public async Task DeleteEmployee(Guid Id, CancellationToken cancellationToken)
        {
            await _dynamoDBContext.DeleteAsync<Employee>(Id, cancellationToken);
        }
        public async Task<IEnumerable<Employee>> GetAllEmployee(CancellationToken cancellationToken)
        {
            //var ss = await All();
            return await _dynamoDBContext.ScanAsync<Employee>(default).GetRemainingAsync(cancellationToken);
        }

        public async Task<Employee> GetEmployeeById(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dynamoDBContext.LoadAsync<Employee>(id, cancellationToken);

            // This will work if table contains range key e.g id is hashkey and name is the range key
            //var result = await _dynamoDBContext.QueryAsync<Employee>(id).GetRemainingAsync();
            //return result[0];

        }

        public async Task UpdateEmployee(Employee Request, CancellationToken cancellationToken)
        {
            await _dynamoDBContext.SaveAsync(Request, cancellationToken);
        }
        public async Task<IEnumerable<Employee>> FindEmployee(EmployeeRequest Request, CancellationToken cancellationToken)
        {
            var scanConditions = new List<ScanCondition>();
            if (!string.IsNullOrEmpty(Request.Name.ToString()))
                scanConditions.Add(new ScanCondition("Name", ScanOperator.Equal, Request.Name));
            if (!string.IsNullOrEmpty(Request.Designation))
                scanConditions.Add(new ScanCondition("Designation", ScanOperator.Equal, Request.Designation));
            
            return await _dynamoDBContext.ScanAsync<Employee>(scanConditions, null).GetRemainingAsync();
        }
        private async Task<IEnumerable<Employee>> All(string paginationToken = "")
        {
            // Get the Table ref from the Model
            var table = _dynamoDBContext.GetTargetTable<Employee>();

            // If there's a PaginationToken
            // Use it in the Scan options
            // to fetch the next set
            var scanOps = new ScanOperationConfig();

            if (!string.IsNullOrEmpty(paginationToken))
            {
                scanOps.PaginationToken = paginationToken;
            }

            // returns the set of Document objects
            // for the supplied ScanOptions
            var results = table.Scan(scanOps);
            List<Document> data = await results.GetNextSetAsync();

            // transform the generic Document objects
            // into our Entity Model
            IEnumerable<Employee> employees = _dynamoDBContext.FromDocuments<Employee>(data);
            return employees;

            /* The Non-Pagination approach */
            //var scanConditions = new List<ScanCondition>() { new ScanCondition("Id", ScanOperator.IsNotNull) };
            //var searchResults = _context.ScanAsync<Employee>(scanConditions, null);
            //return await searchResults.GetNextSetAsync();

        } 
    }
}
