using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.ImportDemo.Core.Entities
{
    public class ImportInfo
    {
        public ImportInfo (string importKey, object data)
        {
            ImportKey = importKey;
            ImportData = data;
        }
        public string ImportKey { get; set; }
        public object ImportData { get; set; }
    }
}
