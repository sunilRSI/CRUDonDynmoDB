using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCatalog.Shared.Services
{
    public interface IDbContext
    {
        public Task Initilize(string TableName);
    }
}
