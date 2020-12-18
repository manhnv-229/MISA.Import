using MISA.ImportDemo.Core.Enumeration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.ImportDemo.Core.Entities
{
    public class ActionServiceResult
    {
        public ActionServiceResult()
        {
    
        }
        public ActionServiceResult(bool success, string msg, MISACode code)
        {
            this.Success = success;
            this.Messenge = msg;
            this.MISACode = code;
        }

        public ActionServiceResult(bool success, string msg, MISACode code, object data)
        {
            this.Success = success;
            this.Messenge = msg;
            this.MISACode = code;
            this.Data = data;
        }
        public bool Success { get; set; } = true;
        public string Messenge { get; set; }
        public MISACode MISACode { get; set; }
        public object Data { get; set; }
    }
}
