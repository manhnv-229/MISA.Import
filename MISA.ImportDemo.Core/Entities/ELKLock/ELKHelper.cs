using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace MISA.ImportDemo.Core.Entities.ELKLock
{
    public class ELKHelper
    {
        private string elkAppID, elkSystemID, elkVersion, elkOn, elkUrl = "";
        //private readonly IOptions<AppsettingConfig> configuration;
        public ELKHelper()
        {
            //configuration = config;
            //elkAppID = configuration.Value.ELKConfig.ELKAppID;
            //elkSystemID = configuration.Value.ELKConfig.ELKSystemID;
            //elkVersion = configuration.Value.ELKConfig.ELKVersion;
            //elkOn = configuration.Value.ELKConfig.ELKOn;
            //elkUrl = "http://testlogapi.misa.com.vn";//configuration.Value.ELKConfig.ELKUrl;
            elkAppID = "ivan";
            elkSystemID = "web";
            elkVersion = "v1";
            elkOn = "true";
            elkUrl = "http://testlogapi.misa.com.vn";//configuration.Value.ELKConfig.ELKUrl;
        }

        /// <summary>
        /// Hàm gửi log elk infor
        /// </summary>
        /// <param name="device_infor">tên thiết bị</param>
        /// <param name="message">message</param>
        /// <param name="user_name">tên người dùng</param>
        /// <param name="function">tên hàm</param>
        /// Created by VDThang 10.05.2020
        public void Infor(string device_infor, string message, string user_name, string function)
        {
            var elk = new ELKData();
            elk.app_id = elkAppID;
            elk.system_id = elkSystemID;
            elk.version = elkVersion;
            elk.date = DateTime.Now;
            elk.device_info = device_infor;
            elk.level = "INFOR";
            elk.message = message;
            elk.stack_trace = null;
            elk.inner_exception = null;
            elk.message_exception = null;
            elk.stacktrace_exception = null;
            elk.user_name = user_name;
            elk.function = function;
            var json = JsonConvert.SerializeObject(elk);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            bool send_on = bool.Parse(elkOn);
            if (send_on)
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "bWlzYTpNaXNhQDIwMTk=");
                client.PostAsync(elkUrl, data);
            }
        }

        /// <summary>
        /// Hàm gửi log elk lỗi
        /// </summary>
        /// <param name="device_infor">tên thiết bị</param>
        /// <param name="message">message</param>
        /// <param name="user_name">tên người dùng</param>
        /// <param name="function">tên hàm</param>
        /// <param name="exception">ngoại lệ</param>
        /// Created by VDThang 10.05.2020
        public void Error(string device_infor, string message, string user_name, string function, Exception exception)
        {
            var elk = new ELKData();
            elk.app_id = elkAppID;
            elk.system_id = elkSystemID;
            elk.version = elkVersion;
            elk.date = DateTime.Now;
            elk.device_info = device_infor;
            elk.level = "ERROR";
            elk.message = message;
            elk.stack_trace = exception;
            elk.inner_exception = exception.InnerException == null ? "" : exception.InnerException.ToString();
            elk.message_exception = exception.Message;
            elk.stacktrace_exception = exception.StackTrace;
            elk.user_name = user_name;
            elk.function = function;
            var json = JsonConvert.SerializeObject(elk);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            bool send_on = bool.Parse(elkOn);
            if (send_on)
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "bWlzYTpNaXNhQDIwMTk=");
                client.PostAsync(elkUrl, data);
            }

        }
    }
}
