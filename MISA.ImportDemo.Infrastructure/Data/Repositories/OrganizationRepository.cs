using MISA.ImportDemo.Core.Entities;
using MISA.ImportDemo.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Infrastructure.Data.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public class OrganizationRepository : EfBaseRepository<Organization>, IOrganizationRepository
    {
        /// <summary>
        /// Hàm thực hiện lấy đơn vị theo mã đơn vị
        /// </summary>
        /// <param name="organizationCode">mã đơn vị</param>
        /// <returns></returns>
        /// ntngoc - 25/04/2020
        public IEnumerable<Organization> GetOrganizationByOrganizationCode(string organizationCode)
        {
            return GetData<Organization>("Proc_GetOrganizationByOrganizationCode", new object[] { organizationCode });
        }

        /// <summary>
        /// Hàm thực hiện thêm mới đơn vị
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<object> InsertOrganizationAndReturnSingleValue(Organization entity)
        {
            var procedureName = "Proc_InsertOrganization";
            using (DatabaseConnector databaseConnector = new DatabaseConnector())
            {
                var value = await SaveChangeAndReturnSingleValue(databaseConnector, procedureName, entity);
                databaseConnector.CommitTransaction();
                return value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseConnector"></param>
        /// <param name="procedureName"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        private Task<object> SaveChangeAndReturnSingleValue(DatabaseConnector databaseConnector, string procedureName, Organization entity)
        {
            databaseConnector.MapParameterValueAndEntityProperty<Organization>(procedureName, entity);
            return databaseConnector.ExecuteScalar();
        }
        /// <summary>
        /// Hàm thực hiện sửa đơn vị
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<int> UpdateOrganization(Organization entity)
        {
            using (DatabaseConnector databaseConnector = new DatabaseConnector())
            {
                databaseConnector.MapParameterValueAndEntityProperty("Proc_UpdateOrganization", entity);
                var value = databaseConnector.ExecuteNonQuery();
                databaseConnector.CommitTransaction();
                return value;
            }
        }

    }
}
