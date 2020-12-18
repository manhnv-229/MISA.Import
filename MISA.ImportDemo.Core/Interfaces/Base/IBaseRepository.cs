using MISA.ImportDemo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Core.Interfaces
{
    /// <summary>
    /// Interface của base: định nghĩa các phương thức bắt buộc
    /// </summary>
    /// <typeparam name="T">Đối tượng được khởi tạo cụ thể</typeparam>
    /// CreatedBy: NVMANH (23/04/2020)
    public interface IBaseRepository<T> where T : BaseEntity, IAggregateRoot
    {
        /// <summary>
        /// Lấy List toàn bộ dữ liệu trong bảng:
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyList<T>> GetListAsync();

        /// <summary>
        /// Lấy thông tin đối tượng theo khóa chính
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<T> GetEntityByIdAsync(object Id);

        /// <summary>
        /// Lấy toàn bộ dữ liệu theo điều kiện truyền vào của đối tượng
        /// </summary>
        /// <param name="spec"></param>
        /// <returns></returns>
        Task<IReadOnlyList<T>> GetListAsyncBySpecification(ISpecification<T> spec);

        /// <summary>
        /// Lấy toàn bộ dữ liệu theo điều kiện truyền vào của đối tượng
        /// Cho phép truyền vào instance muốn lấy về
        /// </summary>
        /// <param name="spec"></param>
        Task<IReadOnlyList<Y>> GetListAsyncBySpecification<Y>(ISpecification<Y> spec) where Y : BaseEntity;

        /// <summary>
        /// Lấy dữ liệu theo  tham số truyền vào (Theo thứ tự trong store)
        /// </summary>
        /// <param name="parameters">Mảng chứa các tham số truyền vào cho store theo đúng thứ tự</param>
        Task<IReadOnlyList<T>> GetListAsync(object[] parameters);

        /// <summary>
        /// Lấy dữ liệu có phần trang
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetListPagedAsync();

        /// <summary>
        /// Thêm mới đối tượng vào bảng
        /// </summary>
        /// <param name="entity">Đối tượng</param>
        /// <param name="returnValue">Có trả về giá trị đơn hay không [VD: ID của bản ghi vừa tạo] - true: có</param>
        /// <returns>Kết quả trả về là số bản ghi ảnh hưởng hoặc 1 giá trị đơn lẻ được select khi set returnValue = true</returns>
        Task<object> AddAsync(T entity, bool returnValue = false);

        /// <summary>
        /// Thêm mới đối tượng và trả về đối tượng được thêm mới
        /// </summary>
        /// <param name="entity">Đối tượng muốn thêm mới</param>
        /// <returns>Đối tượng vừa thực hiện thêm mới</returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Thêm 1 List object
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task AddRangeAsync(List<T> entities);

        /// <summary>
        /// Chỉ thực hiện update dữ liệu - không trả về gì
        /// </summary>
        /// <param name="entity"></param>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Chỉ xóa đối tượng, không trả về bất cứ đối tượng hay giá trị nào.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Đếm số bản ghi có trong dữ liệu theo điều kiện cụ thể
        /// </summary>
        /// <param name="spec"></param>
        /// <returns></returns>
        Task<int> CountAsync(ISpecification<T> spec);

        

        /// <summary>
        /// PUT dữ liệu vào bảng trả về số lượng bản ghi được update
        /// </summary>
        Task<int> Update(T entity);

        /// <summary>
        /// Delete dữ liệu theo các tham số truyền vào
        /// Các tham số truyền vào theo thứ tự tương ứng với thứ tự được viết trong store
        /// </summary>
        Task<int> Delete(object[] param);
       
    }
}
