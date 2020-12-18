using MISA.ImportDemo.Core.Entities;
using MMISA.ImportDemo.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.ImportDemo.Core.Specifications
{
    public sealed class ImportFileSpecification:BaseSpecification<ImportFileTemplate>
    {
        public ImportFileSpecification(string tableName) : base(i => (i.TableImport == tableName))
        {
            //AddInclude(b => b.tableName);
        }
    }
}
