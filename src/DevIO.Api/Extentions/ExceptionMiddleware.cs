using System;
using System.Net;
using System.Threading.Tasks;
using Elmah.Io.AspNetCore;
using Microsoft.AspNetCore.Http;

namespace DevIO.Api.Extentions
{
    
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception erro)
            {
                await HandleExceptionAsync(context, erro);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception erro)
        {
            await erro.ShipAsync(context);
            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            
            //return Task.CompletedTask;
        }
    }
}
