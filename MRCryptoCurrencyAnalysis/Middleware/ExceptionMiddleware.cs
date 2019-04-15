using Microsoft.AspNetCore.Http;
using MRIdentityClient.Exception.Basic;
using MRIdentityClient.Exception.MRSystem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MRCryptoCurrencyAnalysis.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            string result;

            if(ex is MRException)
            {
                var mrException = (MRException)ex;

                result = JsonConvert.SerializeObject(new 
                {
                    code = mrException.Code,
                    message = mrException.Message,
                    user_message = mrException.UserMessage,
                });
            }
            else
            {
                result = JsonConvert.SerializeObject(new
                {
                    code = -1,
                    message = "System exception",
                    user_message = "Operation failed",
                });
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
