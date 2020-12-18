using System;
using System.Collections.Generic;

namespace MISA.ImportDemo.Core.Entities
{
    public partial class Relation
    {
        public Relation()
        {
            EmployeeFamily = new HashSet<EmployeeFamily>();
        }

        public int RelationId { get; set; }
        public string RelationCode { get; set; }
        public string RelationName { get; set; }
        public int? Sort { get; set; }

        public virtual ICollection<EmployeeFamily> EmployeeFamily { get; set; }
    }
}
