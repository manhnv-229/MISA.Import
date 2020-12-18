using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MISA.ImportDemo.Core.Entities;
using MISA.ImportDemo.Core.Interfaces;
using MISA.ImportDemo.Core.Interfaces.Base;
using MISA.ImportDemo.Core.Interfaces.Repository;
using MISA.ImportDemo.Core.Specifications;
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
    /// <summary>
    /// Nhập khẩu
    /// </summary>
    /// CreatedBy: NVMANH (22/05/2020)
    public partial class BaseImportRepository : EfBaseRepository<ImportFileTemplate>, IBaseImportRepository
    {
        protected readonly IEntityRepository _entityRepository;
        protected readonly IMemoryCache _importMemoryCache;
        protected readonly List<Nationality> Nationalities;
        protected readonly List<Relation> Relations;
        protected readonly List<Ethnic> Ethnics;
        protected List<Position> Positions;
        protected List<Department> Departments;
        public BaseImportRepository(IEntityRepository entityRepository, IMemoryCache importMemoryCache)
        {
            _entityRepository = entityRepository;
            _importMemoryCache = importMemoryCache;

            using var dbContext = new EfDbContext();
            // Lấy dữ liệu:
            Nationalities = dbContext.Nationality.ToList();
            Relations = dbContext.Relation.ToList();
            Ethnics = dbContext.Ethnic.ToList();
            Positions = dbContext.Position.ToList().Where(e=>e.OrganizationId==_organization.OrganizationId).ToList();
            Departments = dbContext.Department.ToList().Where(e => e.OrganizationId == _organization.OrganizationId).ToList();
            // Cache lại:
            CacheGetOrCreate();
        }

        /// <summary>
        /// Lấy thông tin File nhập khẩu
        /// </summary>
        /// <param name="spec">Các tiêu chí lấy ra bản ghi</param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (06/05/2020)
        public async Task<ImportFileTemplate> GetFileImportInfo(ISpecification<ImportFileTemplate> spec)
        {
            using EfDbContext dbContex = new EfDbContext();
            var queryData = SpecificationEvaluator<ImportFileTemplate>.GetQuery(dbContex.Set<ImportFileTemplate>().AsQueryable(), spec);
            var importFileInfo = await queryData.Include(e => e.ImportWorksheet).ThenInclude(e => e.ImportColumn).ToListAsync();
            //var importFileInfo = (await ApplySpecification(spec).Include(e => e.ImportWorksheet).ThenInclude(e => e.ImportColumn).ToListAsync()).FirstOrDefault();
            return importFileInfo.FirstOrDefault();
        }

        /// <summary>
        /// Thực hiện cache dữ liệu các danh mục phục vụ cho import dữ liệu:
        /// </summary>
        /// CreatedBy: NVMANH (05/2020)
        public void CacheGetOrCreate()
        {
            SetCache("Nationalities", Nationalities);
            SetCache("Relations", Relations);
            SetCache("Ethnics", Ethnics);
            SetCache("Positions", Positions);
            SetCache("Departments", Departments);
        }

        /// <summary>
        /// Lấy cache dữ liệu Theo key
        /// </summary>
        /// CreatedBy: NVMANH (05/2020)
        public object CacheGet(string key)
        {
            var cacheEntry = _importMemoryCache.Get<object>(key);
            return cacheEntry;
        }

        /// <summary>
        ///  Thực hiện cache dữ liệu
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="data">Dữ liệu cache</param>
        /// CreatedBy: NVMANH (25/05/2020)
        public void SetCache(string key, object data)
        {
            _importMemoryCache.GetOrCreate(key, entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(3);
                return data;
            });
        }


        public async Task<List<object>> GetListObjectByTableName(string tableName)
        {
            List<object> list = (List<object>)CacheGet(String.Format("CacheImport_{0}", tableName));
            // Có trong cache thì trả về luôn:
            if (list != null)
                return list;

            using EfDbContext dbContext = new EfDbContext();
            Type type = Type.GetType(String.Format("MISA.ImportDemo.Core.Entities.{0},MISA.ImportDemo.Core", tableName));
            switch (tableName)
            {
                case "Department":
                    list = (await dbContext.Department.ToListAsync()).Cast<object>().ToList();
                    break;
                case "Ethic":
                    list = (await dbContext.Ethnic.ToListAsync()).Cast<object>().ToList();
                    break;
                case "Nationality":
                    list = (await dbContext.Nationality.ToListAsync()).Cast<object>().ToList();
                    break;
                default:
                    break;
            }

            // Thực hiện lưu vào cache:
            SetCache(String.Format("CacheImport_{0}", tableName), list);

            //if (type!=null && type.GetTypeInfo().IsClass)
            //{
            //    type.GetType().GetProperty().GetValue();
            //    var abc = dbContext.GetType().GetProperty(tableName);
            //}


            return list;
        }

        public Organization GetCurrentOrganization()
        {
            return CommonUtility.GetCurrentOrganization();
        }

        public virtual async Task<ActionServiceResult> Import(string importKey, bool overriderData, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new ActionServiceResult(true, "Thực hiện nhập khẩu", Core.Enumeration.MISACode.Success));
        }
    }
}
