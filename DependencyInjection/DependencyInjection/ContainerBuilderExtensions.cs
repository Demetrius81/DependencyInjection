using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection
{
    public static class ContainerBuilderExtensions
    {
        private static IContainerBuilder RedisterType(this IContainerBuilder builder, Type service, Type implimintation, LifeTime lifeTime)
        {
            builder.Register(new TypeBasedServiceDescriptor()
            {
                ImplementationType = implimintation,
                ServiceType = service,
                Lifetime = lifeTime
            });
            return builder;
        }

        private static IContainerBuilder RedisterFactory(this IContainerBuilder builder, Type service, Func<IScope, object> factory, LifeTime lifeTime)
        {
            builder.Register(new FactoryBasedServiceDescriptor()
            {
                Factory = factory,
                ServiceType = service,
                Lifetime = lifeTime
            });
            return builder;
        }

        private static IContainerBuilder RedisterInstance(this IContainerBuilder builder, Type service, object instance)
        {
            builder.Register(new InstanceBasedServiceDescriptor(service, instance));
            return builder;
        }

        public static IContainerBuilder RegisterSingleton(this IContainerBuilder builder, Type serviceInterface, Type serviceImplimintation)
        => builder.RedisterType(serviceInterface, serviceImplimintation, LifeTime.Singleton);

        public static IContainerBuilder RegisterTranscient(this IContainerBuilder builder, Type serviceInterface, Type serviceImplimintation)
        => builder.RedisterType(serviceInterface, serviceImplimintation, LifeTime.Transient);

        public static IContainerBuilder RegisterScoped(this IContainerBuilder builder, Type serviceInterface, Type serviceImplimintation)
        => builder.RedisterType(serviceInterface, serviceImplimintation, LifeTime.Scoped);

        //

        public static IContainerBuilder RegisterTranscient<TService, TImplementation>(this IContainerBuilder builder) where TImplementation : TService
        => builder.RedisterType(typeof(TService), typeof(TImplementation), LifeTime.Transient);

        public static IContainerBuilder RegisterScoped<TService, TImplementation>(this IContainerBuilder builder) where TImplementation : TService
        => builder.RedisterType(typeof(TService), typeof(TImplementation), LifeTime.Scoped);

        public static IContainerBuilder RegisterSingleton<TService, TImplementation>(this IContainerBuilder builder) where TImplementation : TService
        => builder.RedisterType(typeof(TService), typeof(TImplementation), LifeTime.Singleton);

        //

        public static IContainerBuilder RegisterTranscient<TService>(this IContainerBuilder builder, Func<IScope, TService> factory)
        => builder.RedisterFactory(typeof(TService), s => factory(s), LifeTime.Transient);

        public static IContainerBuilder RegisterScoped<TService>(this IContainerBuilder builder, Type service, Func<IScope, TService> factory)
        => builder.RedisterFactory(service, s => factory(s), LifeTime.Scoped);

        public static IContainerBuilder RegisterSingleton<TService>(this IContainerBuilder builder, Type service, Func<IScope, TService> factory)
        => builder.RedisterFactory(service, s => factory(s), LifeTime.Singleton);

        //
        public static IContainerBuilder RegisterSingleton<TService>(this IContainerBuilder builder, object instance)
        => builder.RedisterInstance(typeof(TService), instance);
    }
}
