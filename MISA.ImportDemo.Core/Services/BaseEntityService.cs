using MISA.ImportDemo.Core.Entities;
using MISA.ImportDemo.Core.Enumeration;
using MISA.ImportDemo.Core.Interfaces;
using MISA.ImportDemo.Core.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Core.Services
{
    /// <summary>
    /// Service chung
    /// </summary>
    /// <typeparam name="T">Generic</typeparam>
    /// CreateBy: NVMANH (20/04/2020)
    public class BaseEntityService<T> : IBaseEntityService<T> where T : BaseEntity, IAggregateRoot
    {
        protected readonly IBaseRepository<T> _baseRepository;
        /// <summary>
        /// Khởi tạo service
        /// </summary>
        /// <param name="baseRepository"></param>
        public BaseEntityService(IBaseRepository<T> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        /// <summary>
        /// Lấy toàn bộ dữ liệu
        /// </summary>
        /// <returns></returns>
        /// CreateBy: NVMANH (20/04/2020)
        public async virtual Task<IReadOnlyList<T>> GetEntities()
        {
            var entities = await _baseRepository.GetListAsync();
            return entities;
        }

        /// <summary>
        /// Lấy dữ liệu có phân trang
        /// </summary>
        /// <returns></returns>
        /// CreateBy: NVMANH (20/04/2020)
        public async Task<IEnumerable<T>> GetEntitiesPaging()
        {
            return await _baseRepository.GetListPagedAsync();
        }

        /// <summary>
        /// Lấy thông tin đối tượng theo khóa chính
        /// </summary>
        /// <param name="id">ID của bản ghi</param>
        /// <returns></returns>
        /// CreateBy: NVMANH (20/04/2020)
        public async virtual Task<T> GetEntityById(object id)
        {
            return await _baseRepository.GetEntityByIdAsync(id);
        }

        /// <summary>
        /// Sửa thông tin bản ghi
        /// </summary>
        /// <param name="entity">đối tượng thực hiện sửa</param>
        /// <returns></returns>
        public async virtual Task<ActionServiceResult> Update(T entity)
        {
            if (Validate(entity) == true)
                return new ActionServiceResult
                {
                    Success = true,
                    MISACode = MISACode.Success,
                    Messenge = Resources.ErrorValidate_NotValid,
                    Data = await _baseRepository.Update(entity)
                };

            return new ActionServiceResult
            {
                Success = false,
                MISACode = MISACode.ValidateBussiness,
                Messenge = Resources.ErrorValidate_NotValid,
                Data = null
            };
        }

        /// <summary>
        /// Thêm mới bản ghi vào CSDL
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="returnSingleValue">Có trả về dữ liệu tùy chọn từ store hay không</param>
        /// <returns></returns>
        /// CreateBy: NVMANH (20/04/2020)
        public async virtual Task<ActionServiceResult> Insert(T entity, bool returnSingleValue = false)
        {
            if (Validate(entity) == true)
                return new ActionServiceResult
                {
                    Success = true,
                    MISACode = MISACode.Success,
                    Data = await _baseRepository.AddAsync(entity, returnSingleValue)
                };
            else
                return new ActionServiceResult
                {
                    Success = false,
                    MISACode = MISACode.ValidateBussiness,
                    Messenge = Resources.ErrorValidate_NotValid,
                    Data = null
                };
        }

        
        /// <summary>
        /// Xóa bản ghi dựa vào khóa chính
        /// </summary>
        /// <param name="id">Khóa chính</param>
        /// <returns></returns>
        /// CreateBy: NVMANH (20/04/2020)
        public async virtual Task<int> Delete(object id)
        {
            return await _baseRepository.Delete(new object[] { id });
        }


        /// <summary>
        /// Hàm thực hiện validate trước khi cất dữ liệu
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected virtual bool Validate(T entity)
        {
            return true;
        }

        /// <summary>
        /// Add List Range
        /// </summary>
        /// <param name="entities">List các objects</param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (22/05/2020)
        public async Task InsertRange(List<T> entities)
        {
            await _baseRepository.AddRangeAsync(entities);
        }
    }
}
