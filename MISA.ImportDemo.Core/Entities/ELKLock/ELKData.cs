using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.ImportDemo.Core.Entities.ELKLock
{
    public class ELKData
    {
        public string app_id { get; set; }
        public string system_id { get; set; }
        public string device_info { get; set; }
        public string version { get; set; }
        public string level { get; set; }
        public string message { get; set; }
        public DateTime date { get; set; }
        public string user_name { get; set; }
        public Exception stack_trace { get; set; }
        public string inner_exception { get; set; }
        public string message_exception { get; set; }
        public string stacktrace_exception { get; set; }
        public string function { get; set; }
    }
}
