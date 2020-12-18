using MISA.ImportDemo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Core.Interfaces.Repository
{
    /// <summary>
    /// Interface khai báo các hàm cung cấp cho việc nhập khẩu nhân viên
    /// </summary>
    /// CreatedBy: NVMANH (06/06/2020)
    public interface IImportEmployeeRepository : IBaseImportRepository
    {
        /// <summary>
        /// Lấy toàn bộ danh sách nhân viên có trong Db
        /// </summary>
        /// <returns>List Nhân viên</returns>
        /// CreatedBy: NVMANH (06/06/2020)
        Task<List<Employee>> GetEmployees();
    }
}
