using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace HZ.DbHelper
{
    /// <summary>
    /// 兼容旧代码中 HZ.DbHelper.HttpContext.Current 的访问方式。
    /// .NET Core 没有 System.Web.HttpContext.Current，所以通过 IHttpContextAccessor 获取当前请求上下文。
    /// </summary>
    public static class HttpContext
    {
        private static IHttpContextAccessor _contextAccessor;

        /// <summary>
        /// 当前请求上下文。后台线程或非HTTP请求场景下可能为空。
        /// </summary>
        public static Microsoft.AspNetCore.Http.HttpContext Current
        {
            get
            {
                return _contextAccessor == null ? null : _contextAccessor.HttpContext;
            }
        }

        /// <summary>
        /// 保存应用启动时注入的 IHttpContextAccessor。
        /// </summary>
        internal static void Configure(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
    }
}

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// 注册静态 HttpContext 访问器，兼容项目中原有 app.UseStaticHttpContext() 调用。
    /// </summary>
    public static class StaticHttpContextExtensions
    {
        public static IApplicationBuilder UseStaticHttpContext(this IApplicationBuilder app)
        {
            var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            HZ.DbHelper.HttpContext.Configure(httpContextAccessor);
            return app;
        }
    }
}
