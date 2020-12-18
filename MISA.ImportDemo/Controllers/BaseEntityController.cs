using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MISA.ImportDemo.Core.Entities;
using MISA.ImportDemo.Core.Enumeration;
using MISA.ImportDemo.Core.Interfaces;
using MISA.ImportDemo.Core.Properties;

namespace MISA.ImportDemo.Controllers
{
    /// <summary>
    /// Class Base
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// CreateBy: NVMANH (20/04/2020)
    [Route("api/v1/[controller]")]
    [ApiController]
    public abstract class BaseEntityController<T> : ControllerBase
    {
        /// <summary>
        /// Interface reference tới BaseEntityService
        /// </summary>
        protected readonly IBaseEntityService<T> _baseEntityService;
        
        /// <summary>
        /// Hàm khởi tạo mặc định
        /// </summary>
        /// CreatedBy: NVMANH (12/12/2012)
        public BaseEntityController()
        {
           
        }

        /// <summary>
        /// Khởi tạo service chung
        /// </summary>
        /// <param name="baseEntityService"></param>
        /// CreateBy: NVMANH (20/04/2020)
        public BaseEntityController(IBaseEntityService<T> baseEntityService)
        {
            _baseEntityService = baseEntityService;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách đối tượng
        /// </summary>
        /// <returns></returns>
        /// CreatedBy: NVMANH (14/04/2020)
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<T>>> GetEntities()
        {
            var entities = await _baseEntityService.GetEntities();
            return Ok(entities);
        }

        /// <summary>
        /// Lấy thông tin theo mã (khóa chính)
        /// </summary>
        /// <param name="id">giá trị khóa chính trong bảng CSDL</param>
        /// <returns></returns>
        /// CreateBy: NVMANH (20/04/2020)
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("{id}")]
        public virtual async Task<ActionResult<T>> GetEntityByID(string id)
        {
            var entity = await _baseEntityService.GetEntityById(id);
            if (entity == null)
            {
                return NotFound();
            }
            return Ok(entity);
        }

        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="entity">Đối tượng thêm mới</param>
        /// <returns></returns>
        /// CreateBy: NVMANH (20/04/2020)
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        public virtual async Task<ActionResult<T>> Post([FromBody] T entity)
        {
            // Validate dữ liệu theo các Attribure Property
            if (!ModelState.IsValid)
            {
                var resultValidate = new ActionServiceResult()
                {
                    Success = false,
                    Messenge = Resources.ErrorValidate_NotValid,
                    MISACode = MISACode.ValidateEntity,
                    Data = ModelState
                };
                return BadRequest(resultValidate);
            }

            var result = await _baseEntityService.Insert(entity, true);

            if (result.Success == false)
                return BadRequest(result);
            var tableName = entity.GetType().Name;
            return Created("Created " + tableName, result.Data);
        }

        /// <summary>
        /// Thêm List Object
        /// </summary>
        /// <param name="entities">List các object</param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (22/05/2020)
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("range")]
        public virtual async Task<ActionResult> Post([FromBody] List<T> entities)
        {
            await _baseEntityService.InsertRange(entities);
            return Ok();
        }

        /// <summary>
        /// Cập nhật
        /// </summary>
        /// <param name="entity">Đối tượng sửa</param>
        /// <returns></returns>
        /// CreateBy: NVMANH (20/04/2020)
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut]
        public virtual async Task<ActionResult<T>> Put([FromBody] T entity)
        {
            var result = await _baseEntityService.Update(entity);
            if (entity == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// Xóa theo ID
        /// </summary>
        /// <param name="id">id của đối tượng</param>
        /// <returns></returns>
        /// CreateBy: NVMANH (20/04/2020)
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete]
        public virtual async Task<ActionResult<T>> Delete(object id)
        {
            var result = await _baseEntityService.Delete(id);
            if (id == null)
            {
                return BadRequest("ID không hợp lệ");
            }
            return Ok(result);
        }

    }

}