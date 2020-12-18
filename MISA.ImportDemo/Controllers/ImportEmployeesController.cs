using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.ImportDemo.Core.Entities;
using MISA.ImportDemo.Core.Enumeration;
using MISA.ImportDemo.Core.Interfaces;

namespace MISA.ImportDemo.Controllers
{
    /// <summary>
    /// Service thực hiện nhập khẩu nhân viên
    /// </summary>
    /// Author: NVMANH (10/10/2020)
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ImportEmployeesController : BaseEntityController<ImportFileTemplate>
    {
        readonly IImportEmployeeService _importService;

        /// <summary>
        /// Khởi tạo service
        /// </summary>
        /// <param name="importService"></param>
        public ImportEmployeesController(IImportEmployeeService importService) : base(importService)
        {
            _importService = importService;
        }

        /// <summary>
        /// Api thực hiện việc đọc và phân loại dữ liệu từ file Excel - hồ sơ lao động
        /// </summary>
        /// <param name="fileImport">Tệp nhập khẩu</param>
        /// <param name="cancellationToken">Tham số tùy chọn xử lý đa luồng (hiện tại chưa sử dụng)</param>
        /// <returns>200: Nhập khẩu thành công</returns>
        /// CreatedBy: NVMANH(05/2020)
        [HttpPost("reader")]
        public async Task<IActionResult> UploadImportFile(IFormFile fileImport, CancellationToken cancellationToken)
        {
            var res = await _importService.ReadEmployeeDataFromExcel(fileImport, cancellationToken);
            return Ok(res);
        }

        /// <summary>
        /// Thực hiện nhập khẩu dữ liệu
        /// </summary>
        /// <param name="keyImport">Key xác định lấy dữ liệu để nhập khẩu từ cache</param>
        /// <param name="overriderData">Có cho phép ghi đè hay không (true- ghi đè dữ liệu trùng lặp trong db)</param>
        /// <param name="cancellationToken">Tham số tùy chọn xử lý đa luồng (hiện tại chưa sử dụng)</param>
        /// <returns>ActionResult(với các thông tin tương ứng tùy thuộc kết nhập khẩu)</returns>
        /// CreatedBy: NVMANH(05/2020)
        [HttpPost("{keyImport}")]
        public async Task<ActionResult<ImportFileTemplate>> Post(string keyImport, bool overriderData, CancellationToken cancellationToken)
        {
            await _importService.Import(keyImport,overriderData, cancellationToken);
            return Ok(new ActionServiceResult(true, Core.Properties.Resources.Msg_ImportSuccess, MISACode.Success));
        }
    }
}
