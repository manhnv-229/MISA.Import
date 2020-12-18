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
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ImportEmployeesController : BaseEntityController<ImportFileTemplate>
    {
        readonly IImportEmployeeService _importService;
        public ImportEmployeesController(IImportEmployeeService importService) : base(importService)
        {
            _importService = importService;
        }

        /// <summary>
        /// Api thực hiện việc đọc và phân loại dữ liệu từ file Excel - hồ sơ lao động
        /// </summary>
        /// <param name="fileImport"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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
        /// <param name="keyImport"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="overriderData">Ghi đè dữ liệu hay không (true-có ghi đè)</param>
        /// <returns></returns>
        [HttpPost("{keyImport}")]
        public async Task<ActionResult<ImportFileTemplate>> Post(string keyImport, bool overriderData, CancellationToken cancellationToken)
        {
            await _importService.Import(keyImport,overriderData, cancellationToken);
            return Ok(new ActionServiceResult(true, "Nhập khẩu thành công!", MISACode.Success));
        }
    }
}
