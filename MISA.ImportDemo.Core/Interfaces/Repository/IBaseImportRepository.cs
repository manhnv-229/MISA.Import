using MISA.ImportDemo.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Core.Interfaces.Repository
{
    public interface IBaseImportRepository:IBaseRepository<ImportFileTemplate>
    {
        /// <summary>
        /// Lấy thông tin File mẫu nhập khẩu được khai báo trong Database
        /// </summary>
        /// <param name="spec">Các tiêu chí lấy File mẫu nhập khẩu (Thông thường sẽ lấy theo tên bảng sẽ import dữ liệu vào)</param>
        /// <returns>Thông tin file mẫu nhập khẩu</returns>
        /// CreatedBy: NVMANH (01/06/2020)
        Task<ImportFileTemplate> GetFileImportInfo(ISpecification<ImportFileTemplate> spec);

        Task<ActionServiceResult> Import(string importKey, bool overriderData, CancellationToken cancellationToken);
        ///// <summary>
        ///// Thực hiện nhập khẩu
        ///// </summary>
        ///// <param name="entities"></param>
        ///// <returns></returns>
        //Task Import(List<object> entities);

        /// <summary>
        /// Lấy cache theo key
        /// </summary>
        /// <param name="key">Khóa cache</param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (01/06/2020)
        object CacheGet(string key);

        /// <summary>
        /// Lưu cache
        /// </summary>
        /// <param name="key">Khóa cache</param>
        /// <param name="data">Dữ liệu muốn cache</param>
        /// CreatedBy: NVMANH (01/06/2020)
        void SetCache(string key, object data);

        Task<List<object>> GetListObjectByTableName(string tableName);

        Organization GetCurrentOrganization();
    }
}
