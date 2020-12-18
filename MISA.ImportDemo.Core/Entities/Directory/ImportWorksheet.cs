using System;
using System.Collections.Generic;

namespace MISA.ImportDemo.Core.Entities
{
    public partial class ImportWorksheet
    {
        public ImportWorksheet()
        {
            ImportColumn = new HashSet<ImportColumn>();
        }

        public Guid ImportWorksheetId { get; set; }
        public string ImportWorksheetName { get; set; }
        public Guid? ImportFileTemplateId { get; set; }
        public int RowHeaderPosition { get; set; }
        public int? RowStartImport { get; set; }
        public int? RowEndImport { get; set; }
        public int? WorksheetPosition { get; set; }
        public string ImportToTable { get; set; }
        public ulong IsImport { get; set; }

        public virtual ImportFileTemplate ImportFileTemplate { get; set; }
        public virtual ICollection<ImportColumn> ImportColumn { get; set; }
    }
}
