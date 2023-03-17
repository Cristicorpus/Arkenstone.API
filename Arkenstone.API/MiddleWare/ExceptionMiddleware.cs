using Arkenstone.Logic.BusinessException;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Arkenstone.API.MiddleWare
{
    public class ExceptionMiddleware : IMiddleware
    {

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (NoContent ex)
            {
                var response = new
                {
                    message = ex.Message
                };
                var json = JsonSerializer.Serialize(response);

                context.Response.StatusCode = 204;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(json);
            }
            catch (NotAuthentified ex)
            {
                var response = new
                {
                    message = ex.Message
                };
                var json = JsonSerializer.Serialize(response);

                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(json);
            }
            catch (NotAuthorized ex)
            {
                var response = new
                {
                    message = ex.Message
                };
                var json = JsonSerializer.Serialize(response);

                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(json);
            }
            catch (NotFound ex)
            {
                var response = new
                {
                    message = ex.Message
                };
                var json = JsonSerializer.Serialize(response);

                context.Response.StatusCode = 404;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(json);
            }
            catch (ParameterException ex)
            {
                var response = new
                {
                    message = ex.Message
                };
                var json = JsonSerializer.Serialize(response);

                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(json);
            }
            catch (BadRequestException ex)
            {
                var response = new
                {
                    message = ex.Message
                };
                var json = JsonSerializer.Serialize(response);

                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(json);
            }
            catch (Exception ex)
            {
                var response = new { 
                    message = ex.Message,
                    stackTrace = ex.StackTrace
                };
                var json = JsonSerializer.Serialize(response);

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(json);
            }
        }
    }
    
   
}
