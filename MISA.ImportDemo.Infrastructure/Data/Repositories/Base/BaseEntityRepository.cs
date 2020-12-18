using MISA.ImportDemo.Core.Entities;
using MISA.ImportDemo.Core.Enumeration;
using MISA.ImportDemo.Core.Interfaces.Base;
using MISA.Infrastructure;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Infrastructure.Data.Repositories
{
    public class BaseEntityRepository : IEntityRepository
    {
        protected Organization _organization = CommonUtility.GetCurrentOrganization();
        #region GET_DATA
        /// <summary>
        /// Lấy toàn bộ danh sách các bản ghi của bảng có trong Database
        /// </summary>
        /// <returns></returns>
        /// CreateBy: NVMANH (20/04/2020)
        public async Task<IReadOnlyList<T>> GetAllEntities<T>()
        {
            var storeName = DatabaseUtility.GeneateStoreName<T>(ProcdureTypeName.Get);
            var entities = await Task.FromResult(GetData<T>(storeName, null).ToList());
            return entities;
        }

        /// <summary>
        /// Lấy thông tin đối tượng theo khóa chính
        /// </summary>
        /// <param name="id">Id của bản ghi</param>
        /// <returns></returns>
        /// CreateBy: NVMANH (20/04/2020)
        public async Task<T> GetEntityByIdAsync<T>(object id)
        {
            var storeName = DatabaseUtility.GeneateStoreName<T>(ProcdureTypeName.GetById);
            var entities = GetData<T>(storeName, new object[] { id });
            return await Task.FromResult(entities.ToList().FirstOrDefault());
        }

        /// <summary>
        /// Lấy danh sách các đối tượng theo một bộ tham số truyền vào
        /// </summary>
        /// <returns></returns>
        /// CreateBy: NVMANH (20/04/2020)
        public async Task<IReadOnlyList<T>> GetEntities<T>()
        {
            var storeName = DatabaseUtility.GeneateStoreName<T>(ProcdureTypeName.Get);
            var entities = await Task.FromResult(GetData<T>(storeName, null).ToList());
            return entities;
        }
        /// <summary>
        /// Lấy danh sách các đối tượng theo một bộ tham số truyền vào
        /// </summary>
        /// <param name="parameters">một mảng chứa các tham số sẽ truyền vào store</param>
        /// <returns></returns>
        /// CreateBy: NVMANH (20/04/2020)
        public async Task<IReadOnlyList<T>> GetEntities<T>(object[] parameters)
        {
            var storeName = DatabaseUtility.GeneateStoreName<T>(ProcdureTypeName.Get);
            var entities = await Task.FromResult(GetData<T>(storeName, parameters).ToList());
            return entities;
        }

        /// <summary>
        /// Lấy danh sách các đối tượng theo một bộ tham số truyền vào
        /// </summary>
        /// <param name="procedureName">Tên thủ thục viết trên database</param>
        /// <param name="parameters">một mảng chứa các tham số sẽ truyền vào store</param>
        /// <returns></returns>
        /// CreateBy: NVMANH (20/04/2020)
        public async Task<IReadOnlyList<T>> GetEntities<T>(string procedureName, object[] parameters)
        {
            var entities = await Task.FromResult(GetData<T>(procedureName, parameters).ToList());
            return entities;
        }

        public Task<IEnumerable<T>> GetEntitiesPaging<T>()
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
        public async Task<object> AddAsync<T>(T entity, bool returnValue)
        {
            if (returnValue)
                return await InsertAndReturnSingleValue(entity);
            return await InsertAndReturnNumberRecordEffect(entity);
        }
        /// <summary>
        /// Thêm mới dữ liệu theo store mặc định tự sinh
        /// </summary>
        /// <param name="entity">Đối tượng sẽ thêm mới vào Database</param>
        /// <returns>Số bản ghi thêm mới thành công</returns>
        /// CreateBy: NVMANH (21/04/2020)
        protected virtual async Task<object> InsertAndReturnNumberRecordEffect<T>(T entity)
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
        protected virtual async Task<object> InsertAndReturnSingleValue<T>(T entity)
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
        protected virtual async Task<object> InsertAndReturnSingleValue<T>(string procedureName, T entity)
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
        public virtual Task<int> Update<T>(string procedureName, T entity)
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
        public virtual Task<int> Update<T>(T entity)
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
        public Task<int> Delete<T>(object[] param)
        {
            using (DatabaseConnector databaseConnector = new DatabaseConnector())
            {
                var procedureName = DatabaseUtility.GeneateStoreName<T>(ProcdureTypeName.Delete);
                databaseConnector.SetParameterValue(procedureName, param);
                databaseConnector.CommitTransaction();
                return databaseConnector.ExecuteNonQuery();
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
        protected virtual IEnumerable<T> GetData<T>(string procedureName, object[] parameters)
        {
            using (DatabaseConnector databaseConnector = new DatabaseConnector())
            {
                if (parameters != null && parameters.Length > 0)
                    databaseConnector.SetParameterValue(procedureName, parameters);
                MySqlDataReader mySqlDataReader = databaseConnector.ExecuteReader(procedureName);
                var organizationId = _organization.OrganizationId;
                while (mySqlDataReader.Read())
                {
                    var entity = Activator.CreateInstance<T>();
                    var hasOrganizationMatch = false;
                    for (int i = 0; i < mySqlDataReader.FieldCount; i++)
                    {
                        string fieldName = mySqlDataReader.GetName(i);
                        PropertyInfo property = entity.GetType().GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                        var fieldValue = mySqlDataReader[fieldName];
                        if (property != null && fieldValue != DBNull.Value)
                        {
                            if (property.Name.ToLower() == "organizationid" && organizationId != null && fieldValue.ToString() == organizationId.ToString())
                            {
                                hasOrganizationMatch = true;
                                property.SetValue(entity, fieldValue, null);
                            }
                            else if(property.Name.ToLower() == "organizationid" && organizationId != null && fieldValue.ToString() != organizationId.ToString())
                                break;
                            else
                                property.SetValue(entity, fieldValue, null);
                        }
                    }
                    // Nếu thông tin đơn vị không trùng với đơn vị hiện tại thì next --->
                    if (!hasOrganizationMatch)
                        continue;
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
        private Task<int> SaveChangeAndReturnRecordEffect<T>(DatabaseConnector databaseConnector, string procedureName, T entity)
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
        private Task<object> SaveChangeAndReturnSingleValue<T>(DatabaseConnector databaseConnector, string procedureName, T entity)
        {
            databaseConnector.MapParameterValueAndEntityProperty<T>(procedureName, entity);
            return databaseConnector.ExecuteScalar();
        }
        #endregion

        #region EXTEND_METHOD

        #endregion
    }
}
