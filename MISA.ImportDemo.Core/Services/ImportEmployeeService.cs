using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using MISA.ImportDemo.Core.Entities;
using MISA.ImportDemo.Core.Enumeration;
using MISA.ImportDemo.Core.Interfaces;
using MISA.ImportDemo.Core.Interfaces.Repository;
using MISA.ImportDemo.Core.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Core.Services
{
    /// <summary>
    /// Service xử lý việc nhập khẩu nhân viên
    /// </summary>
    /// CreatedBy: NVMANH (10/10/2020)
    public class ImportEmployeeService : BaseImportService, IImportEmployeeService
    {
        #region DECLARE
        #endregion
        #region CONSTRUCTOR
        public ImportEmployeeService(IImportEmployeeRepository importRepository, IMemoryCache importMemoryCache) : base(importRepository, importMemoryCache, "Employee")
        {
            //EntitiesFromDatabase = GetListProfileBookDetailsByProfileBookId().Cast<object>().ToList();
        }
        #endregion

        #region METHOD
        /// <summary>
        /// Thực hiện nhập khẩu dữ liệu
        /// </summary>
        /// <param name="keyImport">Key xác định lấy dữ liệu để nhập khẩu từ cache</param>
        /// <param name="overriderData">Có cho phép ghi đè hay không (true- ghi đè dữ liệu trùng lặp trong db)</param>
        /// <param name="cancellationToken">Tham số tùy chọn xử lý đa luồng (hiện tại chưa sử dụng)</param>
        /// <returns>ActionServiceResult(với các thông tin tương ứng tùy thuộc kết nhập khẩu)</returns>
        /// CreatedBy: NVMANH (10/10/2020)
        public async Task<ActionServiceResult> Import(string keyImport, bool overriderData, CancellationToken cancellationToken)
        {
            return await _importRepository.Import(keyImport, overriderData, cancellationToken);
        }

        /// <summary>
        /// Thực hiện đọc dữ liệu từ tệp nhập khẩu
        /// </summary>
        /// <param name="importFile">Tệp nhập khẩu</param>
        /// <param name="cancellationToken">Tham số tùy chọn sử dụng xử lý Task đa luồng</param>
        /// <returns>ActionServiceResult(với các thông tin tương ứng tùy thuộc kết quả đọc tệp)</returns>
        /// CreatedBy: NVMANH (10/10/2020)
        public async Task<ActionServiceResult> ReadEmployeeDataFromExcel(IFormFile importFile, CancellationToken cancellationToken)
        {
            // Lấy dữ liệu nhân viên trên Db về để thực hiện check trùng:
            EntitiesFromDatabase = (await GetEmployeesFromDatabase()).Cast<object>().ToList();
            var employees = await base.ReadDataFromExcel<Employee>(importFile, cancellationToken);
            var importInfo = new ImportInfo(String.Format("EmployeeImport_{0}", Guid.NewGuid()), employees);
            // Lưu dữ liệu vào cache:
            importMemoryCache.Set(importInfo.ImportKey, employees);
            // Lưu các vị trí mới vào cache:
            importMemoryCache.Set(string.Format("Position_{0}", importInfo.ImportKey), _newPossitons);
            return new ActionServiceResult(true, Resources.Msg_ImportFileReadSuccess, MISACode.Success, importInfo);
        }

        /// <summary>
        ///  Lấy toàn bộ danh sách Nhân viên đang có trong Database theo từng công ty.
        ///  với bộ hồ sơ (ProfileBook) đang nhập khẩu vào - lưu vào cache để thực hiện check trùng
        /// </summary>
        /// CreatedBy: NVMANH (02/06/2020)
        private async Task<List<Employee>> GetEmployeesFromDatabase()
        {
            var importRepository = _importRepository as IImportEmployeeRepository;
            return await importRepository.GetEmployees();

        }

        /// <summary>
        /// Check trùng dữ liệu trong File Excel và trong database, dựa vào số chứng minh thư
        /// </summary>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <param name="entitiesInFile">Danh sách các đối tượng được build từ tệp nhập khẩu</param>
        /// <param name="entity">thực thể hiện tại</param>
        /// <param name="cellValue">Giá trị nhập trong ô excel đang đọc</param>
        /// <param name="importColumn">Thông tin cột nhập khẩu (tiêu đề cột, kiểu giá trị....)</param>
        /// CreatedBy: NVMANH (19/06/2020)
        protected override void CheckDuplicateData<T>(List<T> entitiesInFile, T entity, object cellValue, ImportColumn importColumn)
        {
            if (entity is Employee)
            {
                var newEmployee = entity as Employee;
                // Validate: kiểm tra trùng dữ liệu trong File Excel và trong Database: check theo số CMTND
                if (importColumn.ColumnInsert == "CitizenIdentityNo" && cellValue != null)
                {
                    var citizenIndentityNo = cellValue.ToString().Trim();
                    // Check trong File
                    var itemDuplicate = entitiesInFile.Where(item => (item.GetType().GetProperty("CitizenIdentityNo").GetValue(item) ?? string.Empty).ToString() == citizenIndentityNo).FirstOrDefault();
                    if (itemDuplicate != null)
                    {
                        entity.ImportValidState = ImportValidState.DuplicateInFile;
                        itemDuplicate.ImportValidState = ImportValidState.DuplicateInFile;
                        entity.ImportValidError.Add(string.Format(Resources.Error_ImportDataDuplicateInFile, entity.GetType().GetProperty("FullName").GetValue(entity).ToString()));
                        itemDuplicate.ImportValidError.Add(string.Format(Resources.Error_ImportDataDuplicateInFile, itemDuplicate.GetType().GetProperty("FullName").GetValue(itemDuplicate).ToString()));
                    }
                    // Check trong Db:
                    var itemDuplicateInDb = EntitiesFromDatabase.Where(item => (item.GetType().GetProperty("CitizenIdentityNo").GetValue(item) ?? string.Empty).ToString() == citizenIndentityNo).Cast<T>().FirstOrDefault();
                    if (itemDuplicateInDb != null)
                    {
                        entity.ImportValidState = ImportValidState.DuplicateInDb;
                        newEmployee.EmployeeId = (Guid)itemDuplicateInDb.GetType().GetProperty("EmployeeId").GetValue(itemDuplicateInDb);
                        itemDuplicateInDb.ImportValidState = ImportValidState.DuplicateInFile;
                        entity.ImportValidError.Add(string.Format(Resources.Error_ImportDataDuplicateInDatabase, entity.GetType().GetProperty("FullName").GetValue(entity).ToString()));
                        itemDuplicateInDb.ImportValidError.Add(string.Format(Resources.Error_ImportDataDuplicateInDatabase, itemDuplicateInDb.GetType().GetProperty("FullName").GetValue(itemDuplicateInDb).ToString()));
                    }
                }
            }
            else
            {
                base.CheckDuplicateData(entitiesInFile, entity, cellValue, importColumn);
            }
        }

        /// <summary>
        /// Khởi tạo đối tượng trước khi build các thông tin
        /// Dựa vào thông tin bảng dữ liệu sẽ import dữ liệu vào mà map các đối tượng tương ứng.
        /// </summary>
        /// <typeparam name="T">Kiểu của object</typeparam>
        /// <returns>Thực thể được khởi tạo với kiểu tương ứng</returns>
        /// OverriderBy: NVMANH (15/12/2020)
        protected override dynamic InstanceEntityBeforeMappingData<T>()
        {
            var ImportToTable = ImportWorksheetTemplate.ImportToTable;
            switch (ImportToTable)
            {
                case "Employee":
                    var newEntity = new Employee();
                    newEntity.EmployeeId = Guid.NewGuid();
                    return newEntity;
                case "EmployeeFamily":
                    var eFamily = new EmployeeFamily()
                    {
                        EmployeeFamilyId = Guid.NewGuid()
                    }; //Activator.CreateInstance("MISA.ImportDemo.Core.Entities", "ProfileFamilyDetail");
                    return eFamily;
                default:
                    return base.InstanceEntityBeforeMappingData<T>();
            }
        }

        /// <summary>
        /// Sau khi các thông tin được build hoàn chỉnh thì làm một số việc cần thiết
        /// 1. Mapping dữ liệu thông tin thành viên gia đình tương ứng với nhân viên nào
        /// 2. Validate có lỗi gì cụ thể
        /// </summary>
        /// <typeparam name="T">kiểu của object</typeparam>
        /// <param name="entity">object thành viên trong gia đình</param>
        /// OverriderBy: NVMANH (15/12/2020)
        protected override void ProcessDataAfterBuild<T>(object entity)
        {
            if (entity is EmployeeFamily)
            {
                var employeeFamily = entity as EmployeeFamily;
                var sort = employeeFamily.Sort;
                var employeeMaster = _entitiesFromEXCEL.Cast<Employee>().Where(pbd => pbd.Sort == sort).FirstOrDefault();
                if (employeeMaster != null && sort != null)
                {
                    employeeFamily.EmployeeId = employeeMaster.EmployeeId;
                    employeeMaster.EmployeeFamily.Add(employeeFamily);

                    // Duyệt từng lỗi của detail và add thông tin vào master:
                    foreach (var importValidError in employeeFamily.ImportValidError)
                    {
                        employeeMaster.ImportValidError.Add(String.Format("Thông tin thành viên trong gia đình: {0} - {1}", employeeFamily.FullName, importValidError));
                    }

                    // Nếu master không có lỗi valid, detail có thì gán lại cờ cho master là invalid:
                    if (employeeFamily.ImportValidState != ImportValidState.Valid && employeeMaster.ImportValidState == ImportValidState.Valid)
                        employeeMaster.ImportValidState = ImportValidState.Invalid;
                }
            }
            base.ProcessDataAfterBuild<T>(entity);
        }

        /// <summary>
        /// Xử lý đặc thù với các thông tin lấy trên tệp nhập khẩu ở dạng lựa chọn thông tin ở 1 danh mục trong database
        /// </summary>
        /// <typeparam name="T">kiểu của thực thể đang build</typeparam>
        /// <param name="entity">thực thể</param>
        /// <param name="cellValue">giá trị của cell đọc được trên tệp</param>
        /// <param name="importColumn">thông tin cột nhập khẩu hiện tại được khai báo trong databse</param>
        /// OverriderBy: NVMANH (15/12/2020)
        protected override void ProcessCellValueByDataTypeWhenTableReference<T>(object entity, ref object cellValue, ImportColumn importColumn)
        {
            var value = cellValue;
            if (importColumn.ObjectReferenceName == "ParticipationForm" && entity is Employee)
            {
                //var listData = _importRepository.GetListObjectByTableName("ParticipationForm").Result.Cast<ParticipationForm>().ToList();
                //var par = listData.Where(e => e.Rate == decimal.Parse(value.ToString().Replace(",","."))).FirstOrDefault();
                //if (par == null)
                //    return;
                //(entity as Employee).ParticipationFormId = par.ParticipationFormId;
                //(entity as Employee).ParticipationFormName = par.ParticipationFormName;
            }
            else
            {
                base.ProcessCellValueByDataTypeWhenTableReference<T>(entity, ref cellValue, importColumn);
            }
        }

        /// <summary>
        /// Xử lý đặc thù với các thông tin lấy trên tệp nhập khẩu ở dạng lựa chọn thông tin lưu trữ có kiểu là Enum (VD giới tính, tình trạng hôn nhân...)
        /// </summary>
        /// <typeparam name="T">kiểu của thực thể đang build</typeparam>
        /// <param name="entity">thực thể</param>
        /// <param name="enumType">Kiểu của enum</param>
        /// <param name="cellValue">giá trị của cell đọc được trên tệp</param>
        /// <param name="importColumn">thông tin cột nhập khẩu hiện tại được khai báo trong databse</param>
        /// OverriderBy: NVMANH (15/12/2020)
        protected override void CustomAfterSetCellValueByColumnInsertWhenEnumReference<Y>(object entity, Y enumType, string columnInsert, ref object cellValue)
        {
            if (columnInsert == "ResidentialAreaType")
            {
                //var employee = entity as Employee;
                //var enumPropertyName = (ResidentialAreaType)cellValue;
                //employee.ResidentialAreaName = Resources.ResourceManager.GetString(string.Format("Enum_ResidentialAreaType_{0}", enumPropertyName));
            }
            else
            {
                base.CustomAfterSetCellValueByColumnInsertWhenEnumReference<Y>(entity, enumType, columnInsert, ref cellValue);
            }

        }

        /// <summary>
        /// Sau khi xử lý dữ liệu với các giá trị được chọn trong 1 danh mục ở Database thì tùy chỉnh lại nếu có nhu cầu
        /// </summary>
        /// <typeparam name="Y">Kiểu của đối tượng</typeparam>
        /// <param name="entity">thực thể đang build từ 1 dòng trong file excel</param>
        /// <param name="columnInsert">Thông tin cột đang nhập khẩu</param>
        /// <param name="cellValue">Giá trị ô trong file excel đang đọc được</param>
        /// CreatedBy: NVMANH (12/12/2020)
        protected override void CustomAfterSetCellValueByColumnInsertWhenTableReference<Y>(object entity, Y objectReference, string columnInsert, ref object cellValue)
        {
            // Gán lại chính xác thông tin liên hệ của thành viên có mối quan hệ là gì?
            if (objectReference is Relation && entity is EmployeeFamily)
            {
                var pfd = entity as EmployeeFamily;
                pfd.RelationId = (objectReference as Relation).RelationId;
            }
            else
                base.CustomAfterSetCellValueByColumnInsertWhenTableReference(entity, objectReference, columnInsert, ref cellValue);
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
        protected override DateTime? GetProcessDateTimeValue<T>(T entity, object cellValue, Type type, ImportColumn importColumn = null)
        {
            if (entity is EmployeeFamily && importColumn.ColumnInsert == "DateOfBirth")
            {
                var empFamily = entity as EmployeeFamily;
                var dateDisplaySetting = (DateDisplaySetting)empFamily.DobdisplaySetting;
                DateTime? dateOfBirth = null;
                var dateString = cellValue.ToString();
                switch (dateDisplaySetting)
                {
                    case DateDisplaySetting.mmyyyy:
                        Regex dateValidRegex = new Regex(@"^([0]?[1-9]|[1][0-2])[./-]([0-9]{4})$");
                        if (dateValidRegex.IsMatch(dateString))
                        {
                            var dateSplit = dateString.Split(new string[] { "/", ".", "-" }, StringSplitOptions.None);
                            var month = int.Parse(dateSplit[0]);
                            var year = int.Parse(dateSplit[1]);
                            dateOfBirth = new DateTime(year, month, 1);
                        }
                        else
                        {
                            dateOfBirth = base.GetProcessDateTimeValue(entity, cellValue, type, importColumn);
                            //entity.ImportValidState = ImportValidState.Invalid;
                            //entity.ImportValidError.Add(string.Format("Thông tin [{0}] không đúng định dạng.", importColumn.ColumnTitle));
                        }
                        break;
                    case DateDisplaySetting.yyyy:
                        Regex yearValidRegex = new Regex(@"^([0-9]{4})$");
                        if (yearValidRegex.IsMatch(dateString))
                        {
                            var year = int.Parse(dateString);
                            dateOfBirth = new DateTime(year, 1, 1);
                        }
                        else
                        {
                            dateOfBirth = base.GetProcessDateTimeValue(entity, cellValue, type, importColumn);
                            //entity.ImportValidState = ImportValidState.Invalid;
                            //entity.ImportValidError.Add(string.Format("Thông tin [{0}] không đúng định dạng.", importColumn.ColumnTitle));
                        }
                        break;
                    default:
                        dateOfBirth = base.GetProcessDateTimeValue(entity, cellValue, type, importColumn);
                        break;
                }
                return dateOfBirth;
            }
            else
                return base.GetProcessDateTimeValue(entity, cellValue, type, importColumn);
        }
        #endregion
    }
}
