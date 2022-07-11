using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection
{
    public enum LifeTime
    {
        Transient,
        Scoped,
        Singleton
    }


    public interface IContainerBuilder
    {
        void Register(ServiceDescriptor descriptor);
        IContainer Build();
    }

    public class ContainerBuilder : IContainerBuilder
    {
        private readonly List<ServiceDescriptor> _descriptors = new();

        public IContainer Build()
        {
            return new Container(_descriptors);
        }

        public void Register(ServiceDescriptor descriptor)
        {
            _descriptors.Add(descriptor);
        }
    }

    public class Container : IContainer
    {
        private readonly Dictionary<Type, ServiceDescriptor> _descriptors;
        private class Scope : IScope
        {
            private readonly Container _container;

            public Scope(Container container)
            {
                _container = container;
            }
            public object Resolve(Type service)
            {
                // Временная реализация

                return _container.CreateInstance(service, this);
            }
        }

        private object CreateInstance(Type service, IScope scope)
        {
            if (!_descriptors.TryGetValue(service, out var descriptor))
            {
                throw new InvalidOperationException($"Service {service} is not regisrered.");
            }

            if (descriptor is InstanceBasedServiceDescriptor ib)
            {
                return ib.Instance;
            }
            if (descriptor is FactoryBasedServiceDescriptor fb)
            {
                return fb.Factory(scope);
            }

            var tb = (TypeBasedServiceDescriptor)descriptor;

            var ctor = tb.ImplementationType.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Single();
            var args = ctor.GetParameters();
            var argsForCtor = new object[args.Length];

            for (int i = 0; i < args.Length; i++)
            {
                argsForCtor[i] = CreateInstance(args[i].ParameterType, scope);
            }

            return ctor.Invoke(argsForCtor);
        }

        public Container(IEnumerable<ServiceDescriptor> descriptors)
        {
            _descriptors = descriptors.ToDictionary(x => x.ServiceType);
        }
        public IScope CreateScope()
        {
            return new Scope(this);
        }
    }

    public interface IContainer
    {
        IScope CreateScope();
    }

    public interface IScope
    {
        object Resolve(Type service);
    }





    //var reg = new Registration();
    //var conteiner = reg.ConfigureServices();
    //conteiner.Resolve<Controller>().Do();





    //class ContainerBuilder : IContainerBuilder
    //{

    //}


    //class Container : IContainer
    //{

    //}

}
