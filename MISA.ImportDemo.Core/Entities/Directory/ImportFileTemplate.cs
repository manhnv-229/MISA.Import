using System;
using System.Collections.Generic;

namespace MISA.ImportDemo.Core.Entities
{
    public partial class ImportFileTemplate:BaseEntity
    {
        public ImportFileTemplate()
        {
            ImportWorksheet = new HashSet<ImportWorksheet>();
        }

        public Guid ImportFileTemplateId { get; set; }
        public string ImportFileTemplateCode { get; set; }
        public string ImportFileTemplateName { get; set; }
        public string Version { get; set; }
        public string FileFormat { get; set; }
        public string TableImport { get; set; }
        public string ProcedureName { get; set; }
        public int TotalWorksheet { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

        public virtual ICollection<ImportWorksheet> ImportWorksheet { get; set; }
    }
}
