using System;
using System.Collections.Generic;

namespace MISA.ImportDemo.Core.Entities
{
    public partial class Qualification
    {
        public Qualification()
        {
            Employee = new HashSet<Employee>();
        }

        public Guid QualificationId { get; set; }
        public string QualificationName { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

        public virtual ICollection<Employee> Employee { get; set; }
    }
}
