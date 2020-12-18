using System;
using System.Collections.Generic;

namespace MISA.ImportDemo.Core.Entities
{
    public partial class Ethnic
    {
        public Ethnic()
        {
            Employee = new HashSet<Employee>();
        }

        public int EthnicId { get; set; }
        public string EthnicCode { get; set; }
        public string EthnicName { get; set; }
        public DateTime? ActiveDate { get; set; }
        public DateTime? ExpireDate { get; set; }

        public virtual ICollection<Employee> Employee { get; set; }
    }
}
