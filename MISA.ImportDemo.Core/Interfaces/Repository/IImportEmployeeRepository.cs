using MISA.ImportDemo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Core.Interfaces.Repository
{
    public interface IImportEmployeeRepository : IBaseImportRepository
    {
        Task<List<Employee>> GetEmployees();
    }
}
