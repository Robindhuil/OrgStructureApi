using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OrgStructureApi.Dtos;

namespace OrgStructureApi.Middleware;

public class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ApiExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DbUpdateException dbEx)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;

            var resp = new ApiErrorResponse((int)HttpStatusCode.Conflict, "Conflict", "A database conflict occurred.")
            {
                Detail = dbEx.InnerException?.Message ?? dbEx.Message
            };

            var json = JsonSerializer.Serialize(resp);
            await context.Response.WriteAsync(json);
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var resp = new ApiErrorResponse((int)HttpStatusCode.InternalServerError, "Server Error", ex.Message);
            var json = JsonSerializer.Serialize(resp);
            await context.Response.WriteAsync(json);
        }
    }
}
