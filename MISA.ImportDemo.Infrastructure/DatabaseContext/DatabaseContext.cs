using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MISA.ImportDemo.Infrastructure.DatabaseContext
{
    /// <summary>
    /// Class thực hiện đóng/ mở kết nối tới CSDL
    /// CreateBy: NVMANH (14/04/2020)
    /// </summary>
    public class DatabaseContext: IDatabaseContext
    {
        public static string ConnectionString;
        private MySqlConnection _myconnection;

        /// <summary>
        /// Thực hiện khởi tạo và mở kết nối
        /// </summary>
        public MySqlConnection Connection
        {
            get
            {
                if (_myconnection == null)
                {
                    _myconnection = new MySqlConnection(ConnectionString);
                }
                if (_myconnection.State != ConnectionState.Open)
                {
                    _myconnection.Open();
                }
                return _myconnection;
            }
        }

        /// <summary>
        /// Tự đóng kết nối khi giải phóng đối tượng
        /// </summary>
        public void Dispose()
        {
            if (_myconnection != null && _myconnection.State == ConnectionState.Open)
                _myconnection.Close();
        }
    }
}
