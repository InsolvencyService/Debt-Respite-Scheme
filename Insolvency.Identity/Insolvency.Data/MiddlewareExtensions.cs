using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Insolvency.Data
{
    public static class MiddlewareExtensions
    {
        public static void UseVersioningEndpoint(this IApplicationBuilder appBuilder, PathString path = default)
        {
            if (path == default)
                path = "/version";

            Func<HttpContext, bool> predicate = context =>
                context.Request.Path.StartsWithSegments(path, out var remaining) &&
                string.IsNullOrEmpty(remaining);

            appBuilder.MapWhen(predicate, aB => aB.UseMiddleware<VersioningEndpointMiddleware>());
        }
    }
}
