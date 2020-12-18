using Microsoft.EntityFrameworkCore;
using MISA.ImportDemo.Core.Entities;
using MISA.ImportDemo.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository cho Nhân viên
    /// </summary>
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        public Task<IEnumerable<Employee>> GetEmployeeByFilter(object[] filter)
        {
            return Task.FromResult(GetEntities("Proc_GetEmployeeByFilter",filter));
        }
    }
}
