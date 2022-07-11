using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        private readonly ImmutableDictionary<Type, ServiceDescriptor> _descriptors;

        private readonly ConcurrentDictionary<Type, Func<IScope, object>> _buildActovators = new();

        private readonly Scope _rootScope;

        public Container(IEnumerable<ServiceDescriptor> descriptors)
        {
            _descriptors = descriptors.ToImmutableDictionary(x => x.ServiceType);
            _rootScope = new(this);
        }

        private class Scope : IScope
        {
            private readonly Container _container;

            private readonly ConcurrentDictionary<Type, object> _scopedInstances = new();

            public Scope(Container container)
            {
                _container = container;
            }
            public object Resolve(Type service)
            {
                var descriptor = _container.FindDescriptor(service);
                if (descriptor.Lifetime == LifeTime.Transient)
                {
                    return _container.CreateInstance(service, this);
                }

                if (descriptor.Lifetime == LifeTime.Scoped || _container._rootScope == this)
                {
                    return _scopedInstances.GetOrAdd(service, s => _container.CreateInstance(s, this));
                }
                else
                {
                    return _container._rootScope.Resolve(service);
                }
            }
        }

        private ServiceDescriptor? FindDescriptor(Type service)
        {
            _descriptors.TryGetValue(service, out var descriptor);

            return descriptor;
        }

        private Func<IScope, object> BuildActivation(Type service)
        {
            if (!_descriptors.TryGetValue(service, out var descriptor))
            {
                throw new InvalidOperationException($"Service {service} is not regisrered.");
            }

            if (descriptor is InstanceBasedServiceDescriptor ib)
            {
                return _ => ib.Instance;
            }
            if (descriptor is FactoryBasedServiceDescriptor fb)
            {
                return fb.Factory;
            }

            var tb = (TypeBasedServiceDescriptor)descriptor;
            var ctor = tb.ImplementationType.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Single();
            var args = ctor.GetParameters();
            return s =>
            {
                var argsForCtor = new object[args.Length];

                for (int i = 0; i < args.Length; i++)
                {
                    argsForCtor[i] = CreateInstance(args[i].ParameterType, s);
                }

                return ctor.Invoke(argsForCtor);
            };
        }

        private object CreateInstance(Type service, IScope scope)
        {
            return _buildActovators.GetOrAdd(service, service => BuildActivation(service))(scope);
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
}
