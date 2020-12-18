using MISA.ImportDemo.Core.Enumeration;
using MISA.ImportDemo.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MISA.ImportDemo.Core.Entities
{
    public abstract class BaseEntity: IAggregateRoot
    {
        public EntityState BaseEntityState = EntityState.GET;
        public ImportValidState ImportValidState = ImportValidState.Valid;
        public List<string> ImportValidError = new List<string>();

        /// <summary>
        /// Trường xác định trong Property của object có dữ liệu nhập khẩu
        /// </summary>
        public bool HasPropertyValueImport = false;

        //[NotMapped]
        //public double? Sort { get; set; }
    }
}
