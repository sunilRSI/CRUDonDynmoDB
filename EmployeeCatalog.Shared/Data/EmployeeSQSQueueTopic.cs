using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeCatalog.Shared.Data
{
    public class EmployeeSQSQueueTopic
    { 

        public const string EmpCreated = "mysqscreate"; 

        public const string EmpDeleted = "MySQSQuqueDelete";

        public static readonly string[] AllTopics = { EmpCreated, EmpDeleted };
    }
}
