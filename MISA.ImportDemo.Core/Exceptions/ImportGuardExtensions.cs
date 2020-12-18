using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Http;
using MISA.ImportDemo.Core.Entities;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;

namespace MISA.ImportDemo.Core.Exceptions
{
    public static class ImportGuardExtensions
    {
        public static void NullFile(this IGuardClause guardClause, IFormFile formFile)
        {
            if (formFile == null)
                throw new ImportException("Không tìm thấy File import");
        }

        /// <summary>
        /// Kiểm tra định dạng File có đúng hay không
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="formFile"></param>
        /// <param name="importFileTemplate"></param>
        /// CreatedBy: NVMANH
        public static void FileExtensionInvalid(this IGuardClause guardClause, IFormFile formFile,ImportFileTemplate importFileTemplate)
        {
            var extension = Path.GetExtension(formFile.FileName).Replace(".",string.Empty);
            var extensionTemplate = importFileTemplate.FileFormat;
            if (extensionTemplate != null && extensionTemplate!= string.Empty && extensionTemplate.Contains(extension)==false)
                throw new ImportException(string.Format("Tệp nhập khẩu không đúng định dạng, vui lòng chọn tệp có định dạng là [{0}]", extensionTemplate));
        }

        /// <summary>
        /// Kiểm tra vị trí của worksheet và tên worksheet
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="importWorksheet">worksheet mẫu</param>
        /// <param name="sheetName">Tên sheet</param>
        /// <param name="sheetPosition">Vị trí sheet hiện tại</param>
        /// CreatedBy: NVMANH
        public static void WorksheetInValid(this IGuardClause guardClause, ImportWorksheet importWorksheet, string sheetName, int sheetPosition)
        {
            if(importWorksheet == null)
                throw new ImportException(String.Format("Tệp nhập khẩu không đúng mẫu, worksheet [{0}] sai vị trí hoặc không tồn tại trong tệp mẫu", sheetName));

        }

        /// <summary>
        /// Kiểm tra số lượng worksheet có đúng với file mẫu hay không?
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="worksheetCount"></param>
        /// <param name="worksheetTemplateCount"></param>
        /// <param name="importFileTemplate"></param>
        /// CreatedBy: NVMANH (05/2020)
        public static void FileContentInvalid(this IGuardClause guardClause,int worksheetCount, int worksheetTemplateCount, ImportFileTemplate importFileTemplate)
        {
            if(worksheetCount != worksheetTemplateCount)
                throw new ImportException(String.Format("Tệp nhập khẩu không đúng mẫu, số lượng Worlsheet trong file mẫu là {0}", importFileTemplate.TotalWorksheet));
        }
        /// <summary>
        /// Hàm kiểm tra xem file có đúng file mẫu hay không
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="formFile"></param>
        /// CreatedBy: NVMANH
        public static void FileContentInvalid(this IGuardClause guardClause, ExcelPackage excelPackage, ImportFileTemplate importFileTemplate)
        {
            // Check số lượng Sheet:
            var worksheets = excelPackage.Workbook.Worksheets;
            var workSheetCount = worksheets.Count;
            if (workSheetCount != worksheets.Count())
                throw new ImportException(String.Format("Tệp nhập khẩu không đúng mẫu, số lượng Worlsheet trong file mẫu là {0}", importFileTemplate.TotalWorksheet));

            // Check số lượng cột ở từng sheet:
            for (int i = 0; i < workSheetCount; i++)
            {
                var sheet = worksheets[i];
                var sheetName = sheet.Name;
                var totalColumns = sheet.Dimension.Columns;

                // Check xem Worksheet trên tệp có giống File mẫu hay không?
                var worksheetTemplate = importFileTemplate.ImportWorksheet.Where(e => e.ImportWorksheetName.Trim().ToLower() == sheetName.Trim().ToLower() && e.WorksheetPosition == i + 1).FirstOrDefault();

                if (sheet.Hidden == OfficeOpenXml.eWorkSheetHidden.Hidden || (worksheetTemplate!= null && worksheetTemplate.IsImport == 0))
                    continue;

                if (worksheetTemplate == null)
                    throw new ImportException(String.Format("Tệp nhập khẩu không đúng mẫu, worksheet [{0}] sai vị trí hoặc không tồn tại trong tệp mẫu", sheetName));
                
                // Check số lượng cột trong workSheet có giống với số lượng cột tại worksheet như tệp mẫu hay không?
                var listColumnsTemplate = worksheetTemplate.ImportColumn;
                if (totalColumns != listColumnsTemplate.Count())
                    throw new ImportException(String.Format("Tệp nhập khẩu không đúng mẫu, số lượng cột tại worksheet [{1}] trong file mẫu là {0}", listColumnsTemplate.Count(), sheetName));

                // For từng cột, kiểm tra tiêu đề cột có khớp hay không:
                var range = sheet.Cells[1, 1, 1, totalColumns];
                for (int j = 1; j < totalColumns; j++)
                {
                    var headerName = range[1, j].Value.ToString().Replace("\n"," ");
                    var headerNameTemplate = listColumnsTemplate.Where(col => (col.ColumnTitle.Trim().ToLower().Contains(headerName.Trim().ToLower()) && j == col.ColumnPosition)).FirstOrDefault();
                    var address = range[1, j].Address;
                    if (headerNameTemplate == null)
                        throw new ImportException(String.Format("Tệp nhập khẩu không đúng mẫu, vị trí cột [{0}] tại worksheet [{1}] - [{2}] sai hoặc không tồn tại trong file mẫu", headerName, sheetName, address));
                }
            }
            // Các cột có tiêu đề giống với File mẫu hay không, có đúng thứ tự hay không?

        }
    }
}
