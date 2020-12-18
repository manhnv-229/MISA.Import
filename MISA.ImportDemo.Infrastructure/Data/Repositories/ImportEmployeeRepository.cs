using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MISA.ImportDemo.Core.Entities;
using MISA.ImportDemo.Core.Enumeration;
using MISA.ImportDemo.Core.Interfaces;
using MISA.ImportDemo.Core.Interfaces.Base;
using MISA.ImportDemo.Core.Interfaces.Repository;
using MISA.ImportDemo.Core.Properties;
using MISA.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Infrastructure.Data.Repositories
{
    public class ImportEmployeeRepository : BaseImportRepository, IImportEmployeeRepository
    {

        public ImportEmployeeRepository(IEntityRepository entityRepository, IMemoryCache importMemoryCache) : base(entityRepository, importMemoryCache)
        {

        }

        /// <summary>
        /// Thực hiện nhập khẩu nhân viên
        /// </summary>
        /// <param name="importKey"></param>
        /// <param name="overriderData"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<ActionServiceResult> Import(string importKey, bool overriderData, CancellationToken cancellationToken)
        {
            var employees = ((List<Employee>)CacheGet(importKey)).Where(e => e.ImportValidState == ImportValidState.Valid || (overriderData && e.ImportValidState == ImportValidState.DuplicateInDb)).ToList(); ;

            using var dbContext = new EfDbContext();

            // Danh sách các vị trí/ chức vụ mới:
            var newPositons = ((List<Position>)CacheGet(string.Format("Position_{0}", importKey)));
            await dbContext.Position.AddRangeAsync(newPositons);

            // Danh sách nhân viên thêm mới:
            var newEmployees = employees.Where(e => e.ImportValidState == Core.Enumeration.ImportValidState.Valid).ToList();
            await dbContext.Employee.AddRangeAsync(newEmployees);

            // Danh sách nhân viên thực hiện ghi đè:
            var modifiedEmployees = employees.Where(e => e.ImportValidState == Core.Enumeration.ImportValidState.DuplicateInDb).ToList();
            foreach (var emp in modifiedEmployees)
            {
                dbContext.Entry(emp).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                var pbd = dbContext.EmployeeFamily.Where(e => e.EmployeeId == emp.EmployeeId);
                dbContext.EmployeeFamily.AddRange(emp.EmployeeFamily);
                dbContext.EmployeeFamily.RemoveRange(pbd);
            }
            //dbContext.Employee.UpdateRange(modifiedEmployees);
            await dbContext.SaveChangesAsync();
            return new ActionServiceResult(true, Resources.Msg_ImportSuccess, MISACode.Success, employees);
        }

        public async Task<List<Employee>> GetEmployees()
        {
            var currentOrganizationId = CommonUtility.GetCurrentOrganizationId();
            using var dbContext = new EfDbContext();
            var employees = await dbContext.Employee.Where(e => e.OrganizationId == currentOrganizationId).ToListAsync();
            return employees;
        }
    }
}
