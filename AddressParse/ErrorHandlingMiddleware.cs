using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace AddressParse
{


    // link：https://stackoverflow.com/questions/38630076/asp-net-core-web-api-exception-handling
    public class ErrorHandlingMiddleware
    {
       
        private readonly RequestDelegate next;
        /// <summary>
        /// 环境
        /// </summary>
        private readonly IWebHostEnvironment Environment;
        public ErrorHandlingMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            Environment = env;
            this.next = next;
        }

        public IWebHostEnvironment Environment1 => Environment;

        public async Task Invoke(HttpContext context)
        {
            String msg = String.Empty;
            try
            {             
              
                await next(context);
            }
            catch (Exception ex)
            {
                msg = ex.Message;             
            }
            finally
            {
              
            }
        }
    }
}
