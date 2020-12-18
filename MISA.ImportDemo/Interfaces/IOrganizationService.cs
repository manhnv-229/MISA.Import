using MISA.ImportDemo.Core.Entities;
using MISA.ImportDemo.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Interfaces
{
    /// <summary>
    /// Interface quản lý nghiệp vụ đơn vị
    /// </summary>
    /// CreatedBy: 
    public interface IOrganizationService:  IBaseEntityService<Organization>
    {
        /// <summary>
        /// Hàm thực hiện lấy thông tin đơn vị theo mã đơn vị
        /// </summary>
        /// <param name="organizationCode">mã đơn vị</param>
        /// <returns></returns>
        /// ntngoc - 25/04/2020
        Organization GetOrganizationByOrganizationCode(string organizationCode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<object> InsertOrganizationAndReturnSingleValue(Organization entity);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> UpdateOrganization(Organization entity);

    }
}
