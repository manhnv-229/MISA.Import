using Microsoft.AspNetCore.Http;
using MISA.ImportDemo.Core.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MISA.ImportDemo.Core.Properties;
using MISA.ImportDemo.Core.Exceptions;
using MISA.ImportDemo.Core.Enumeration;
using Microsoft.Extensions.Configuration;
using MISA.ImportDemo.Core.Entities.ELKLock;
using System.Diagnostics;
using System.Text;
using System.IO;
using MISA.ImportDemo.Infrastructure.Data.Repositories;
using MISA.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MISA.ImportDemo.Middleware
{
    /// <summary>
    /// Xử lý khi có exception xảy ra
    /// </summary>
    /// CreatedBy: NVMANH (05/2020)
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private IWebHostEnvironment _webHostEnvironment;
        public ErrorHandlingMiddleware(RequestDelegate next, IWebHostEnvironment webHostEnvironment)
        {
            this._next = next;
            _webHostEnvironment = webHostEnvironment;
        }

        //TODO: xử lý các request không có OrgId
        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await HandleRequest(context);
                //await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Hàm xử lý các Request được gửi lên
        /// 1. Check xem có truyền thông tin đơn vị lên hay không? Chặn mọi Request không gửi hoặc gửi sai
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task HandleRequest(HttpContext context)
        {
            var request = context.Request;
            var cookies = context.Request.Cookies;
            //TODO: xử lý thông tin đơn vị - Fix đơn vị là MISA
            await HandleRequestWhenAuthenticationSuccess(context, Guid.Parse("166d7560-0f85-4bef-8981-c9f3c682c947"));
            //var orgIdString = request.Headers["OrganizationId"].ToString();
            //Guid orgId;
            //await HandleRequestWhenAuthenticationSuccess(context, Guid.Parse(orgIdString));
            //// Môi trường Dev bỏ qua xác thực thông tin Cookie lấy từ AMIS:
            //if ((_webHostEnvironment.IsDevelopment() && request.Headers["OrganizationId"].FirstOrDefault() != null) || request.Path.HasValue && request.Path.Value == "/swagger/index.html")
            //{
            //    await HandleRequestWhenAuthenticationSuccess(context, Guid.Parse(orgIdString));
            //}
            //// Đọc các thông tin trong Cookie, nếu thiếu thông tin đơn vị, user sẽ không cho phép thực hiện gọi tới các API.
            //else if (cookies["iv-sid"] == null ||
            //    cookies["iv-tenantId"] == null ||
            //    cookies["iv-tenantCode"] == null ||
            //    cookies["iv-userId"] == null ||
            //    request.Headers["OrganizationId"].FirstOrDefault() == null ||//Không có thông tin đơn vị
            //    (!Guid.TryParse(orgIdString, out orgId)))// Organization không đúng định dạng Guid
            //    await HandleRequestHasNotAuthentication(context);
            //else
            //{
            //    await HandleRequestWhenAuthenticationSuccess(context, Guid.Parse(orgIdString));
            //}

        }

        /// <summary>
        /// Xử lý request khi xác thực thông tin đầy đủ
        /// </summary>
        /// <param name="context"></param>
        /// <param name="orgId"></param>
        private async Task HandleRequestWhenAuthenticationSuccess(HttpContext context, Guid orgId)
        {
            CommonUtility.httpContext = context;
            var cookies = context.Request.Cookies;
            //TODO: Lấy thông tin đơn vị theo Id (Fix tạm):
            var OrganizationInfo = CommonUtility.GetOrganizationByOrganizationId(Guid.Parse("166d7560-0f85-4bef-8981-c9f3c682c947"));
            if (OrganizationInfo == null)
            {
                await HandleRequestHasNotAuthentication(context);
            }
            else
            {
                //context.Session.SetString("iv-sid", cookies["iv-sid"]);
                //context.Session.SetString("iv-tenantId", cookies["iv-tenantId"]);
                //context.Session.SetString("iv-tenantCode", cookies["iv-tenantCode"]);
                //context.Session.SetString("iv-userId", cookies["iv-userId"]);
                //context.Session.SetString("iv-userName", cookies["iv-userName"]);
                //context.Session.SetString("iv-fullName", cookies["iv-fullName"]);
                context.Session.SetString("OrganizationId", orgId.ToString());
                // Do work that doesn't write to the Response.
                await _next(context);
                // Do logging or other work that doesn't write to the Response.
            }
        }

        /// <summary>
        /// Xử lý khi thông tin xác thực không hợp lệ
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (02/07/2020)
        private static Task HandleRequestHasNotAuthentication(HttpContext context)
        {
            var result = JsonConvert.SerializeObject(
                                         new ActionServiceResult
                                         {
                                             Success = false,
                                             Messenge = Resources.Erro_NotAuthentication,
                                             MISACode = MISACode.Exception,
                                             Data = Resources.Erro_NotAuthentication
                                         });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return context.Response.WriteAsync(result);
        }


        /// <summary>
        /// Xử lý khi chương trình có Exception xảy ra
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        /// CreatedBy: NVMANH (01/07/2020)
        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var elkHelper = new ELKHelper();
            var methodName = new StackTrace(ex).GetFrame(0).GetMethod().Name;
            elkHelper.Error(string.Empty, ex.Message, string.Empty, methodName, ex);

            var code = HttpStatusCode.InternalServerError; // 500 if unexpected
            var msg = Resources.ErrorException; // 500 if unexpected
            var mCode = MISACode.Exception;
            if (ex is ImportException)
            {
                code = HttpStatusCode.BadRequest;
                msg = ex.Message;

            }
            //else if (ex is MyUnauthorizedException) code = HttpStatusCode.Unauthorized;
            //else if (ex is MyException) code = HttpStatusCode.BadRequest;
            var result = new ActionServiceResult
            {
                Success = false,
                Messenge = msg,
                MISACode = mCode,
            };
            if (_webHostEnvironment.IsDevelopment())
                result.Data = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            var resultJson = JsonConvert.SerializeObject(result);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(resultJson);
        }
    }
}
