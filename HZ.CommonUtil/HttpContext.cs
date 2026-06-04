using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.CommonUtil
{
    //public static class HttpContext
    //{
    //    private static IHttpContextAccessor _contextAccessor;

    //    public static Microsoft.AspNetCore.Http.HttpContext Current => _contextAccessor != null ? _contextAccessor.HttpContext : null;

    //    internal static void Configure(IHttpContextAccessor contextAccessor)
    //    {
    //        _contextAccessor = contextAccessor;
    //    }
    //}

    //public static class StaticHttpContextExtensions
    //{
    //    public static IApplicationBuilder UseStaticHttpContext(this IApplicationBuilder app)
    //    {
    //        var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
    //        HttpContext.Configure(httpContextAccessor);
    //        return app;
    //    }
    //}
}
