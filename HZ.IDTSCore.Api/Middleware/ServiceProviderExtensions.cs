using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Middleware
{
    public static class ServiceProviderExtensions
    {
        public static T ResolveWith<T>(this IServiceProvider provider,string s) where T : class =>
           ActivatorUtilities.CreateInstance<T>(provider, s);
    }
}
