using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MISA.ImportDemo.Core.Entities;

namespace MISA.ImportDemo.Infrastructure.Data
{
    public partial class EfDbContext : DbContext
    {
        public static string ConnectionString;
        public EfDbContext()
        {
        }

        public EfDbContext(DbContextOptions<EfDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<EmployeeFamily> EmployeeFamily { get; set; }
        public virtual DbSet<Ethnic> Ethnic { get; set; }
        public virtual DbSet<ImportColumn> ImportColumn { get; set; }
        public virtual DbSet<ImportFileTemplate> ImportFileTemplate { get; set; }
        public virtual DbSet<ImportWorksheet> ImportWorksheet { get; set; }
        public virtual DbSet<Nationality> Nationality { get; set; }
        public virtual DbSet<Organization> Organization { get; set; }
        public virtual DbSet<Position> Position { get; set; }
        public virtual DbSet<Qualification> Qualification { get; set; }
        public virtual DbSet<Relation> Relation { get; set; }
        public virtual DbSet<ViewEmployee> ViewEmployee { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(ConnectionString, x => x.ServerVersion("10.3.25-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasComment("Phòng ban");

                entity.HasIndex(e => e.OrganizationId)
                    .HasName("FK_Department_OrganizationId");

                entity.Property(e => e.DepartmentId)
                    .ValueGeneratedNever()
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedBy)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DepartmentCode)
                    .IsRequired()
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.DepartmentName)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.OrganizationId)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Department)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("FK_Department_OrganizationId");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasComment("Danh mục nhân viên");

                entity.HasIndex(e => e.DepartmentId)
                    .HasName("FK_Employee_Department_DepartmentId");

                entity.HasIndex(e => e.EthnicId)
                    .HasName("FK_Employee_EthnicId");

                entity.HasIndex(e => e.NationalityId)
                    .HasName("FK_Employee_NationalityId");

                entity.HasIndex(e => e.OrganizationId)
                    .HasName("FK_Employee_OrganizationId");

                entity.HasIndex(e => e.PositionId)
                    .HasName("FK_Employee_Position_PositionId");

                entity.HasIndex(e => e.QualificationId)
                    .HasName("FK_Employee_Qualification_QualificationId");

                entity.Property(e => e.EmployeeId)
                    .ValueGeneratedNever()
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Address)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedBy)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.DepartmentId)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.EducationalBackground).HasColumnType("int(11)");

                entity.Property(e => e.Email)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.EmployeeCode)
                    .IsRequired()
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.EthnicId).HasColumnType("int(11)");

                entity.Property(e => e.FirstName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Gender).HasColumnType("int(11)");

                entity.Property(e => e.IdentityDate).HasColumnType("date");

                entity.Property(e => e.IdentityNo)
                    .HasColumnType("varchar(25)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.IdentityPlace)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.JoinDate).HasColumnType("date");

                entity.Property(e => e.LastName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.MaritalStatus).HasColumnType("int(11)");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.NationalityId)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.OrganizationId)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PersonalTaxCode)
                    .HasColumnType("varchar(25)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PhoneNumber)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ContractNo)
                   .HasColumnType("varchar(50)")
                   .HasCharSet("utf8")
                   .HasCollation("utf8_general_ci");

                entity.Property(e => e.ContractType).HasColumnType("int(11)");

                entity.Property(e => e.PositionId)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.QualificationId)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.WorkStatus).HasColumnType("int(11)");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.DepartmentId);

                entity.HasOne(d => d.Ethnic)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.EthnicId)
                    .HasConstraintName("FK_Employee_EthnicId");

                entity.HasOne(d => d.Nationality)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.NationalityId)
                    .HasConstraintName("FK_Employee_NationalityId");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employee_OrganizationId");

                entity.HasOne(d => d.Position)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.PositionId);

                entity.HasOne(d => d.Qualification)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.QualificationId);
            });

            modelBuilder.Entity<EmployeeFamily>(entity =>
            {
                entity.HasComment("Bảng thông tin chi tiết gia đình của Nhân viên");

                entity.HasIndex(e => e.EmployeeId)
                    .HasName("FK_EmployeeFamily_Emp_EmpID");

                entity.HasIndex(e => e.FullName)
                    .HasAnnotation("MySql:FullTextIndex", true);

                entity.HasIndex(e => e.RelationId)
                    .HasName("FK_EmployeeFamily_RelationID");

                entity.Property(e => e.EmployeeFamilyId)
                    .HasColumnName("EmployeeFamilyID")
                    .HasComment("Khóa chính")
                    .ValueGeneratedNever()
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.BirthVillageAddress)
                    .HasColumnType("text")
                    .HasComment("Địa chỉ khai sinh")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CitizenIdentityNo)
                    .HasColumnType("varchar(25)")
                    .HasComment("Số chứng minh thư/ hộ chiếu")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedBy)
                    .HasColumnType("varchar(255)")
                    .HasComment("Người tạo")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasComment("Ngày tạo bản ghi");

                entity.Property(e => e.DateOfBirth)
                    .HasColumnType("date")
                    .HasComment("Ngày sinh");

                entity.Property(e => e.DobdisplaySetting)
                    .HasColumnName("DOBDisplaySetting")
                    .HasColumnType("int(255)")
                    .HasDefaultValueSql("'0'")
                    .HasComment("Kiểu hiển thị ngày sinh(1- ngày/tháng/năm; 2- tháng/ngày/năm;...)");

                entity.Property(e => e.EmployeeId)
                    .HasColumnName("EmployeeID")
                    .HasComment("Khóa ngoại với bảng Employee")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.EthnicId)
                    .HasColumnName("EthnicID")
                    .HasColumnType("int(11)")
                    .HasComment("ID dân tộc");

                entity.Property(e => e.FullName)
                    .HasColumnType("varchar(255)")
                    .HasComment("Họ và tên")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Gender)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'")
                    .HasComment("Giới tính 0: Nữ, 1: Nam");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnType("varchar(255)")
                    .HasComment("Người thực hiện chỉnh sửa")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasComment("Ngày thực hiện chỉnh sửa gần nhất");

                entity.Property(e => e.NationalityId)
                    .HasColumnName("NationalityID")
                    .HasColumnType("int(11)")
                    .HasComment("ID quốc tịch");

                entity.Property(e => e.RelationId)
                    .HasColumnName("RelationID")
                    .HasColumnType("int(255)")
                    .HasComment("Quan hệ với chủ hộ (1- Bố, 2 - mẹ...)");

                entity.Property(e => e.SortOrder).HasColumnType("int(5)");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeFamily)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_EmployeeFamily_Emp_EmpID");

                entity.HasOne(d => d.Relation)
                    .WithMany(p => p.EmployeeFamily)
                    .HasForeignKey(d => d.RelationId)
                    .HasConstraintName("FK_EmployeeFamily_RelationID");
            });

            modelBuilder.Entity<Ethnic>(entity =>
            {
                entity.HasComment("Danh mục dân tộc");

                entity.Property(e => e.EthnicId)
                    .HasColumnName("EthnicID")
                    .HasColumnType("int(11)")
                    .HasComment("PK");

                entity.Property(e => e.ActiveDate)
                    .HasColumnType("date")
                    .HasComment("Ngày hiệu lực");

                entity.Property(e => e.EthnicCode)
                    .IsRequired()
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("''")
                    .HasComment("Mã dân tộc")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.EthnicName)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasComment("Tên dân tộc")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ExpireDate)
                    .HasColumnType("date")
                    .HasComment("Ngày hết hiệu lực");
            });

            modelBuilder.Entity<ImportColumn>(entity =>
            {
                entity.HasComment("Bảng dữ liệu các cột nhập khẩu");

                entity.HasIndex(e => e.ImportWorksheetId)
                    .HasName("FK_ImportWorksheet_ImportColumn_WorksheetID");

                entity.Property(e => e.ImportColumnId)
                    .HasColumnName("ImportColumnID")
                    .ValueGeneratedNever()
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ColumnDataType)
                    .HasColumnType("int(11)")
                    .HasComment("Kiểu dữ liệu");

                entity.Property(e => e.ColumnInsert)
                    .HasColumnType("varchar(255)")
                    .HasComment("Tên cột dữ liệu tương ứng")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ColumnPosition)
                    .HasColumnType("int(11)")
                    .HasComment("Vị trí của cột");

                entity.Property(e => e.ColumnTitle)
                    .HasColumnType("varchar(255)")
                    .HasComment("Tiêu đề cột")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ImportWorksheetId)
                    .HasColumnName("ImportWorksheetID")
                    .HasComment("Khóa ngoại với bảng ImportWorksheet")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.IsRequired)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'0'")
                    .HasComment("Là trường bắt buộc hay không (1- bắt buộc; 0- không bắt buộc)");

                entity.Property(e => e.ObjectReferenceName)
                    .HasColumnType("varchar(255)")
                    .HasComment("Tên Enum/ Table tham chiếu tương ứng")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.ImportWorksheet)
                    .WithMany(p => p.ImportColumn)
                    .HasForeignKey(d => d.ImportWorksheetId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ImportWorksheet_ImportColumn_WorksheetID");
            });

            modelBuilder.Entity<ImportFileTemplate>(entity =>
            {
                entity.HasComment("Bảng dữ liệu thông tin File nhập khẩu");

                entity.Property(e => e.ImportFileTemplateId)
                    .HasColumnName("ImportFileTemplateID")
                    .HasComment("Khóa chính")
                    .ValueGeneratedNever()
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedBy)
                    .HasColumnType("varchar(255)")
                    .HasComment("Người tạo")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasComment("Ngày tạo bản ghi");

                entity.Property(e => e.FileFormat)
                    .HasColumnType("varchar(255)")
                    .HasComment("Định dạng File")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ImportFileTemplateCode)
                    .HasColumnType("varchar(20)")
                    .HasComment("Mã tệp nhập khẩu")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ImportFileTemplateName)
                    .HasColumnType("varchar(255)")
                    .HasComment("Tên tệp nhập khẩu")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnType("varchar(255)")
                    .HasComment("Người thực hiện chỉnh sửa")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasComment("Ngày thực hiện chỉnh sửa gần nhất");

                entity.Property(e => e.ProcedureName)
                    .HasColumnType("varchar(255)")
                    .HasComment("Tên store thực hiện nhập khẩu")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.TableImport)
                    .HasColumnType("varchar(255)")
                    .HasComment("Teen bảng nhập khẩu")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.TotalWorksheet)
                    .HasColumnType("int(255)")
                    .HasDefaultValueSql("'1'")
                    .HasComment("Tổng số Worksheet");

                entity.Property(e => e.Version)
                    .HasColumnType("varchar(255)")
                    .HasComment("Phiên bản")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<ImportWorksheet>(entity =>
            {
                entity.HasIndex(e => e.ImportFileTemplateId)
                    .HasName("FK_ImportFileTemplate_ImportWorksheet_ImportTemplateID");

                entity.Property(e => e.ImportWorksheetId)
                    .HasColumnName("ImportWorksheetID")
                    .HasComment("Khóa chính")
                    .ValueGeneratedNever()
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ImportFileTemplateId)
                    .HasColumnName("ImportFileTemplateID")
                    .HasComment("Khóa ngoại bảng thông tin File nhập khẩu")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ImportToTable)
                    .HasColumnType("varchar(255)")
                    .HasComment("Tên bảng sẽ Import dữ liệu vào")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ImportWorksheetName)
                    .HasColumnType("varchar(255)")
                    .HasComment("Tên worksheet")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.IsImport)
                    .HasColumnType("bit(1)")
                    .HasDefaultValueSql("b'1'")
                    .HasComment("Sử dụng để nhập khẩu hay không (1-có; 0- không)");

                entity.Property(e => e.RowEndImport)
                    .HasColumnType("int(10)")
                    .HasComment("Vị trí dòng dừng nhập khẩu dữ liệu");

                entity.Property(e => e.RowHeaderPosition)
                    .HasColumnType("int(10)")
                    .HasDefaultValueSql("'1'")
                    .HasComment("Vị trí dòng tiêu đề");

                entity.Property(e => e.RowStartImport)
                    .HasColumnType("int(10)")
                    .HasComment("Vị trí dòng bắt đầu nhập khẩu dữ liệu");

                entity.Property(e => e.WorksheetPosition)
                    .HasColumnType("int(10)")
                    .HasComment("Vị trí của Wprksheet");

                entity.HasOne(d => d.ImportFileTemplate)
                    .WithMany(p => p.ImportWorksheet)
                    .HasForeignKey(d => d.ImportFileTemplateId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ImportFileTemplate_ImportWorksheet_ImportTemplateID");
            });

            modelBuilder.Entity<Nationality>(entity =>
            {
                entity.HasComment("Danh mục quốc tịch");

                entity.Property(e => e.NationalityId)
                   .HasColumnName("NationalityId")
                   .HasColumnType("int(11)")
                   .HasComment("PK");

                entity.Property(e => e.CreatedBy)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.NationalityCode)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.NationalityName)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.Property(e => e.OrganizationId)
                    .ValueGeneratedNever()
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Address)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ContactAddress)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ContactEmail)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ContactName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ContactPhoneNumber)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedBy)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.OrganizationCode)
                    .IsRequired()
                    .HasColumnType("varchar(20)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.OrganizationName)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.OrganizationType).HasColumnType("int(11)");

                entity.Property(e => e.ParentOrganizationId)
                    .HasColumnName("ParentOrganizationID")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PhoneNumber)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.TaxNo)
                    .HasColumnType("varchar(20)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.TenantCode)
                    .HasColumnType("varchar(250)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Position>(entity =>
            {
                entity.HasComment("Vị trí/ Chức vụ");

                entity.HasIndex(e => e.OrganizationId)
                    .HasName("FK_Position_OrganizationId");

                entity.Property(e => e.PositionId)
                    .ValueGeneratedNever()
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedBy)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.OrganizationId)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ParentId)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PositionCode)
                    .IsRequired()
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PositionName)
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Position)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("FK_Position_OrganizationId");
            });

            modelBuilder.Entity<Qualification>(entity =>
            {
                entity.HasComment("Trình độ chuyên môn");

                entity.Property(e => e.QualificationId)
                    .ValueGeneratedNever()
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedBy)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.QualificationName)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<Relation>(entity =>
            {
                entity.HasComment("Danh mục quan hệ");

                entity.Property(e => e.RelationId)
                    .HasColumnName("RelationID")
                    .HasColumnType("int(11)")
                    .HasComment("PK");

                entity.Property(e => e.RelationCode)
                    .IsRequired()
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("''")
                    .HasComment("Mã quan hệ")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.RelationName)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasComment("Tên quan hệ")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Sort)
                    .HasColumnType("int(255)")
                    .HasComment("STT/Sắp xếp");
            });

            modelBuilder.Entity<ViewEmployee>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_Employee");

                entity.Property(e => e.Address)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedBy)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.DepartmentCode)
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.DepartmentId)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.DepartmentName)
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.EducationalBackground).HasColumnType("int(11)");

                entity.Property(e => e.Email)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.EmployeeCode)
                    .IsRequired()
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.EmployeeId)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.FirstName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasColumnType("varchar(100)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Gender).HasColumnType("int(11)");

                entity.Property(e => e.IdentityDate).HasColumnType("date");

                entity.Property(e => e.IdentityNumber)
                    .HasColumnType("varchar(25)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.IdentityPlace)
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.JoinDate).HasColumnType("date");

                entity.Property(e => e.LastName)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.MaritalStatus).HasColumnType("int(11)");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.PersonalTaxCode)
                    .HasColumnType("varchar(25)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PhoneNumber)
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PositionCode)
                    .HasColumnType("varchar(20)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PositionId)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PositionName)
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.QualificationId)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.QualificationName)
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.WorkStatus).HasColumnType("int(11)");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
