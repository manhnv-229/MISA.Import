using Microsoft.AspNetCore.Mvc;
using MISA.ImportDemo.Interfaces;
using MISA.ImportDemo.Core.Entities;
using MISA.ImportDemo.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MISA.ImportDemo.Core.Services;

namespace MISA.ImportDemo.Services
{
    /// <summary>
    /// Service cơ quan bảo hiểm xã hội
    /// </summary>
    /// CreatedBy: NVMANH (20/04/2020)
    public class OrganizationService : BaseEntityService<Organization>, IOrganizationService
    {
        private readonly IOrganizationRepository _organizationRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="organizationRepository"></param>
        public OrganizationService(IOrganizationRepository organizationRepository):base(organizationRepository)
        {
            _organizationRepository = organizationRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="organizationCode"></param>
        /// <returns></returns>
        public Organization GetOrganizationByOrganizationCode(string organizationCode)
        {
             return _organizationRepository.GetOrganizationByOrganizationCode(organizationCode).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<object> InsertOrganizationAndReturnSingleValue(Organization entity)
        {
            var organizationDuplicate = GetOrganizationByOrganizationCode(entity.OrganizationCode);
            if(organizationDuplicate != null)
            {
                return Task.FromResult<object>(false);
            }
            else
            {
                return _organizationRepository.InsertOrganizationAndReturnSingleValue(entity);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<int> UpdateOrganization(Organization entity)
        {
            var organizationDuplicate = GetOrganizationByOrganizationCode(entity.OrganizationCode);
            if (organizationDuplicate != null && entity.OrganizationId == organizationDuplicate.OrganizationId)
            {
                return _organizationRepository.UpdateOrganization(entity);
            }
            else
            {
                return Task.FromResult(0);
            }
        }
    }
}
