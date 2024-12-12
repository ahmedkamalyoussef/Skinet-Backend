using System.Net;
using System.Text.Json;
using Skinet.API.Errors;
namespace Skinet.API.Middlewares;

public class ExceptionMiddleware(IHostEnvironment env ,RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception,env); 
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, IHostEnvironment env)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        var response = env.IsDevelopment()
            ? new ApiErrorRespone(context.Response.StatusCode, exception.Message, exception.StackTrace)
            : new ApiErrorRespone(context.Response.StatusCode, exception.Message, "Internal Server Error");

        var option = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var json = JsonSerializer.Serialize((response, option));
        return context.Response.WriteAsync(json);
    }
}