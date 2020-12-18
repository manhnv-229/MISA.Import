using System;
using System.Collections.Generic;

namespace MISA.ImportDemo.Core.Entities
{
    public partial class ImportColumn:BaseEntity
    {
        public Guid ImportColumnId { get; set; }
        public string ColumnTitle { get; set; }
        public int? ColumnDataType { get; set; }
        public ulong IsRequired { get; set; }
        public int? ColumnPosition { get; set; }
        public Guid? ImportWorksheetId { get; set; }
        public string ObjectReferenceName { get; set; }
        public string ColumnInsert { get; set; }

        public virtual ImportWorksheet ImportWorksheet { get; set; }
    }
}
