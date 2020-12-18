using Microsoft.Extensions.Caching.Memory;
using MISA.ImportDemo.Core.Entities;
using MISA.ImportDemo.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace MISA.Infrastructure
{
    /// <summary>
    /// Class tiện ích chung
    /// </summary>
    public static class CommonUtility
    {
        public static HttpContext httpContext;

        /// <summary>
        /// Lấy thông tin đơn vị theo khóa chính bảng đơn vị
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (20/06/2020)
        public static Organization GetOrganizationByOrganizationId(Guid orgId)
        {
            using EfDbContext dbContex = new EfDbContext();
            return dbContex.Organization.Where(o => o.OrganizationId == orgId).FirstOrDefault();
        }

        /// <summary>
        /// Lấy thông tin đơn vị được lưu trong cache
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (20/06/2020)
        //TODO: Thông tin đơn vị cho các đối tượng (NVMANH)
        public static Organization GetCurrentOrganization()
        {
            byte[] orgId;
            httpContext.Session.TryGetValue("OrganizationId", out orgId);
            if (orgId != null)
            {
                using EfDbContext dbContex = new EfDbContext();
                Guid organizationId = Guid.Empty;
                Guid.TryParse(System.Text.Encoding.UTF8.GetString(orgId), out organizationId);
                if (organizationId == Guid.Empty)
                    return null;
                return dbContex.Organization.Where(o => o.OrganizationId == organizationId).FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// Lấy mã thông tin đơn vị được lưu trong cache
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (20/06/2020)
        public static Guid? GetCurrentOrganizationId()
        {
            var org = GetCurrentOrganization();
            if (org == null)
                return null;
            return org.OrganizationId;
        }
    }
}
