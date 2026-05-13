using Autofac;
using HZ.DbHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces
{
    /// <summary>
    /// Autofac的服务定位器
    /// </summary>
    public class ServiceLocator
    {
        private static IContainer _container;

        /// <summary>
        /// 设置Ico容器
        /// </summary>
        /// <param name="container"></param>
        public static void SetContainer(IContainer container)
        {
            _container = container;
        }

        public static IService GetService<IService>()
        {
            return _container.Resolve<IService>();
        }

        public static IService GetService<IService>(SessionInfo session)
        {
            //return _container.Resolve<IService>(new NamedParameter("token", token), new NamedParameter("org", org));
            return _container.Resolve<IService>(new NamedParameter("session", session));
        }

        public static IService GetServiceByName<IService>(string name, SessionInfo session)
        {
            //return _container.ResolveNamed<IService>(name, new NamedParameter("token", token), new NamedParameter("org", org));
            return _container.ResolveNamed<IService>(name, new NamedParameter("session", session));
        }

        //public static IBaseService<entityT> GetService<IBaseService, entityT>(string token)
        //{
        //    IBaseService<entityT> interfaceInjection = (IBaseService<entityT>)_container.Resolve<IBaseService>(new NamedParameter("token", token));
        //    interfaceInjection.SetUs(token);
        //    return interfaceInjection;
        //}

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <typeparam name="IService"></typeparam>
        /// <returns></returns>
        //public static T GetService<T>() where T : IBaseService<T> 
        //{
        //    return _container.Resolve<IService>();

        //    T  v= _container.Resolve<IService>();

        //    ((IBaseService<T>)v).SetUs(((IBaseService<T>)v).GetUs());
        //    return (IBaseService<T>)v;
        //}

        /// <summary>
        /// 获取容器对象
        /// </summary>
        /// <returns></returns>
        public static IContainer GetContainer()
        {
            return _container;
        }
    }
}