using System;
using System.Collections.Generic;

namespace MISA.ImportDemo.Core.Entities
{
    public partial class Nationality
    {
        public Nationality()
        {
            Employee = new HashSet<Employee>();
        }

        public int NationalityId { get; set; }
        public string NationalityCode { get; set; }
        public string NationalityName { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual ICollection<Employee> Employee { get; set; }
    }
}
