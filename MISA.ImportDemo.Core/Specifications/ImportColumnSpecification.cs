using MISA.ImportDemo.Core.Entities;
using MMISA.ImportDemo.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.ImportDemo.Core.Specifications
{
    public sealed class ImportColumnSpecification: BaseSpecification<ImportColumn>
    {
        public ImportColumnSpecification(Guid worksheetId) : base(i => (i.ImportWorksheetId == worksheetId))
        {
            //AddInclude(b => b.tableName);
        }
    }
}
