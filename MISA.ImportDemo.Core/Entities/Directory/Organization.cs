using System;
using System.Collections.Generic;

namespace MISA.ImportDemo.Core.Entities
{
    public partial class Organization:BaseEntity
    {
        public Organization()
        {
            Department = new HashSet<Department>();
            Employee = new HashSet<Employee>();
            Position = new HashSet<Position>();
        }

        public Guid OrganizationId { get; set; }
        public string OrganizationCode { get; set; }
        public string OrganizationName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string ContactAddress { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhoneNumber { get; set; }
        public Guid? ParentOrganizationId { get; set; }
        public string ContactName { get; set; }
        public string TenantCode { get; set; }
        public int? OrganizationType { get; set; }
        public string TaxNo { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

        public virtual ICollection<Department> Department { get; set; }
        public virtual ICollection<Employee> Employee { get; set; }
        public virtual ICollection<Position> Position { get; set; }
    }
}
