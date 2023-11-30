using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCatalog.Shared.Data
{
    [DynamoDBTable("Employee")]
    public class Employee
    {
        [DynamoDBProperty("Id")]
        [DynamoDBHashKey]
        public Guid Id { get; set; }

        //[DynamoDBRangeKey]
        [DynamoDBProperty("Name")]
        public string? Name { get; set; }

        [DynamoDBProperty("Designation")]
        public string? Designation { get; set; }

        [DynamoDBProperty("Age")]
        public int? Age { get; set; }
    }
}
