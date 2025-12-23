using Microsoft.AspNetCore.Builder;
using OrgStructureApi.Middleware;

namespace OrgStructureApi.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseApiExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ApiExceptionMiddleware>();
    }
}
