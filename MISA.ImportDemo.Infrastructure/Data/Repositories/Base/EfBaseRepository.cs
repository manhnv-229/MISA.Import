using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MISA.ImportDemo.Core.Entities;
using MISA.ImportDemo.Core.Interfaces;
using MISA.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Infrastructure.Data.Repositories
{
    /// <summary>
    /// DB context tương tác cơ sở dữ liệu sử dụng Entity Framework Core
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// CreatedBy: NVMANH (05/2020)
    /// EditBy: NVMANH (03/06/2020) - xử lý lỗi connection - thay đổi cơ chế, mỗi lượt tương tác với DbContext sẽ using và tự động Disponse tránh lỗi many connection
    public class EfBaseRepository<T> : BaseEntityRepository, IBaseRepository<T> where T : BaseEntity, IAggregateRoot
    {
        #region DECLARE
        //protected EfDbContext _dbContext;
        //protected DbSet<T> dbSet;
        
        #endregion

        #region CONSTRUCTOR
        public EfBaseRepository()
        {
            //_dbContext = new EfDbContext();
            //dbSet = _dbContext.Set<T>();
        }
        #endregion

        #region METHOD
        #region GETDATA
        /// <summary>
        /// Lấy dữ liệu trong bảng theo nhiều them số đầu vào sử dụng ADO.NET
        /// (Cần tạo Store trước - tên store có dạng Proc_Get[TableName]s)
        /// </summary>
        /// <param name="parameters">mảng các tham số theo thứ tự được viết trong store</param>
        /// <returns>List dữ liệu</returns>
        /// CreatedBy: NVMANH (05/2020)
        public virtual async Task<IReadOnlyList<T>> GetListAsync(object[] parameters)
        {
            return await GetEntities<T>(parameters);
        }

        /// <summary>
        /// Lấy dữ liệu từ store được viết sẵn
        /// </summary>
        /// <param name="procedureName">Tên store viết trong database</param>
        /// <param name="parameters">Mảng các tham số truyền vào (theo thứ tự tương ứng trong store)</param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (05/2020)
        public virtual async Task<IReadOnlyList<T>> GetListAsync(string procedureName, object[] parameters)
        {
            return await GetEntities<T>(procedureName, parameters);
        }

        /// <summary>
        /// Lấy toàn bộ dữ liệu sử dụng EF core
        /// </summary>
        /// <returns>List toàn bộ dữ liệu trong bảng</returns>
        /// CreatedBy: NVMANH (05/2020)
        public virtual async Task<IReadOnlyList<T>> GetListAsync()
        {
            using EfDbContext dbContex = new EfDbContext();
            var entities = await dbContex.Set<T>().ToListAsync();
            var firstEntity = entities.FirstOrDefault();
            var property = firstEntity.GetType().GetProperty("OrganizationId", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
            // Nếu có thông tin đơn vị thì lấy dữ liệu theo đơn vị:
            if (property != null && _organization != null)
                return entities.Where(e => Guid.Parse((e.GetType().GetProperty("OrganizationId", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public).GetValue(e) ?? Guid.Empty).ToString()) == _organization.OrganizationId).ToList();
            return entities;
        }

        //TODO:NVMANH-GetListPagedAsync
        public virtual Task<IEnumerable<T>> GetListPagedAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Lấy thông tin chi tiết của entity theo khóa chính
        /// </summary>
        /// <param name="id">Giá trị của khóa chính</param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (05/2020)
        public virtual async Task<T> GetEntityByIdAsync(object id)
        {
            using EfDbContext dbContex = new EfDbContext();
            var type = GetTypeOfPrimaryKey();
            var converter = TypeDescriptor.GetConverter(type);
            var idAfterConvert = converter.ConvertFromString(id.ToString());
            return await dbContex.Set<T>().FindAsync(idAfterConvert);
        }

        /// <summary>
        /// Lấy dữ liệu theo các tiêu chí đặt sẵn sử dụng EF core
        /// </summary>
        /// <returns>List dữ liệu trong bảng</returns>
        /// CreatedBy: NVMANH (05/2020
        //TODO:NVMANH
        public virtual async Task<IReadOnlyList<T>> GetListAsyncBySpecification(ISpecification<T> spec)
        {
            using var dbContext = new EfDbContext();
            var entities = await (SpecificationEvaluator<T>.GetQuery(dbContext.Set<T>().AsQueryable(), spec)).ToListAsync();
            var firstEntity = entities.FirstOrDefault();
            if (firstEntity == null)
                return entities;
            var property = firstEntity.GetType().GetProperty("OrganizationId", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
            // Nếu có thông tin đơn vị thì lấy dữ liệu theo đơn vị:
            if (property != null && _organization != null)
                return entities.Where(e => Guid.Parse((e.GetType().GetProperty("OrganizationId", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public).GetValue(e) ?? Guid.Empty).ToString()) == _organization.OrganizationId).ToList();
            return entities;
        }

        /// <summary>
        /// Lấy dữ liệu theo các tiêu chí đặt sẵn sử dụng EF core
        /// </summary>
        /// <returns>List dữ liệu trong bảng</returns>
        /// CreatedBy: NVMANH (05/2020
        //TODO:NVMANH
        public virtual async Task<IReadOnlyList<Y>> GetListAsyncBySpecification<Y>(ISpecification<Y> spec) where Y: BaseEntity
        {
            using var dbContext = new EfDbContext();
            var entities = await (SpecificationEvaluator<Y>.GetQuery(dbContext.Set<Y>().AsQueryable(), spec)).ToListAsync();
            var firstEntity = entities.FirstOrDefault();
            if (firstEntity == null)
                return entities;
            var property = firstEntity.GetType().GetProperty("OrganizationId", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
            // Nếu có thông tin đơn vị thì lấy dữ liệu theo đơn vị:
            if (property != null && _organization != null)
                return entities.Where(e => Guid.Parse((e.GetType().GetProperty("OrganizationId", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public).GetValue(e) ?? Guid.Empty).ToString()) == _organization.OrganizationId).ToList();
            return entities;
        }

        //TODO:NVMANH
        public virtual async Task<int> CountAsync(ISpecification<T> spec)
        {
            using var dbContext = new EfDbContext();
            var data = SpecificationEvaluator<T>.GetQuery(dbContext.Set<T>().AsQueryable(), spec);
            return await data.CountAsync();
        }

        #endregion

        #region "INSERT - UPDATE - DELETE"

        /// <summary>
        /// thêm mới dữ liệu sử dụng EF Core
        /// </summary>
        /// <param name="entity">Đối tượng thêm mới</param>
        /// <returns>Trả về đối tượng vừa được thêm mới</returns>
        /// CreatedBy: NVMANH (05/2020)
        public virtual async Task<T> AddAsync(T entity)
        {
            using var dbContext = new EfDbContext();
            DoBeforeInsert(entity);
            await dbContext.Set<T>().AddAsync(entity);
            await SaveChangesAsync();
            DoAfterSave(entity);
            return entity;
        }

        /// <summary>
        /// Thêm một List các object vào database
        /// </summary>
        /// <param name="entities">List đối tượng</param>
        /// <returns></returns>
        /// CreatedBy NVMANH (20/05/2020)
        public virtual async Task AddRangeAsync(List<T> entities)
        {
            using var dbContext = new EfDbContext();
            await dbContext.Set<T>().AddRangeAsync(entities);
            await SaveChangesAsync();
        }


        /// <summary>
        /// Thêm mới dữ liệu sử dụng ADO.NET
        /// </summary>
        /// <param name="entity">Đối tượng thêm mới</param>
        /// <param name="returnValue">Trả về giá trị hay không (true: có, false: không)</param>
        /// <returns>Giá trị trả về tùy thuộc vào tham số return là true hay false- nếu false thì sẽ trả về só bản ghi được thêm mới</returns>
        /// CreatedBy: NVMANH (05/2020)
        public virtual async Task<object> AddAsync(T entity, bool returnValue = false)
        {
            object res;
            DoBeforeInsert(entity);
            if (returnValue == true)
                res = await InsertAndReturnSingleValue<T>(entity);
            else
                res = await InsertAndReturnNumberRecordEffect<T>(entity);
            DoAfterInsert(entity);
            return res;
        }

        /// <summary>
        /// Cập nhật dữ liệu cho đối tượng sử dụng EF Core
        /// </summary>
        /// <param name="entity">Đối tượng sẽ cập nhật</param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (05/2020)
        public virtual async Task UpdateAsync(T entity)
        {
            using var dbContext = new EfDbContext();
            DoBeforeSave(entity);
            dbContext.Entry(entity).State = EntityState.Modified;
            await SaveChangesAsync();
            DoAfterInsert(entity);
        }

        /// <summary>
        /// Save    
        /// </summary>
        /// <returns></returns>
        protected virtual async Task SaveChangesAsync()
        {
            using var dbContext = new EfDbContext();
            await dbContext.SaveChangesAsync();
        }
        /// <summary>
        /// Cập nhật dữ liệu sử dụng ADO.NET
        /// (Cần tạo trước store Update trong Database)
        /// </summary>
        /// <param name="entity">Đối tượng sẽ thực hiện cập nhật</param>
        /// <returns>Số bản ghi được cập nhật</returns>
        /// CreatedBy: NVMANH (04/2020)
        public virtual Task<int> Update(T entity)
        {
            DoBeforeSave(entity);
            var res = Update<T>(entity);
            DoAfterSave(entity);
            return res;
        }

        /// <summary>
        /// Xóa dữ liệu sử dụng EF Core
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>TRả về đối tượng đã xóa</returns>
        /// CreatedBy: NVMANH (04/2020)
        public virtual async Task DeleteAsync(T entity)
        {
            using var dbContext = new EfDbContext();
            dbContext.Set<T>().Remove(entity);
            await dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Xóa dữ liệu theo nhiều tham số đầu vào sử dụng ADO.NET.
        /// (Cần tạo trước store xóa dữ liệu)
        /// </summary>
        /// <param name="param">param</param>
        /// <returns></returns>
        /// CreatedBy: NVMANH
        public virtual Task<int> Delete(object[] param)
        {
            var res = Delete<T>(param);
            return res;
        }
        #endregion

        #endregion

        #region UTINITY
        protected virtual void DoBeforeInsert(T Entity)
        {

        }
        protected virtual void DoAfterInsert(T Entity)
        {

        }
        protected virtual void DoBeforeSave(T Entity)
        {

        }
        protected virtual void DoAfterSave(T Entity)
        {

        }

        /// <summary>
        /// Lấy kiểu dữ liệu của khóa chính
        /// </summary>
        /// <returns>Trả về kiểu dữ liệu (ClrType)</returns>
        /// CreatedBy: NVMANH (05/2020)
        public virtual Type GetTypeOfPrimaryKey()
        {
            using var dbContext = new EfDbContext();
            var keyType = dbContext.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties
                .Select(x => x.ClrType).Single();
            return keyType;
        }
        #endregion

    }
}
