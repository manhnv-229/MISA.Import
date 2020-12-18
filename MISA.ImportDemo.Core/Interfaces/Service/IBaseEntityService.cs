using MISA.ImportDemo.Core.Entities;
using MISA.ImportDemo.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Core.Interfaces
{
    /// <summary>
    /// Interface bace
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// CreateBy: NVMANH (20/04/2020)
    public interface IBaseEntityService<T>
    {
        /// <summary>
        /// Lấy toàn bộ dữ liệu
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyList<T>> GetEntities();

        /// <summary>
        /// Lấy dữ liệu phân trang
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetEntitiesPaging();

        /// <summary>
        /// Lấy theo khóa chính
        /// </summary>
        /// <param name="id">PK bản ghi trong CSDL</param>
        /// <returns></returns>
        /// CreateBy: NVMANH (20/04/2020)
        Task<T> GetEntityById(object id);

        /// <summary>
        /// Thêm mới dữ liệu
        /// </summary>
        /// <param name="entity">đối tượng thêm mới</param>
        /// <param name="returnSingleValue">lấy giá trị trả về từ store hay không (true: có)</param>
        /// <returns>Số bản ghi được thêm mới</returns>
        /// CreateBy: NVMANH (20/04/2020)
        Task<ActionServiceResult> Insert(T entity, bool returnSingleValue = false);

        Task InsertRange(List<T> entities);

        /// <summary>
        /// Sửa dữ liệu
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Số bản ghi sửa thành công</returns>
        /// CreateBy: NVMANH (20/04/2020)
        Task<ActionServiceResult> Update(T entity);

        /// <summary>
        /// Xóa dữ liệu theo khóa chính
        /// </summary>
        /// <param name="id">PK bản ghi trong CSDL</param>
        /// <returns>Số bản ghi được xóa trong Database</returns>
        /// CreateBy: NVMANH (20/04/2020)
        Task<int> Delete(object id);


    }
}
