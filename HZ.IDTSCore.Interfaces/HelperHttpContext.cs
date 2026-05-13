using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces
{
   public static class HelperHttpContext
    {
        public static IServiceCollection serviceCollection;

        public static HttpContext Current
        {
            get
            {
                object factory = serviceCollection.BuildServiceProvider().GetService(typeof(IHttpContextAccessor));
                HttpContext context = ((IHttpContextAccessor)factory).HttpContext;
                return context;
            }
        }
    }
}
