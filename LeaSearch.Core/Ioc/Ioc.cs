using System.Collections.Generic;
using Autofac;
using Autofac.Core;

namespace LeaSearch.Core.Ioc
{
    public static class Ioc
    {
        private static IContainer _container;
        private static ILifetimeScope _beginLifetimeScope;

        public static void SetContainer(IContainer container)
        {
            _container = container;
            _beginLifetimeScope = _container.BeginLifetimeScope();
        }

        public static TService Reslove<TService>()
        {
            return _container.Resolve<TService>();
        }

        public static TService Reslove<TService>(IEnumerable<Parameter> parameters)
        {
            return _container.Resolve<TService>(parameters);
        }

        public static TService Reslove<TService>(params Parameter[] parameters)
        {
            return _container.Resolve<TService>(parameters);
        }


        /// <summary>
        /// when a compon is useless, it will dispose
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public static TService ResloveUsingLifetime<TService>()
        {

            return _beginLifetimeScope.Resolve<TService>();
        }

        public static TService ResloveUsingLifetime<TService>(IEnumerable<Parameter> parameters)
        {
            return _beginLifetimeScope.Resolve<TService>(parameters);
        }

        public static TService ResloveUsingLifetime<TService>(params Parameter[] parameters)
        {
            return _beginLifetimeScope.Resolve<TService>(parameters);
        }
    }
}
