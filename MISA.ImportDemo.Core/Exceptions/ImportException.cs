using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.ImportDemo.Core.Exceptions
{
    public class ImportException: Exception
    {
        public ImportException(int importFileTemplate) : base($"File không đúng định dạng {importFileTemplate}")
        {
        }

        protected ImportException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }

        public ImportException(string message) : base(message)
        {
        }

        public ImportException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}
