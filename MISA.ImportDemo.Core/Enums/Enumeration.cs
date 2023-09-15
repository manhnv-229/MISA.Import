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

    /// <summary>
    /// Loại sắp xếp
    /// </summary>
    public enum SortType
    {
        /// <summary>
        /// Tăng dần
        /// </summary>
        ASC = 1,

        /// <summary>
        /// Giảm dần
        /// </summary>
        DESC = 2
    }

    /// <summary>
    /// Loại lọc dữ liệu
    /// </summary>
    public enum FilterType
    {
        /// <summary>
        /// Chứa
        /// </summary>
        Containt = 1,

        /// <summary>
        /// Không chứa
        /// </summary>
        NotContaint = 2,

        /// <summary>
        /// Bắt đầu với
        /// </summary>
        StartWith = 3,

        /// <summary>
        /// Kết thúc với
        /// </summary>
        EndWith = 4,

        /// <summary>
        /// Bằng
        /// </summary>
        Equal = 5,

        /// <summary>
        /// Không bằng
        /// </summary>
        NotEqual = 6
    }

    public enum MSRole
    {
        Administrator = 1, // Quản trị
        Management = 5, // Quản lý
        Employee = 10, // Nhân viên
        Teacher = 15, // Giáo viên, Giảng viên
        Advisor = 20, // Quản lý nhóm
        HRIntern = 21, // Intern nhân sự
        Fresher = 25, // Fresher
        Intern = 30, // Intert lập trình
        Newbie = 35, // Thành viên mới
    }

    /// <summary>
    /// Trạng thái của Entity
    /// </summary>
    public enum MISAEntityState
    {
        /// <summary>
        /// Thêm mới
        /// </summary>
        AddNew = 1,

        /// <summary>
        /// Cập nhật
        /// </summary>
        Update = 2,

        DELETE = 3,
    }

    /// <summary>
    /// Tình trạng hôn nhân
    /// </summary>
    public enum MaritalStatus
    {
        /// <summary>
        /// Độc thân
        /// </summary>
        Single = 1,

        /// <summary>
        /// Đã đính hôn
        /// </summary>
        Engaged = 2,

        /// <summary>
        /// Đã kết hôn
        /// </summary>
        Married = 3,

        /// <summary>
        /// Ly thân
        /// </summary>
        Separated = 4,

        /// <summary>
        /// Ly hôn
        /// </summary>
        Divorced = 5,

        /// <summary>
        /// Góa phụ/ Góa chông
        /// </summary>
        Widow = 6,

        /// <summary>
        /// Góa vợ
        /// </summary>
        Widower = 7
    }

    /// <summary>
    /// Tình trạng công việc
    /// </summary>
    public enum WorkStatus
    {
        /// <summary>
        /// Đang làm việc
        /// </summary>
        Working = 1,

        /// <summary>
        /// Đang học việc
        /// </summary>
        Trainning = 2,

        /// <summary>
        /// Đang thử việc
        /// </summary>
        Probationary = 3,

        /// <summary>
        /// Đã nghỉ việc
        /// </summary>
        QuitWork = 4,

        /// <summary>
        /// Đã nghỉ hưu
        /// </summary>
        Retired = 5
    }

    /// <summary>
    /// Bậc đào tạo
    /// </summary>
    public enum TrainningLevel
    {
        /// <summary>
        /// Cao học
        /// </summary>
        Master = 1,

        /// <summary>
        /// Đại học
        /// </summary>
        Bachelor = 2,

        /// <summary>
        /// Nghiên cứu sinh
        /// </summary>
        PhilosophyDoctor = 3,

        /// <summary>
        /// Cao đẳng
        /// </summary>
        Diploma = 4,

        /// <summary>
        /// Trung cấp
        /// </summary>
        IntermediateProfessionalEducation = 5
    }

    /// <summary>
    /// Xếp loại bằng tốt nghiệp
    /// </summary>
    public enum DegreeClassification
    {
        /// <summary>
        /// Xuất sắc
        /// </summary>
        Excellent = 1,

        /// <summary>
        /// Giỏi
        /// </summary>
        VeryGood = 2,

        /// <summary>
        /// Khá
        /// </summary>
        Good = 3,

        /// <summary>
        /// Trung bình/ Khá
        /// </summary>
        AverageGood = 4,

        /// <summary>
        /// Trung bình
        /// </summary>
        Ordinary = 5
    }

    public enum TokenLimitTime
    {
        Day = 1,
        Hour = 2,
        Minute = 3,
        Second = 4
    }

    public enum MessageGroupType
    {
        Private = 1,
        Group = 2,
        Public = 3
    }

    public enum ExerciseType
    {
        Constructed = 1,
        Selected = 2
    }

    public enum QuestionType
    {
        /// <summary>
        /// tự nhập
        /// </summary>
        Constructed = 1,

        /// <summary>
        /// Chọn 1
        /// </summary>
        OneSelected = 2,

        /// <summary>
        /// Chọn nhiều đáp án
        /// </summary>
        MultiSelected = 3
    }

    public enum ReactionType
    {
        UnReaction = 0,
        Like = 1,
        Unlike = 2,
        Heart = 3
    }
    public enum VideoType
    {
        YoutubeId = 0,
        UrlCustom = 1,
        Upload = 2
    }

    public enum ThreadType
    {
        Discuss = 0,
        Question = 1,
        Shared = 2,
        Notice = 3,
        Other = 10
    }
}
