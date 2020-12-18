using MISA.ImportDemo.Core.Enumeration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MISA.ImportDemo.Core.Entities
{
    public partial class Employee:BaseEntity
    {
        public Employee()
        {
            EmployeeFamily = new HashSet<EmployeeFamily>();
        }

        public Guid EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public int? Gender { get; set; }
        public string GenderName
        {
            get
            {
                var name = string.Empty;
                switch ((Enumeration.Gender)Gender)
                {
                    case Enumeration.Gender.Female:
                        name = Properties.Resources.Enum_Gender_Female;
                        break;
                    case Enumeration.Gender.Male:
                        name = Properties.Resources.Enum_Gender_Male;
                        break;
                    case Enumeration.Gender.Other:
                        name = Properties.Resources.Enum_Gender_Other;
                        break;
                    default:
                        break;
                }
                return name;
            }
        }

        public DateTime? DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string IdentityNo { get; set; }
        public DateTime? IdentityDate { get; set; }
        public string IdentityPlace { get; set; }
        public DateTime? JoinDate { get; set; }
        public int? MaritalStatus { get; set; }
        public int? EducationalBackground { get; set; }
        public Guid? QualificationId { get; set; }
        public Guid? DepartmentId { get; set; }
        public int? EthnicId { get; set; }
        public int? NationalityId { get; set; }
        public Guid? PositionId { get; set; }
        public Guid OrganizationId { get; set; }
        public int? WorkStatus { get; set; }
        public string PersonalTaxCode { get; set; }
        public string ContractNo { get; set; }
        public int? ContractType { get; set; }
        public double? Salary { get; set; }

        [NotMapped]
        public string PositionName { get; set; }

        [NotMapped]
        public string DepartmentName { get; set; }
        [NotMapped]
        public string NationalityName { get; set; }
        [NotMapped]
        public string EthnicName { get; set; }


        [NotMapped]
        public int? Sort { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

       
        public virtual Department Department { get; set; }
        public virtual Ethnic Ethnic { get; set; }
        public virtual Nationality Nationality { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual Position Position { get; set; }
        public virtual Qualification Qualification { get; set; }
        public virtual ICollection<EmployeeFamily> EmployeeFamily { get; set; }

       
    }
}
