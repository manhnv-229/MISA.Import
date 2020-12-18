using MISA.ImportDemo.Core.Entities;
using MMISA.ImportDemo.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.ImportDemo.Core.Specifications
{
    public sealed class EmployeeSpecification : BaseSpecification<Employee>
    {
        public EmployeeSpecification(string citizenIdentityNo) : base(i => (i.IdentityNo == citizenIdentityNo))
        {
            //AddInclude(b => b.tableName);
        }
    }
}
