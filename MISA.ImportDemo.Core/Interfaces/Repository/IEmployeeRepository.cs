using MISA.ImportDemo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Core.Interfaces
{
    /// <summary>
    /// interface quản lý nghiệp vụ phần đơn vị
    /// </summary>
    public interface IEmployeeRepository 
    {
        Task<IEnumerable<Employee>> GetEmployeeByFilter(object[] filter);



    }
}
