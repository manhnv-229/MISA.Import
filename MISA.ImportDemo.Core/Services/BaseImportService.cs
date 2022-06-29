using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using MISA.ImportDemo.Core.Entities;
using MISA.ImportDemo.Core.Enumeration;
using MISA.ImportDemo.Core.Enums;
using MISA.ImportDemo.Core.Exceptions;
using MISA.ImportDemo.Core.Interfaces;
using MISA.ImportDemo.Core.Interfaces.Repository;
using MISA.ImportDemo.Core.Properties;
using MISA.ImportDemo.Core.Specifications;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Core.Services
{
    public class BaseImportService : BaseEntityService<ImportFileTemplate>
    {
        #region DECLARE
        protected IBaseImportRepository _importRepository;
        protected IMemoryCache importMemoryCache;
        protected List<Nationality> Nationalities;
        protected List<Position> Positions;
        protected List<Department> Departments;
        protected List<Relation> Relations;
        protected List<Ethnic> Ethnics;
        protected EnumUtility _enumUtility;
        protected string TableToImport;
        protected ExcelWorksheet Worksheet;
        protected ImportWorksheet ImportWorksheetTemplate;
        protected List<object> _entitiesFromEXCEL = new List<object>();
        protected List<object> EntitiesFromDatabase;
        protected readonly Organization CurrentOrganization;
        protected List<Position> _newPossitons = new List<Position>();

        #endregion

        #region CONSTRUCTOR
        public BaseImportService(IBaseImportRepository importRepository, IMemoryCache importMemoryCache, string tableToImport) : base(importRepository)
        {
            _importRepository = importRepository;
            _enumUtility = new EnumUtility();
            this.importMemoryCache = importMemoryCache;
            TableToImport = tableToImport;
            // Lấy dữ liệu từ cache:
            Nationalities = (List<Nationality>)_importRepository.CacheGet("Nationalities");
            Positions = (List<Position>)_importRepository.CacheGet("Positions");
            Departments = (List<Department>)_importRepository.CacheGet("Departments");
            Relations = (List<Relation>)_importRepository.CacheGet("Relations"); ;
            Ethnics = (List<Ethnic>)_importRepository.CacheGet("Ethnics");
            CurrentOrganization = _importRepository.GetCurrentOrganization();
        }

        #endregion

        #region METHOD
        /// <summary>
        /// Đọc dữ liệu từ tệp nhập khẩu.
        /// </summary>
        /// <param name="importFile">File nhập khẩu</param>
        /// <param name="cancellationToken"></param>
        /// <returns>List các nhân viên được đọc thành công từ tệp nhập khẩu</returns>
        /// CreatedBy: NVMANH (20/05/2020)
        public async Task<List<T>> ReadDataFromExcel<T>(IFormFile importFile, CancellationToken cancellationToken) where T : BaseEntity
        {
            // Kiểm tra xem File có hay không?
            Guard.Against.NullFile(importFile);

            // Lấy thông tin tệp nhập khẩu: dựa vào tên bảng thực hiện nhập khẩu:
            var fileImpportSpec = new ImportFileSpecification(TableToImport);
            var importFileTemplate = await _importRepository.GetFileImportInfo(fileImpportSpec);
            Guard.Against.FileExtensionInvalid(importFile, importFileTemplate);

            var list = new List<T>();

            using (var stream = new MemoryStream())
            {
                await importFile.CopyToAsync(stream, cancellationToken);
                using (var package = new ExcelPackage(stream))
                {
                    // Check xem File nhập khẩu có các thông tin đúng với mẫu hay không?
                    // Guard.Against.FileContentInvalid(package, importFileTemplate);
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;
                    list = BuildListDataFromExcel<T>(package, importFileTemplate).Cast<T>().ToList();
                }
            }
            return list;
        }

        /// <summary>
        /// Thực hiện build dữ liệu nhập khẩu.
        /// </summary>
        /// <param name="excelPackage">Phạm vi chứa dữ liệu của file Excel</param>
        /// <param name="importFileTemplate">Đối tượng chứa thông tin file mẫu nhập khẩu được khai báo trong Database</param>
        /// <returns>Danh sách các đối tượng được build từ File Excel</returns>
        /// CreatedBy: NVMANH (20/05/2020)
        public List<object> BuildListDataFromExcel<T>(ExcelPackage excelPackage, ImportFileTemplate importFileTemplate) where T : BaseEntity
        {

            var worksheets = excelPackage.Workbook.Worksheets;
            var workSheetCount = worksheets.Count;
            // Duyệt từng sheet nhé 
            // - mỗi sheet sẽ được khai báo tương ứng trong database gồm các thông tin: sheet này sẽ mapping với object nào, bảng sẽ lưu dữ liệu là gì...
            for (int i = 0; i < workSheetCount; i++)
            {
                Worksheet = worksheets[i];
                var sheetName = Worksheet.Name;

                // Vả vào mồm người dùng nếu tệp không đúng là tệp mẫu: Tệp mẫu của MISA cấp luôn có ít nhất 1 sheet và phải có khai báo các thông tin - tức là không được phép null.
                if (Worksheet.Dimension == null)
                    throw new ImportException(string.Format(Resources.Error_ImportFile_NotMatchTemplate));

                var totalColumns = Worksheet.Dimension.Columns;

                // Check xem Worksheet trên tệp có giống File mẫu hay không? Không có lại vả tiếp
                ImportWorksheetTemplate = importFileTemplate.ImportWorksheet.Where(e => e.ImportWorksheetName.Trim().ToLower() == sheetName.Trim().ToLower() || e.WorksheetPosition == i + 1).FirstOrDefault();

                // Bỏ qua các sheet ẩn và các sheet được khai báo là không nhập khẩu:
                if (ImportWorksheetTemplate == null || Worksheet.Hidden == OfficeOpenXml.eWorkSheetHidden.Hidden || (ImportWorksheetTemplate != null && ImportWorksheetTemplate.IsImport == 0))
                    continue;

                // Bảng dữ liệu trong database được khai báo sẽ lưu trữ đối tượng là gì? nếu không khai báo thì bỏ qua nốt.
                TableToImport = ImportWorksheetTemplate.ImportToTable;
                if (string.IsNullOrEmpty(TableToImport))
                    continue;

                //Guard.Against.FileContentInvalid(workSheetCount, importFileTemplate.ImportWorksheet.Count, importFileTemplate);
                //Guard.Against.WorksheetInValid(worksheetTemplate, sheetName, i);

                // Check số lượng cột trong workSheet có giống với số lượng cột tại worksheet như tệp mẫu hay không?
                // Không giống tức là không đúng mẫu - vả lỗi vào mồm!
                var listColumnsTemplate = ImportWorksheetTemplate.ImportColumn;
                if (totalColumns != listColumnsTemplate.Count())
                    throw new ImportException(String.Format(Resources.Error_ImportFileWorksheetPossitionInvalid, listColumnsTemplate.Count(), sheetName));

                // For từng cột [xác định là tiêu đề], kiểm tra tiêu đề cột có khớp hay không - không khớp tức là cột linh tinh không đúng mẫu - ném lỗi vào mồm:
                var rangeHeader = Worksheet.Cells[1, 1, 1, totalColumns];
                for (int j = 1; j < totalColumns; j++)
                {
                    var headerName = rangeHeader[1, j].Value.ToString().Replace("\n", "");
                    var excelHeaderNameRemoveDiacritics = RemoveDiacritics(headerName).Trim().ToLower().Replace(" ", string.Empty);
                    var headerNameTemplate = listColumnsTemplate.Where(col => (RemoveDiacritics(col.ColumnTitle.Trim().ToLower()).Replace(" ", string.Empty).Contains(excelHeaderNameRemoveDiacritics))).FirstOrDefault();
                    var address = rangeHeader[1, j].Address;
                    if (headerNameTemplate == null)
                        throw new ImportException(String.Format(Resources.Error_ImportFileColumnPossitionInvalid, headerName, sheetName, address));
                }

                // Khai báo một số biến cần dùng trước khi thực hiện mapping dữ liệu nhập khẩu từng dòng:
                var rowCount = Worksheet.Dimension.Rows;
                var rowBeginImport = ImportWorksheetTemplate.RowStartImport ?? 0; // Nếu null thì lấy từ vị trí thứ 2 (sau vị trí tiêu đề)
                var tableName = importFileTemplate.TableImport;
                if (rowBeginImport > rowCount)
                    throw new ImportException(Resources.Error_ImportFileColumnHeaderPossitionInvalid);

                // Duyệt từng dòng, thực hiện mapping các giá trị:
                for (int row = rowBeginImport; row <= rowCount; row++)
                {
                    List<T> entitiesFromExcel = _entitiesFromEXCEL.Cast<T>().ToList();
                    var entity = BuildObject(Worksheet, rangeHeader, listColumnsTemplate, totalColumns, row, entitiesFromExcel);
                    SetOrgId(entity);
                    ProcessDataAfterBuild<T>(entity);
                }

            }
            return _entitiesFromEXCEL;
        }

        /// <summary>
        /// Thực hiện khởi tạo các giá trị cần thiết trước khi mapping dữ liệu (VD: gán ID của master)
        /// Hàm này cho phép overrider lại với mục định khởi tạo động được object với kiểu bất kỳ khi có nhu cầu.
        /// </summary>
        /// <typeparam name="T">Kiểu của đối tượng (VD: Employee)</typeparam>
        /// <returns>Một object cụ thể bao gồm các thông tin mặc định</returns>
        /// CreatedBy: NVMANH (12/12/2020)
        protected virtual dynamic InstanceEntityBeforeMappingData<T>() where T : BaseEntity
        {
            return Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Set lại thông tin công ty (nếu có) cho entity sau khi build các thông tin khác thành công
        /// </summary>
        /// <param name="entity">Thực thể đang build</param>
        /// CreatedBy: NVMANH (12/12/2020)
        private void SetOrgId(object entity)
        {
            var orgIdProperty = entity.GetType().GetProperty("OrganizationId", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
            var organization = _importRepository.GetCurrentOrganization();
            if (orgIdProperty != null && organization != null)
                orgIdProperty.SetValue(entity, organization.OrganizationId);
        }

        /// <summary>
        /// Hàm xử lý sau khi build được object
        /// </summary>
        /// <typeparam name="T">Kiểu của đối tượng (VD: Employee)</typeparam>
        /// <param name="entity">thực thể đang dựng</param>
        /// CreatedBy: NVMANH (12/12/2020)
        protected virtual void ProcessDataAfterBuild<T>(object entity) where T : BaseEntity
        {
            // Trên Excel phải nhập ít nhất 1 ô dữ liệu - nếu không nhập ô nào thì coi như đó là dòng trống
            // Trường HasPropertyValueImport sinh ra để làm mục đích này:
            if (entity is T)
            {
                var entityCast = entity as T;
                if (entityCast.HasPropertyValueImport == true)
                    _entitiesFromEXCEL.Add(entity);
            }
        }

        /// <summary>
        /// Build object theo dữ liệu từng dòng.
        /// Cơ chế:  - Đọc từng ô ở trong các dòng, 
        ///          - Mỗi ô dữ liệu đọc được sẽ phân tích các thông tin (giá trị/ kiểu dữ liệu...) 
        ///          - Thực hiện xử lý dữ liệu đó và gán lại giá trị cho property tương ứng của thực thể
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="worksheet">Sheet dữ liệu đang đọc</param>
        /// <param name="rangeHeader">Khung chứa dữ liệu của tiêu đề</param>
        /// <param name="listColumnsTemplate">danh sách mô tả các cột dữ liệu trong Database</param>
        /// <param name="totalColumns">Tổng số cột dữ liệu trong tệp nhập khẩu</param>
        /// <param name="rowIndex">Vị trí dòng hiện tại</param>
        /// <param name="entitiesInFile">List các đối tượng đã được build từ tệp nhập khẩu</param>
        /// CreatedBy: NVMANH (25/12/2020)
        protected object BuildObject<T>(ExcelWorksheet worksheet, ExcelRange rangeHeader, ICollection<ImportColumn> listColumnsTemplate, int totalColumns, int rowIndex, List<T> entitiesInFile = null) where T : BaseEntity
        {
            // Đầu tiên là cứ khởi tạo object với các thông tin mặc định (trống/ null hoặc gán mặc định từ hàm khởi tạo)
            var entity = InstanceEntityBeforeMappingData<T>();
            for (int columnIndex = 1; columnIndex <= totalColumns; columnIndex++)
            {
                var headerName = rangeHeader[1, columnIndex].Value.ToString().Replace("\n", " ");
                Console.WriteLine(headerName);
                var headerNameRemoveDiacritics = RemoveDiacritics(headerName);
                var importColumnTemplate = listColumnsTemplate.Where(col => RemoveDiacritics(col.ColumnTitle).Contains(headerNameRemoveDiacritics) && columnIndex == col.ColumnPosition).FirstOrDefault();
                var address = rangeHeader[1, columnIndex].Address;

                // Không có cột tương ứng được khai báo trong Database: bỏ qua và duyệt tiếp.
                if (importColumnTemplate == null)
                    continue;

                var dataType = importColumnTemplate.ColumnDataType != null ? (DataType)(importColumnTemplate.ColumnDataType) : DataType.String;
                var cellValue = worksheet.Cells[rowIndex, columnIndex].Value;
                CellValueValid(entity, cellValue, importColumnTemplate);

                if (cellValue == null)
                    continue;

                // Check trùng dữ liệu:
                if (entity is T)
                    CheckDuplicateData(entitiesInFile, entity, cellValue, importColumnTemplate);

                // Xử lý dữ liệu từng ô trên Excel và thực hiện mapping tương ứng với entity:
                ProcessCellValueByDataType<T>(dataType, ref cellValue, entity, importColumnTemplate);

                // Nếu không có thông tin cột mapping được khai báo trong Db, bỏ qua duyệt cell tiếp theo
                var columnInsert = importColumnTemplate.ColumnInsert;
                if (columnInsert == null && !string.IsNullOrEmpty(importColumnTemplate.ObjectReferenceName))
                    continue;

                // Nếu không có property tương ứng với cột mapping đã khai báo -> bỏ qua và duyệt cell tiếp theo
                var property = entity.GetType().GetProperty(columnInsert ?? string.Empty, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                if (property == null)
                    continue;
                var propertyType = property.PropertyType;
                // Kiểu ulong do đặc điểm entity framework tự động chuyển kiểu Boolean sang lên cần phải làm thêm việc này
                if (propertyType == typeof(ulong) || propertyType == typeof(ulong?))
                    cellValue = Convert.ToUInt64(cellValue);

                if (cellValue != null && (Nullable.GetUnderlyingType(propertyType) == cellValue.GetType() || propertyType == cellValue.GetType()))
                    property.SetValue(entity, cellValue);
            }
            return entity;
        }

        /// <summary>
        /// Thực hiện xử lý giá trị của cell theo từng loại dữ liệu được khai báo
        /// </summary>
        /// <typeparam name="T">Type của thực thể (bắt buộc class của thực thể phải kế thừa class Base Entity)</typeparam>
        /// <param name="dataType">Kiểu dữ liệu (string, số, enum...) được khai báo trong cơ sở dữ liệu</param>
        /// <param name="cellValue">Giá trị lấy từ ô nhập trong File Excel</param>
        /// <param name="entity">Thực thể sẽ gán các giá trị từ cell sau khi xử lý xong</param>
        /// <param name="importColumnTemplate">Thông tin cột nhập khẩu được khai báo trong Database</param>
        /// CreatedBy: NVMANH (25/05/2020)
        protected virtual void ProcessCellValueByDataType<T>(DataType dataType, ref object cellValue, dynamic entity, ImportColumn importColumnTemplate) where T : BaseEntity
        {
            switch (dataType)
            {
                case DataType.Boolean:
                    if (cellValue.ToString().ToLower() == "có")
                        cellValue = 1;
                    else
                        cellValue = 0;
                    break;
                // Xử lý nếu kiểu dữ liệu khai báo là kiểu Enum:
                case DataType.Enum:
                    var enumName = importColumnTemplate.ObjectReferenceName; // Enum khai báo trong Database
                    // Kiểm tra xem có Enum nào có tên như được khai báo hay không, nếu không khai báo thì bỏ qua:
                    // -> Nếu có thì thực hiện Lấy key từ Resource -> sau đó lấy giá trị và gán lại Property tương ứng" 
                    var enumNameStringContains = string.Format("MISA.ImportDemo.Core.Enumeration.{0}", enumName);
                    var enumType = Type.GetType(enumNameStringContains);
                    if (enumType == null)
                        return;

                    // 1. Lấy key từ Resource: 
                    var resourceStringKeyContains = string.Format("Enum_{0}", enumName);
                    var resourceKey = _enumUtility.GetResourceNameByValue(cellValue.ToString(), resourceStringKeyContains);
                    // Nếu không có resource có giá trị tương ứng thì cảnh báo lỗi:
                    if (resourceKey == null)
                    {
                        entity.ImportValidError.Add(string.Format("Thông tin [{0}] nhập không chính xác.", importColumnTemplate.ColumnTitle));
                        return;
                    }

                    // 2. -> sau đó lấy giá trị và gán lại Property tương ứng"
                    var enumKey = resourceKey.Replace(resourceStringKeyContains + "_", string.Empty);
                    var enumValue = (int)Enum.Parse(enumType, enumKey);
                    cellValue = enumValue;
                    CustomAfterSetCellValueByColumnInsertWhenEnumReference(entity, enumType, importColumnTemplate.ColumnInsert, ref cellValue);
                    break;
                case DataType.ReferenceTable:
                    ProcessCellValueByDataTypeWhenTableReference<T>(entity, ref cellValue, importColumnTemplate);
                    break;
                default:
                    if (importColumnTemplate.ColumnInsert == null)
                        return;

                    // Nếu không có property tương ứng với cột mapping đã khai báo -> bỏ qua và duyệt cell tiếp theo
                    var property = entity.GetType().GetProperty(importColumnTemplate.ColumnInsert);
                    if (property == null)
                        return;

                    var propertyType = property.PropertyType;
                    entity.HasPropertyValueImport = true;
                    if (cellValue == null || cellValue is DBNull)
                        break;
                    else
                    {
                        if (propertyType == typeof(string))
                        {
                            cellValue = cellValue.ToString().Trim();
                            if (importColumnTemplate.ColumnInsert == "FullName")
                            {
                                var fullNames = new List<string>(cellValue.ToString().Split(" "));
                                var count = fullNames.Count();
                                var firstName = string.Empty;
                                var lastName = fullNames[count - 1].ToString();
                                fullNames.RemoveAt(count - 1);
                                foreach (var item in fullNames)
                                {
                                    firstName = string.Format("{0} {1}", firstName, item);
                                }
                                var fNameProperty = entity.GetType().GetProperty("FirstName");
                                var lastNameProperty = entity.GetType().GetProperty("LastName");
                                if (fNameProperty != null)
                                    fNameProperty.SetValue(entity, firstName.Trim());
                                if (lastNameProperty != null)
                                    lastNameProperty.SetValue(entity, lastName.Trim());
                            }
                        }
                        else if (propertyType == typeof(ulong) || propertyType == typeof(ulong?))
                            cellValue = Convert.ToUInt64(cellValue.ToString() == "Có" ? 1 : 0);
                        else if (propertyType == typeof(Decimal) || propertyType == typeof(Decimal?))
                            cellValue = Convert.ToDecimal(cellValue);
                        else if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
                            cellValue = GetProcessDateTimeValue(entity, cellValue, propertyType, importColumnTemplate);
                        else
                        {
                            cellValue = Convert.ChangeType(cellValue, Nullable.GetUnderlyingType(propertyType));
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Xử lý dữ liệu với các cột được khai báo property có tham chiếu tới 1 table
        /// </summary>
        /// <param name="objectReferenceName">Tên bảng trong CSDL được tham chiếu đến</param>
        /// <param name="cellValue">Giá trị của cell</param>
        /// CreatedBy: NVMANH (12/12/2020)
        protected virtual void ProcessCellValueByDataTypeWhenTableReference<T>(dynamic entity, ref object cellValue, ImportColumn importColumn) where T : BaseEntity
        {
            var value = cellValue.ToString().Trim();
            var objectReferenceName = importColumn.ObjectReferenceName;
            var columnInsert = importColumn.ColumnInsert;
            object objectReference = null;
            switch (objectReferenceName)
            {
                case "Nationality":// Quốc tịch
                    var nationality = Nationalities.Where(n => n.NationalityName == value.ToString().Trim()).FirstOrDefault();
                    if (nationality == null)
                        cellValue = null;
                    else
                    {
                        objectReference = nationality;
                        //var nationalityPropertyByColumnInsert = typeof(Nationality).GetProperty(columnInsert);
                        //if (nationalityPropertyByColumnInsert != null)
                        //    cellValue = nationalityPropertyByColumnInsert.GetValue(nationality);
                        SetCellValueByColumnInsertWhenTableReference(nationality, columnInsert, ref cellValue);
                        var nationalityIdProperty = entity.GetType().GetProperty("NationalityId");
                        var nationalityCodeProperty = entity.GetType().GetProperty("NationalityCode");
                        var nationalityNameProperty = entity.GetType().GetProperty("NationalityName");

                        if (nationalityCodeProperty != null)
                            nationalityCodeProperty.SetValue(entity, nationality.NationalityCode);

                        if (nationalityNameProperty != null)
                            nationalityNameProperty.SetValue(entity, nationality.NationalityName);

                        if (nationalityIdProperty != null)
                            nationalityIdProperty.SetValue(entity, nationality.NationalityId);
                    }
                    break;
                case "Relation":// Quan hệ
                    var relation = Relations.Where(n => n.RelationName == value.ToString().Trim()).FirstOrDefault();
                    if (relation == null)
                        cellValue = null;
                    else
                    {
                        objectReference = relation;
                        SetCellValueByColumnInsertWhenTableReference(relation, columnInsert, ref cellValue);

                        var relationIdProperty = entity.GetType().GetProperty("RelationId");
                        var relationCodeProperty = entity.GetType().GetProperty("RelationCode");
                        var relationNameProperty = entity.GetType().GetProperty("RelationName");

                        if (relationIdProperty != null)
                            relationIdProperty.SetValue(entity, relation.RelationId);

                        if (relationCodeProperty != null)
                            relationCodeProperty.SetValue(entity, relation.RelationCode);

                        if (relationNameProperty != null)
                            relationNameProperty.SetValue(entity, relation.RelationName);
                    }

                    break;
                case "Ethnic":// Dân tộc
                    var ethnic = Ethnics.Where(n => n.EthnicName == value.ToString().Trim()).FirstOrDefault();
                    if (ethnic == null)
                        cellValue = null;
                    else
                    {
                        objectReference = ethnic;
                        SetCellValueByColumnInsertWhenTableReference(ethnic, columnInsert, ref cellValue);

                        var ethnicIdProperty = entity.GetType().GetProperty("EthnicId");
                        var ethnicCodeProperty = entity.GetType().GetProperty("EthnicCode");
                        var ethnicNameProperty = entity.GetType().GetProperty("EthnicName");

                        if (ethnicIdProperty != null)
                            ethnicIdProperty.SetValue(entity, ethnic.EthnicId);

                        if (ethnicCodeProperty != null)
                            ethnicCodeProperty.SetValue(entity, ethnic.EthnicCode);

                        if (ethnicNameProperty != null)
                            ethnicNameProperty.SetValue(entity, ethnic.EthnicName);
                    }
                    break;
                case "Position":// Vị trí/ chức vụ
                    var position = Positions.Where(n => n.PositionName.ToLower() == value.ToString().Trim().ToLower()).FirstOrDefault();
                    // Nếu không có vị trí tương ứng thì thực hiện thêm mới vị trí này (không phải thêm ngay vào db mà đánh dấu để thực hiện thêm sau khi người dùng xác nhận nhập khẩu):
                    // Cơ chế là khởi tạo và add đối tượng mới này vào 1 list và thực hiện lưu lại, sau khi gửi 1 respoonse về cho người dùng xác nhận nhập khẩu thì sẽ thêm vị trí này ở bước sau:
                    if (position == null)
                    {
                        position = new Position()
                        {
                            PositionId = Guid.NewGuid(),
                            PositionName = cellValue.ToString(),
                            OrganizationId = _importRepository.GetCurrentOrganization().OrganizationId,
                        };
                        // Thêm vào danh sách các vị trí sẽ thêm mới:
                        _newPossitons.Add(position);
                    }

                    objectReference = position;
                    SetCellValueByColumnInsertWhenTableReference(position, columnInsert, ref cellValue);
                    var positionIdProperty = entity.GetType().GetProperty("PositionId");
                    var positionCodeProperty = entity.GetType().GetProperty("PositionCode");

                    if (positionIdProperty != null)
                        positionIdProperty.SetValue(entity, position.PositionId);

                    if (positionCodeProperty != null)
                        positionCodeProperty.SetValue(entity, position.PositionCode);

                    break;
                default:// Khác
                    var listData = _importRepository.GetListObjectByTableName(objectReferenceName).Result;
                    var iDPropertyName = String.Format("{0}Id", objectReferenceName);
                    var namePropertyName = String.Format("{0}Name", objectReferenceName);
                    var codePropertyName = String.Format("{0}Code", objectReferenceName);
                    if (listData != null && listData.First().GetType().GetProperty(namePropertyName) != null)
                    {
                        List<object> objectReferences = new List<object>();
                        // Tìm chính xác:
                        objectReference = listData.Where(e => e.GetType().GetProperty(namePropertyName).GetValue(e, null).ToString().ToLower() == value.ToLower()).FirstOrDefault();
                        // Tìm gần đúng:
                        if (objectReference == null)
                        {
                            objectReference = listData.Where(e =>
                             RemoveDiacritics(e.GetType().GetProperty(namePropertyName).GetValue(e, null).ToString()).Contains(RemoveDiacritics(value)) ||
                             RemoveDiacritics(value).Contains(RemoveDiacritics(e.GetType().GetProperty(namePropertyName).GetValue(e, null).ToString())))
                            .FirstOrDefault();
                        }

                        if (objectReference == null)
                        {
                            (entity as BaseEntity).ImportValidState = ImportValidState.Invalid;
                            (entity as BaseEntity).ImportValidError.Add(String.Format(@"Thông tin [{0}] ('{1}') nhập trên tệp không tồn tại trên hệ thống.", importColumn.ColumnTitle, value));
                            return;
                        }
                        SetCellValueByColumnInsertWhenTableReference(objectReference, columnInsert, ref cellValue);

                        var idProperty = typeof(T).GetProperty(iDPropertyName);
                        var codeProperty = typeof(T).GetProperty(codePropertyName);
                        var nameProperty = typeof(T).GetProperty(namePropertyName);

                        if (idProperty != null)
                            idProperty.SetValue(entity, objectReference.GetType().GetProperty(iDPropertyName).GetValue(objectReference));

                        if (codeProperty != null)
                            codeProperty.SetValue(entity, objectReference.GetType().GetProperty(codePropertyName).GetValue(objectReference));

                        if (nameProperty != null)
                            nameProperty.SetValue(entity, objectReference.GetType().GetProperty(namePropertyName).GetValue(objectReference));
                    }
                    break;
            }
            CustomAfterSetCellValueByColumnInsertWhenTableReference(entity, objectReference, columnInsert, ref cellValue);
        }

        /// <summary>
        /// Set giá trị cho cell theo thông tin cột dữ liệu nhập khẩu được khai báo trong db
        /// </summary>
        /// <typeparam name="Y">Kiểu của đối tượng</typeparam>
        /// <param name="entity">thực thể đang build từ 1 dòng trong file excel</param>
        /// <param name="columnInsert">Thông tin cột đang nhập khẩu</param>
        /// <param name="cellValue">Giá trị ô trong file excel đang đọc được</param>
        /// CreatedBy: NVMANH (12/12/2020)
        private void SetCellValueByColumnInsertWhenTableReference<Y>(Y objectReference, string columnInsert, ref object cellValue)
        {
            var propertyByColumnInsert = objectReference.GetType().GetProperty(columnInsert ?? string.Empty, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
            if (propertyByColumnInsert != null)
                cellValue = propertyByColumnInsert.GetValue(objectReference);
        }

        /// <summary>
        /// Hàm tùy chỉnh theo nhu cầu khi muốn custom lại giá trị đọc được từ ô excel với kiểu được map tương ứng trong database là Enum
        /// </summary>
        /// <typeparam name="Y">Kiểu của đối tượng</typeparam>
        /// <param name="entity">thực thể đang build từ 1 dòng trong file excel</param>
        /// <param name="enumType">Kiểu enum (VD Gender)</param>
        /// <param name="columnInsert">Thông tin cột đang nhập khẩu</param>
        /// <param name="cellValue">Giá trị ô trong file excel đang đọc được</param>
        /// CreatedBy: NVMANH (12/12/2020)
        protected virtual void CustomAfterSetCellValueByColumnInsertWhenEnumReference<Y>(object entity, Y enumType, string columnInsert, ref object cellValue)
        {

        }


        /// <summary>
        /// Hàm tùy chỉnh theo nhu cầu khi muốn custom lại giá trị đọc được từ ô excel với khi giá trị được chọn thuộc 1 list trong danh mục được lưu trữ ở Database
        /// </summary>
        /// <typeparam name="Y">Kiểu của đối tượng</typeparam>
        /// <param name="entity">thực thể đang build từ 1 dòng trong file excel</param>
        /// <param name="columnInsert">Thông tin cột đang nhập khẩu</param>
        /// <param name="cellValue">Giá trị ô trong file excel đang đọc được</param>
        /// CreatedBy: NVMANH (12/12/2020)
        protected virtual void CustomAfterSetCellValueByColumnInsertWhenTableReference<Y>(object entity, Y objectReference, string columnInsert, ref object cellValue)
        {

        }

        /// <summary>
        /// Validate dữ liệu trong cell (check bắt buộc nhập, check định dạng...)
        /// </summary>
        /// <param name="entity">Chi tiết hồ sơ nhập khẩu</param>
        /// <param name="cellValue">Giá trị của cell</param>
        /// <param name="importColumn">Thông tin cột nhập khẩu được khai báo trong database</param>
        /// CreatedBy: NVMANH (24/05/2020)
        public virtual void CellValueValid<T>(T entity, object cellValue, ImportColumn importColumn) where T : BaseEntity
        {
            var isRequired = importColumn.IsRequired;
            // Kiếm tra bắt buộc nhập:
            if (isRequired == 1 && cellValue == null)
            {
                entity.ImportValidState = ImportValidState.Invalid;
                entity.ImportValidError.Add(string.Format("Thông tin {0} không được để trống", importColumn.ColumnTitle));
            }
        }

        /// <summary>
        /// Check trùng dữ liệu
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitiesInFile">thực thể được build từ Excel</param>
        /// <param name="entity">Thực thể</param>
        /// <param name="cellValue">Giá trị của cell lấy từ file Excel</param>
        /// <param name="importColumn">Thông tin cột import được lấy trong database</param>
        /// CreatedBy: NVMANH (15/05/2020)
        protected virtual void CheckDuplicateData<T>(List<T> entitiesInFile, T entity, object cellValue, ImportColumn importColumn) where T : BaseEntity
        {
            // Validate: kiểm tra trùng dữ liệu trong File Excel:
            if (importColumn.ColumnInsert == "CitizenIdentityNo" && cellValue != null)
            {
                var itemDuplicate = entitiesInFile.Where(item => item.GetType().GetProperty("CitizenIdentityNo").GetValue(item).ToString() == cellValue.ToString()).FirstOrDefault();
                if (itemDuplicate != null)
                {
                    entity.GetType().GetProperty("ImportValidState").SetValue(entity, ImportValidState.DuplicateInFile);
                    entity.ImportValidError.Add(string.Format(Resources.Error_ImportDataDuplicateInFile, itemDuplicate.GetType().GetProperty("FullName").GetValue(itemDuplicate).ToString()));
                }
            }
        }

        /// <summary>
        /// Xử lý dữ liệu liên quan đến ngày/ tháng
        /// </summary>
        /// <param name="entity">Thực thế sẽ import vào Db</param>
        /// <param name="cellValue">Giá trị của cell</param>
        /// <param name="type">Kiểu dữ liệu</param>
        /// <param name="importColumn">Thông tin cột import được khai báo trong Db</param>
        /// <returns>giá trị ngày tháng được chuyển đổi tương ứng</returns>
        /// CreatedBy: NVMANH (25/05/2020)
        protected virtual DateTime? GetProcessDateTimeValue<T>(T entity, object cellValue, Type type, ImportColumn importColumn = null) where T : BaseEntity
        {
            DateTime? dateReturn = null;
            if (cellValue.GetType() == typeof(double))
                return DateTime.FromOADate((double)cellValue);
            var dateString = cellValue.ToString();
            // Ngày tháng phải nhập theo định dạng (ngày/tháng/năm): 
            // VD hợp lệ: [25.04.2017] [02.04.2017] [2.4.2017] [25/04/2017] [5/12/2017] [15/2/2017] [25-04-2017]  [6-10-2017]  [16-5-2017] [09/26/2000 12:00:00 AM]  [09/26/2000 12:00:00 PM] 
            Regex dateValidRegex = new Regex(@"^([0]?[1-9]|[1|2][0-9]|[3][0|1])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$");
            Regex dateValidRegexWithTime = new Regex(@"^([0]?[1-9]|[1][0-2])[/]([0]?[1-9]|[1|2][0-9]|[3][0|1])[/]([0-9]{4}|[0-9]{2}) ([0]|[1])([0-9])[:]([0]|[1])([0-9])[:]([0]|[1])([0-9]) ([AM]|[PM])$");

            if (dateValidRegex.IsMatch(dateString))
            {
                var dateSplit = dateString.Split(new string[] { "/", ".", "-" }, StringSplitOptions.None);
                var day = int.Parse(dateSplit[0]);
                var month = int.Parse(dateSplit[1]);
                var year = int.Parse(dateSplit[2]);
                dateReturn = new DateTime(year, month, day);
            }
            else if (dateValidRegexWithTime.IsMatch(dateString))
            {
                dateReturn = DateTime.Parse(dateString);
            }
            else if (DateTime.TryParse(cellValue.ToString(), out DateTime dateTime) == true)
            {
                dateReturn = dateTime;
            }
            else
            {
                entity.ImportValidState = ImportValidState.Invalid;
                entity.ImportValidError.Add(string.Format("Thông tin [{0}] không đúng định dạng.", importColumn.ColumnTitle));
            }
            return dateReturn;
        }

        /// <summary>
        /// Hàm chuyển các ký tự unicode thành ký tự không dấu, viết liền và viết thường (mục đích để compare gần đúng 2 chuỗi ký tự)
        /// </summary>
        /// <param name="text">Chuỗi ký tự</param>
        /// <returns>Chuỗi ký tự đã loại bỏ dấu và lowercase - phục vụ check map tương đối nội dung của text</returns>
        /// CreatedBy: NVMANH (23/04/2020)
        private string RemoveDiacritics(string text)
        {
            var newText = string.Concat(
                text.Normalize(NormalizationForm.FormD)
                .Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) !=
                                              UnicodeCategory.NonSpacingMark)
              ).Normalize(NormalizationForm.FormC);
            return newText.Replace(" ", string.Empty).ToLower();
        }
        #endregion
    }
}
