using MISA.ImportDemo.Core.Properties;
using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.ImportDemo.Core.Enumeration
{

    /// <summary>
    /// Tên kiểu store sẽ thực thi
    /// </summary>
    /// CreatedBy: NVMANH (14/04/2020)
    public enum ProcdureTypeName
    {
        /// <summary>
        ///  Lấy dữ liệu
        /// </summary>
        Get,

        /// <summary>
        /// Lấy dữ liệu theo khóa chính
        /// </summary>
        GetById,

        /// <summary>
        /// Thêm mới
        /// </summary>
        Insert,

        /// <summary>
        /// Sửa/ cập nhật dữ liệu
        /// </summary>
        Update,

        /// <summary>
        /// Xóa dữ liệu
        /// </summary>
        Delete,

        /// <summary>
        /// Lấy dữ liệu có phân trang
        /// </summary>
        GetPaging
    }

    /// <summary>
    /// Kiểu phương thức 
    /// </summary>
    public enum EntityState
    {
        /// <summary>
        /// Lấy dữ liệu
        /// </summary>
        GET,

        /// <summary>
        /// Thêm mới dữ liệu
        /// </summary>
        INSERT,

        /// <summary>
        /// Sửa dữ liệu
        /// </summary>
        UPDATE,

        /// <summary>
        /// Xóa dữ liệu
        /// </summary>
        DELETE
    }

    /// <summary>
    /// Các mã lỗi
    /// </summary>
    public enum MISACode
    {
        Success = 200,
        /// <summary>
        /// Lỗi validate dữ liệu chung
        /// </summary>
        Validate = 400,

        /// <summary>
        /// Lỗi validate dữ liệu không hợp lệ
        /// </summary>
        ValidateEntity = 401,

        /// <summary>
        /// Lỗi validate dữ liệu do không đúng nghiệp vụ
        /// </summary>
        ValidateBussiness = 402,

        /// <summary>
        /// Lỗi Exception
        /// </summary>
        Exception = 500,

        /// <summary>
        /// Lỗi File không đúng định dạng
        /// </summary>
        FileFormat = 600,

        /// <summary>
        /// File trống
        /// </summary>
        FileEmpty = 601,
        /// <summary>
        /// Lỗi File import không đúng định dạng
        /// </summary>
        ImportFileFormat = 602,

        /// <summary>
        /// Lỗi File Export không đúng định dạng
        /// </summary>
        ExportFileFormat = 603
    }

    /// <summary>
    /// Loại Import - xác định sẽ đối tượng sẽ Import.
    /// </summary>
    public enum ObjectImport
    {
        EProfileBookDetail = 1,
        Employee = 2
    }

    /// <summary>
    /// Enum xác định kết quả check dữ liệu nhập khẩu.
    /// </summary>
    public enum ImportValidState
    {
        /// <summary>
        /// Hợp lệ
        /// </summary>
        Valid = 1,

        /// <summary>
        /// Khôp hợp lệ
        /// </summary>
        Invalid = 2,

        /// <summary>
        /// Trùng lặp trong File
        /// </summary>
        DuplicateInFile = 3,

        /// <summary>
        /// Trùng lặp trong Database:
        /// </summary>
        DuplicateInDb = 4
    }

    /// <summary>
    /// Các kiểu dữ liệu
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// Chuỗi
        /// </summary>
        String = 0,

        /// <summary>
        /// Số nguyên
        /// </summary>
        Int = 1,

        /// <summary>
        /// True/ False
        /// </summary>
        Boolean = 2,

        /// <summary>
        /// Enum
        /// </summary>
        Enum = 3,

        /// <summary>
        /// Tham chiếu tới bảng dữ liệu xác định trong Database
        /// </summary>
        ReferenceTable = 4
    }

    /// <summary>
    /// Enum giới tính
    /// </summary>
    public enum Gender
    {
        /// <summary>
        /// Nam
        /// </summary>
        Male = 1,

        /// <summary>
        /// Nữ
        /// </summary>
        Female = 0,

        /// <summary>
        /// Không xác định
        /// </summary>
        Other = 2
    }

    /// <summary>
    /// Kiểu hiển thị dữ liệu ngày tháng
    /// </summary>
    public enum DateDisplaySetting
    {
        /// <summary>
        /// Ngày/ tháng/ năm
        /// </summary>
        ddmmyyyy = 0,

        /// <summary>
        /// Tháng/năm
        /// </summary>
        mmyyyy = 1,

        /// <summary>
        /// năm
        /// </summary>
        yyyy = 2
    }

    public enum YesNoOption
    {
        No = 0,
        Yes = 1
    }


    public enum ResidentialAreaType
    {
        K1=1,
        K2=2,
        K3=3
    }

    /// <summary>
    /// Trạng thái tham gia BHXH
    /// </summary>
    public enum InsurranceStatus
    {
        NotParticipating = 0,// Không tham gia
        Participating = 1 // Đang tham gia
    }


    /// <summary>
    /// Loại hợp đồng lao động
    /// </summary>
    public enum ContractType
    {
        Limited,
        Unlimitted,
        Other
    }
}
