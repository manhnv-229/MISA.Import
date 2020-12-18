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
    public class ImportEmployeeService : BaseImportService, IImportEmployeeService
    {
        public ImportEmployeeService(IImportEmployeeRepository importRepository, IMemoryCache importMemoryCache) : base(importRepository, importMemoryCache, "Employee")
        {
            //EntitiesFromDatabase = GetListProfileBookDetailsByProfileBookId().Cast<object>().ToList();
        }

        public async Task<ActionServiceResult> Import(string keyImport, bool overriderData, CancellationToken cancellationToken)
        {
            return await _importRepository.Import(keyImport, overriderData, cancellationToken);
        }

        public async Task<ActionServiceResult> ReadEmployeeDataFromExcel(IFormFile importFile, CancellationToken cancellationToken)
        {
            // Lấy dữ liệu nhân viên trên Db về để thực hiện check trùng:
            EntitiesFromDatabase = (await GetEmployeesFromDatabase()).Cast<object>().ToList();
            var employees = await base.ReadDataFromExcel<Employee>(importFile, cancellationToken);
            var importInfo = new ImportInfo(String.Format("EmployeeImport_{0}", Guid.NewGuid()), employees);
            // Lưu dữ liệu vào cache:
            importMemoryCache.Set(importInfo.ImportKey, employees);
            // Lưu các vị trí mới vào cache:
            importMemoryCache.Set(string.Format("Position_{0}",importInfo.ImportKey), _newPossitons);
            return new ActionServiceResult(true, Resources.Msg_ImportFileReadSuccess, MISACode.Success, importInfo);
        }

        /// <summary>
        ///  Lấy dữ liệu chi tiết các hồ sơ (ProfileBookDetail) đã có trong Database tương ứng 
        ///  với bộ hồ sơ (ProfileBook) đang nhập khẩu vào - lưu vào cache để thực hiện check trùng
        /// </summary>
        /// CreatedBy: NVMANH (02/06/2020)
        private async Task<List<Employee>> GetEmployeesFromDatabase()
        {
            var importRepository = _importRepository as IImportEmployeeRepository;
            //ISpecification<ProfileBookDetail> spec = new ProfileBookDetailSpecification(_profileBookId);
            return await importRepository.GetEmployees();

        }

        /// <summary>
        /// Check trùng dữ liệu trong File Excel và trong database, dựa vào số chứng minh thư
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitiesInFile"></param>
        /// <param name="entity"></param>
        /// <param name="cellValue"></param>
        /// <param name="importColumn"></param>
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

        protected override void CustomAfterSetCellValueByColumnInsertWhenTableReference<Y>(object entity, Y objectReference, string columnInsert, ref object cellValue)
        {
            if (objectReference is Relation && entity is EmployeeFamily)
            {
                var pfd = entity as EmployeeFamily;
                pfd.RelationId = (objectReference as Relation).RelationId;
            }
            else
                base.CustomAfterSetCellValueByColumnInsertWhenTableReference(entity, objectReference, columnInsert, ref cellValue);
        }

        /// <summary>
        /// Xử lý dữ liệu đặc thù đối với ngày sinh (thông tin thành viên trong gia đình)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="cellValue"></param>
        /// <param name="type"></param>
        /// <param name="importColumn"></param>
        /// <returns></returns>
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
    }
}
