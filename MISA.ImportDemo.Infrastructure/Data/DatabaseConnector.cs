using MISA.ImportDemo.Infrastructure.UnitOfWork;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Infrastructure.Data
{
    /// <summary>
    /// Clas thực hiện khởi tạo context với database
    /// </summary>
    /// CreateBy: NVMANH (20/04/2020)
    public class DatabaseConnector : IDisposable
    {
        protected readonly IUnitOfWork _unitOfWork;
        MySqlConnection _sqlConnection;
        MySqlCommand _sqlCommand;
        public DatabaseConnector()
        {
            _unitOfWork = new UnitOfWork.UnitOfWork();
            _sqlConnection = _unitOfWork.DataContext.Connection;
            _sqlCommand = _sqlConnection.CreateCommand();
            _sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            _sqlCommand.Transaction = _unitOfWork.BeginTransaction();
        }

        public void SetCommandText(string commandText)
        {
            _sqlCommand.CommandText = commandText;
        }

        public void SetCommandType(System.Data.CommandType commandType)
        {
            _sqlCommand.CommandType = commandType;
        }

        /// <summary>
        /// Lấy toàn bộ tham số đầu vào được khai báo trong store
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        /// Created By : NVMANH (20/10/2019)
        public MySqlParameterCollection GetParamFromStore(string commandText)
        {
            _sqlCommand.CommandText = commandText;
            MySqlCommandBuilder.DeriveParameters(_sqlCommand);
            return _sqlCommand.Parameters;
        }

        /// <summary>
        /// Set các giá trị tương ứng với đầu vào của store
        /// </summary>
        /// <param name="procedureName">tên store</param>
        /// <param name="parameters">Bộ tham số sẽ truyền vào store - theo thứ tự</param>
        /// Created By : NVMANH (20/10/2019)
        public void SetParameterValue(string procedureName, object[] parameters)
        {
            var paramsFromStore = GetParamFromStore(procedureName);
            if (paramsFromStore.Count >= 1 && parameters != null)
            {
                for (int i = 0; i < paramsFromStore.Count; i++)
                {
                    if (i <= parameters.Length)
                    {
                        var value = string.Empty;
                        if (parameters[i] != null)
                            value = parameters[i].ToString();
                        var result = DatabaseUtility.ConvertType(value, paramsFromStore[i].MySqlDbType);
                        paramsFromStore[i].Value = result;
                    }
                }
            }
        }

        /// <summary>
        /// Thực hiện map value của tham số đầu vào tương ứng với value proprty của entity
        /// </summary>
        /// <typeparam name="T">entity generic</typeparam>
        /// <param name="procedureName">Tên store</param>
        /// <param name="entity">entity cụ thể</param>
        /// CreatedBy: NVMANH (20/04/2020)
        public void MapParameterValueAndEntityProperty<T>(string procedureName, T entity)
        {
            var paramsFromStore = GetParamFromStore(procedureName);
            //bool isPostMethod = ((MethodType) procedureName.GetType().GetProperty("MethodType").GetValue(entity) == MethodType.POST);
            if (paramsFromStore.Count >= 1 && paramsFromStore != null)
            {
                foreach (MySqlParameter parameter in paramsFromStore)
                {
                    var paramName = parameter.ParameterName.Replace("@", string.Empty);

                    //Lấy property của đối tượng (không phân biệt ký tự hoa/thường):
                    var property = entity.GetType().GetProperty(paramName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

                    // Nếu là phương thức thêm mới đối tượng thì làm một số việc - sinh mới giá trị cho khóa chính:
                    //if (isPostMethod)
                    //{
                    //    // Nếu property là khóa chính và không có value thì thực hiện gán value cho đối tượng:
                    //    var attribute = Attribute.GetCustomAttribute(property, typeof(KeyAttribute)) as KeyAttribute;
                    //    if (attribute != null) // This property has a KeyAttribute
                    //    {
                    //        // Kiểu dữ liệu là guid thì gán cho guid:
                    //        if (property.PropertyType == typeof(Guid?) || property.PropertyType == typeof(Guid))
                    //        {
                    //            property.SetValue(Guid.NewGuid(), entity);
                    //        }
                    //    }
                    //}

                    if (property != null)
                    {
                        var paramValue = property.GetValue(entity);
                        parameter.Value = paramValue != null ? paramValue : DBNull.Value;
                    }
                    else
                    {
                        parameter.Value = DBNull.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Build dữ liệu trả về từ MySqlDataReader
        /// </summary>
        /// <param name="databaseConnector">MySqlDataReader</param>
        /// <param name="procedureName">Tên procedure</param>
        /// <param name="parameters">bộ tham số</param>
        /// <returns></returns>
        /// CreateBy: NVMANH (20/04/2020)
        public IEnumerable<T> GetData<T>(string procedureName, object[] parameters)
        {
            if (parameters != null && parameters.Length > 0)
                SetParameterValue(procedureName, parameters);
            MySqlDataReader mySqlDataReader = ExecuteReader(procedureName);
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

        /// <summary>
        /// Thực thi store procedure đọc dữ liệu
        /// </summary>
        /// <param name="commandText">Tên store</param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (20/04/2020)
        public MySqlDataReader ExecuteReader(string commandText)
        {
            _sqlCommand.CommandText = commandText;
            return _sqlCommand.ExecuteReader();
        }


        /// <summary>
        /// Lưu lại các thay đổi (thêm mới hoặc sửa)
        /// </summary>
        /// <param name="databaseConnector"></param>
        /// <param name="procedureName"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<int> SaveChangeAndReturnRecordEffect<T>(string procedureName, T entity)
        {
            MapParameterValueAndEntityProperty<T>(procedureName, entity);
            return ExecuteNonQuery();
        }


        /// <summary>
        /// Thực thi store procedure thêm mới/ sửa/ xóa dữ liệu trả về số dòng bị ảnh hưởng
        /// </summary>
        /// <param name="commandText">Tên store</param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (20/04/2020)
        public async Task<int> ExecuteNonQuery()
        {
            var result = await Task.FromResult(_sqlCommand.ExecuteNonQuery());
            //_unitOfWork.Commit();
            return result;
        }

        /// <summary>
        /// Thực thi store procedure thêm mới dữ liệu trả về 1 value
        /// </summary>
        /// <param name="commandText">Tên store</param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (20/04/2020)
        public async Task<object> ExecuteScalar()
        {
            var result = await Task.FromResult(_sqlCommand.ExecuteScalar());
            //_unitOfWork.Commit();
            return result;
        }


        /// <summary>
        /// Commit transaction
        /// </summary>
        public void CommitTransaction()
        {
            _unitOfWork.Commit();
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }

        public void RollBack()
        {
            _unitOfWork.RollBack();
        }


    }
}
