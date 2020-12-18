using MISA.ImportDemo.Core.Enumeration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MISA.ImportDemo.Infrastructure.Data
{
    public class DatabaseUtility
    {
        /// <summary>
        /// Sinh tên store theo tên bảng và kiểu thao tác dữ liệu của store
        /// </summary>
        /// <typeparam name="T">Entity truyền vào</typeparam>
        /// <param name="procdureTypeName">Kiểu thực thi của store (Thêm, sửa, xóa)</param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (14/04/2020)
        public static string GeneateStoreName<T>(ProcdureTypeName procdureTypeName)
        {
            string storeName = string.Empty;
            var tableName = (Activator.CreateInstance<T>()).GetType().Name;
            switch (procdureTypeName)
            {
                case ProcdureTypeName.Get:
                    storeName = $"Proc_Get{tableName}s";
                    break;
                case ProcdureTypeName.GetById:
                    storeName = $"Proc_Get{tableName}ByID";
                    break;
                case ProcdureTypeName.Insert:
                    storeName = $"Proc_Insert{tableName}";
                    break;
                case ProcdureTypeName.Update:
                    storeName = $"Proc_Update{tableName}";
                    break;
                case ProcdureTypeName.Delete:
                    storeName = $"Proc_Delete{tableName}";
                    break;
                case ProcdureTypeName.GetPaging:
                    storeName = $"Proc_Get{tableName}sPaging";
                    break;
                default:
                    break;
            }
            return storeName;
        }

        /// <summary>
        /// Hàm thực hiện việc convert kiểu dữ liệu từ MySqlDbType sang kiểu dữ liệu tương ứng trên c#
        /// </summary>
        /// <param name="value">giá trị</param>
        /// <param name="sqlDbType">Kiểu dữ liệu trên MariaDb (MySqlDbType)</param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (15/04/2020)
        public static object ConvertType(string value, MySqlDbType sqlDbType)
        {
            object result;
            Type type = GetClrType(sqlDbType);

            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            try
            {
                var converter = TypeDescriptor.GetConverter(type);
                result = converter.ConvertFromString(value);

            }
            catch (Exception exception)
            {
                // Log this exception if required.
                throw new InvalidCastException(string.Format("Unable to cast the {0} to type {1}", value, "newType", exception));
            }
            return result;
        }

        /// <summary>
        /// Láy về kiểu dữ liệu tương ứng trên c#
        /// </summary>
        /// <param name="sqlType">MySqlDbType</param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (15/04/2020)
        private static Type GetClrType(MySqlDbType sqlType)
        {
            switch (sqlType)
            {
                case MySqlDbType.UInt16:
                case MySqlDbType.UInt24:
                case MySqlDbType.UInt32:
                case MySqlDbType.UInt64:
                    return typeof(long?);

                case MySqlDbType.Binary:
                //case MySqlDbType.Image:
                case MySqlDbType.Timestamp:
                case MySqlDbType.VarBinary:
                    return typeof(byte[]);

                case MySqlDbType.Bit:
                    return typeof(bool?);

                case MySqlDbType.VarChar:
                case MySqlDbType.LongText:
                case MySqlDbType.String:
                case MySqlDbType.Text:
                    return typeof(string);

                case MySqlDbType.DateTime:
                case MySqlDbType.Date:
                case MySqlDbType.Time:
                    return typeof(DateTime?);

                case MySqlDbType.Decimal:
                    return typeof(decimal?);

                case MySqlDbType.Float:
                    return typeof(double?);

                case MySqlDbType.Int32:
                case MySqlDbType.Int64:
                    return typeof(int?);

                case MySqlDbType.Int16:
                    return typeof(short?);
                case MySqlDbType.Guid:
                    return typeof(Guid);
                default:
                    throw new ArgumentOutOfRangeException("sqlType");
            }
        }

    }
}
