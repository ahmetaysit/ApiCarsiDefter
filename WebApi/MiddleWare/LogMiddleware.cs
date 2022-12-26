using Core;
using Interfaces;
using Interfaces.BusinessIntefaces;
using Library.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;

namespace WebApi.MiddleWare
{
    public class LogMiddleware
    {
        private readonly RequestDelegate _next;

        public LogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, ILogsBS logService)
        {
            logService.Insert(new Library.Entities.Logs { Message = "Basladi"});
            logService.Save();
            var identity = httpContext.User.Identity as ClaimsIdentity;
            var context = identity.FindFirst("userContext");
            if(context != null)
            {
                var user = JsonConvert.DeserializeObject<UserResponseModel>(context.Value);
            }
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //Task.Run(() => { _logger.Log(httpContext.Request.Path.Value); });
            try
            {

                //start
                await _next(httpContext);

                //complete
            }
            catch (Exception ex)
            {
                logService.Insert(new Library.Entities.Logs { Message = ex.Message });
                Task.Run(() => { Console.WriteLine(ex.Message); });
            }
            finally
            {
                stopwatch.Stop();
                string time = stopwatch.Elapsed.ToString();
                Task.Run(() => { Console.WriteLine(time); });
                logService.Save();
            }
            
            
        }
    }
}
