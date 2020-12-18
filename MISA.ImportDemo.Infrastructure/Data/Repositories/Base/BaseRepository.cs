using MISA.ImportDemo.Core.Entities;
using MISA.ImportDemo.Core.Interfaces;
using MISA.ImportDemo.Infrastructure.UnitOfWork;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using MISA.ImportDemo.Core.Enumeration;

namespace MISA.ImportDemo.Infrastructure.Data.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity, IAggregateRoot
    {
        #region GET_DATA
        /// <summary>
        /// Lấy toàn bộ danh sách các bản ghi của bảng có trong Database
        /// </summary>
        /// <returns></returns>
        /// CreateBy: NVMANH (20/04/2020)
        public async Task<IReadOnlyList<T>> GetListAsync()
        {
            var storeName = DatabaseUtility.GeneateStoreName<T>(ProcdureTypeName.Get);
            return await Task.FromResult(GetData(storeName, null).ToList());
        }

        /// <summary>
        /// Lấy thông tin đối tượng theo khóa chính
        /// </summary>
        /// <param name="id">Id của bản ghi</param>
        /// <returns></returns>
        /// CreateBy: NVMANH (20/04/2020)
        public async Task<T> GetEntityByIdAsync(object id)
        {
            var storeName = DatabaseUtility.GeneateStoreName<T>(ProcdureTypeName.GetById);
            var entities = GetData(storeName, new object[] { id });
            return await Task.FromResult(entities.ToList().FirstOrDefault());
        }

        /// <summary>
        /// Lấy danh sách các đối tượng theo một bộ tham số truyền vào
        /// </summary>
        /// <returns></returns>
        /// CreateBy: NVMANH (20/04/2020)
        public async Task<IReadOnlyList<T>> GetEntities()
        {
            var storeName = DatabaseUtility.GeneateStoreName<T>(ProcdureTypeName.Get);
            return await Task.FromResult(GetData(storeName, null).ToList());
        }
        /// <summary>
        /// Lấy danh sách các đối tượng theo một bộ tham số truyền vào
        /// </summary>
        /// <param name="parameters">một mảng chứa các tham số sẽ truyền vào store</param>
        /// <returns></returns>
        /// CreateBy: NVMANH (20/04/2020)
        public async Task<IReadOnlyList<T>> GetListAsync(object[] parameters)
        {
            var storeName = DatabaseUtility.GeneateStoreName<T>(ProcdureTypeName.Get);
            return await Task.FromResult(GetData(storeName, parameters).ToList());
        }

        /// <summary>
        /// Lấy danh sách các đối tượng theo một bộ tham số truyền vào
        /// </summary>
        /// <param name="procedureName">Tên thủ thục viết trên database</param>
        /// <param name="parameters">một mảng chứa các tham số sẽ truyền vào store</param>
        /// <returns></returns>
        /// CreateBy: NVMANH (20/04/2020)
        public IEnumerable<T> GetEntities(string procedureName, object[] parameters)
        {
                return GetData(procedureName, parameters);
        }

        public Task<IEnumerable<T>> GetListPagedAsync()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region INSERT_DATA
        /// <summary>
        /// Thêm mới dữ liệu vào database
        /// </summary>
        /// <param name="entity">đối tượng thêm mới</param>
        /// <param name="returnValue">Có trả về dữ liệu hay không?</param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (21/04/2020)
        public async Task<object> AddAsync(T entity, bool returnValue)
        {
            if (returnValue)
                return await InsertAndReturnSingleValue(entity);
            return await InsertAndReturnNumberRecordEffect(entity);
        }

        /// <summary>
        /// Thêm mới 1 List Object (sử dụng ADO.NET_
        /// </summary>
        /// <param name="entities">Danh sách các object</param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (22/05/2020)
        public async Task AddRangeAsync(List<T> entities)
        {
            using (DatabaseConnector databaseConnector = new DatabaseConnector())
            {
                var storeName = DatabaseUtility.GeneateStoreName<T>(ProcdureTypeName.Insert);
                foreach (var entity in entities)
                {
                    await databaseConnector.SaveChangeAndReturnRecordEffect(storeName, entity);
                }
                databaseConnector.CommitTransaction();
            }
        }

        /// <summary>
        /// Thêm mới dữ liệu theo store mặc định tự sinh
        /// </summary>
        /// <param name="entity">Đối tượng sẽ thêm mới vào Database</param>
        /// <returns>Số bản ghi thêm mới thành công</returns>
        /// CreateBy: NVMANH (21/04/2020)
        protected virtual async Task<object> InsertAndReturnNumberRecordEffect(T entity)
        {
            using (DatabaseConnector databaseConnector = new DatabaseConnector())
            {
                var storeName = DatabaseUtility.GeneateStoreName<T>(ProcdureTypeName.Insert);
                var result = await SaveChangeAndReturnRecordEffect(databaseConnector, storeName, entity);
                databaseConnector.CommitTransaction();
                return result;
            }
        }

        /// <summary>
        /// Thêm mới dữ liệu theo store mặc định tự sinh
        /// </summary>
        /// <param name="entity">Đối tượng sẽ thêm mới vào Database</param>
        /// <returns>Số bản ghi thêm mới thành công</returns>
        /// CreateBy: NVMANH (21/04/2020)
        protected virtual async Task<object> InsertAndReturnSingleValue(T entity)
        {
            var storeName = DatabaseUtility.GeneateStoreName<T>(ProcdureTypeName.Insert);
            return await InsertAndReturnSingleValue(storeName, entity);
        }

        /// <summary>
        /// Thêm mới dữ liệu
        /// </summary>
        /// <param name="procedureName">Tên store</param>
        /// <param name="entity">đối tượng sẽ thêm mới</param>
        /// <returns>số bản ghi thêm mới thành công</returns>
        /// CreatedBy:NVMANH (21.04.2020)
        protected virtual async Task<object> InsertAndReturnSingleValue(string procedureName, T entity)
        {
            using (DatabaseConnector databaseConnector = new DatabaseConnector())
            {
                var value = await SaveChangeAndReturnSingleValue(databaseConnector, procedureName, entity);
                databaseConnector.CommitTransaction();
                return value;
            }
        }

        #endregion

        #region UPDATE_DATA
        /// <summary>
        /// Sửa dữ liệu
        /// </summary>
        /// <param name="procedureName">Tên store</param>
        /// <param name="entity">Đối tượng thực hiện sửa</param>
        /// <returns>Kết quả sửa</returns>
        /// CreateBy: NVMANH (21/04/2020)
        public virtual Task<int> Update(string procedureName, T entity)
        {
            using (DatabaseConnector databaseConnector = new DatabaseConnector())
            {
                var value = SaveChangeAndReturnRecordEffect(databaseConnector, procedureName, entity);
                databaseConnector.CommitTransaction();
                return value;
            }
        }

        /// <summary>
        /// Sửa dữ liệu (tự sinh store sửa theo teamplate Update[Tên bảng]
        /// </summary>
        /// <param name="entity">Thực thể</param>
        /// <returns></returns>
        public virtual Task<int> Update(T entity)
        {
            using (DatabaseConnector databaseConnector = new DatabaseConnector())
            {
                var procedureName = DatabaseUtility.GeneateStoreName<T>(ProcdureTypeName.Update);
                var value = SaveChangeAndReturnRecordEffect(databaseConnector, procedureName, entity);
                databaseConnector.CommitTransaction();
                return value;
            }
        }
        #endregion

        #region DELETE_DATA
        /// <summary>
        /// Xóa dữ liệu theo các điều kiện trả về số bản ghi bị xóa trong DB
        /// </summary>
        /// <param name="param">Bộ tham số truyền vào cho store - map theo thứ tự</param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (21/02/2020)
        public Task<int> Delete(object[] param)
        {
            using (DatabaseConnector databaseConnector = new DatabaseConnector())
            {
                var procedureName = DatabaseUtility.GeneateStoreName<T>(ProcdureTypeName.Delete);
                databaseConnector.SetParameterValue(procedureName, param);
                var result = databaseConnector.ExecuteNonQuery();
                databaseConnector.CommitTransaction();
                return result;
            }

        }
        #endregion

        #region BASE_METHOD
        /// <summary>
        /// Build dữ liệu trả về từ MySqlDataReader
        /// </summary>
        /// <param name="databaseConnector">MySqlDataReader</param>
        /// <param name="procedureName">Tên procedure</param>
        /// <param name="parameters">bộ tham số</param>
        /// <returns></returns>
        /// CreateBy: NVMANH (20/04/2020)
        protected IEnumerable<T> GetData(string procedureName, object[] parameters)
        {
            using (DatabaseConnector databaseConnector = new DatabaseConnector())
            {
                if (parameters != null && parameters.Length > 0)
                    databaseConnector.SetParameterValue(procedureName, parameters);
                MySqlDataReader mySqlDataReader = databaseConnector.ExecuteReader(procedureName);
                while (mySqlDataReader.Read())
                {
                    var entity = Activator.CreateInstance<T>();
                    for (int i = 0; i < mySqlDataReader.FieldCount; i++)
                    {
                        string fieldName = mySqlDataReader.GetName(i);
                        PropertyInfo property = entity.GetType().GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                        var fieldValue = mySqlDataReader[fieldName];
                        if (property != null && fieldValue != DBNull.Value)
                        {
                            property.SetValue(entity, fieldValue, null);
                        }
                    }
                    yield return entity;
                }
            }
        }

        /// <summary>
        /// Lưu lại các thay đổi (thêm mới hoặc sửa)
        /// </summary>
        /// <param name="databaseConnector"></param>
        /// <param name="procedureName"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected Task<int> SaveChangeAndReturnRecordEffect(DatabaseConnector databaseConnector, string procedureName, T entity)
        {
            databaseConnector.MapParameterValueAndEntityProperty<T>(procedureName, entity);
            return databaseConnector.ExecuteNonQuery();
        }

        /// <summary>
        /// Lưu lại các thay đổi (thêm mới hoặc sửa)
        /// </summary>
        /// <param name="databaseConnector"></param>
        /// <param name="procedureName"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        private Task<object> SaveChangeAndReturnSingleValue(DatabaseConnector databaseConnector, string procedureName, T entity)
        {
            databaseConnector.MapParameterValueAndEntityProperty<T>(procedureName, entity);
            return databaseConnector.ExecuteScalar();
        }

        #endregion

        #region EXTEND_METHOD
        public virtual Task UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public virtual Task DeleteAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public virtual Task<int> CountAsync(ISpecification<T> spec)
        {
            throw new NotImplementedException();
        }

        public virtual Task<T> AddAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<T>> GetListAsync(string procedureName, object[] parameters)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<T>> GetListAsyncBySpecification(ISpecification<T> spec)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Y>> GetListAsyncBySpecification<Y>(ISpecification<Y> spec) where Y : BaseEntity
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
