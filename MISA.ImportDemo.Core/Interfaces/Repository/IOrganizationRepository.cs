using MISA.ImportDemo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Core.Interfaces
{
    /// <summary>
    /// interface quản lý nghiệp vụ phần đơn vị
    /// </summary>
    public interface IOrganizationRepository : IBaseRepository<Organization>
    {
        /// <summary>
        /// Hàm thực hiện lấy đơn vị theo mã code
        /// </summary>
        /// <param name="organizationCode"></param>
        /// <returns></returns>
        IEnumerable<Organization> GetOrganizationByOrganizationCode(string organizationCode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<object> InsertOrganizationAndReturnSingleValue (Organization entity);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> UpdateOrganization(Organization entity);
    }
}
