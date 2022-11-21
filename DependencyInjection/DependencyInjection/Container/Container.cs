using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace DependencyInjection;

public class Container : IContainer
{
    private readonly ImmutableDictionary<Type, ServiceDescriptor> _descriptors;

    private readonly ConcurrentDictionary<Type, Func<IScope, object>> _buildActovators = new();

    private readonly Scope _rootScope;

    private readonly IActivationBuilder _builder;

    public Container(IEnumerable<ServiceDescriptor> descriptors, IActivationBuilder builder)
    {
        _descriptors = descriptors.ToImmutableDictionary(x => x.ServiceType);
        _rootScope = new(this);
        _builder = builder;
    }

    private class Scope : IScope
    {
        private readonly Container _container;

        private readonly ConcurrentDictionary<Type, object> _scopedInstances = new();

        private readonly ConcurrentStack<object> _disposables = new();

        public Scope(Container container)
        {
            _container = container;
        }

        public object Resolve(Type service)
        {
            var descriptor = _container.FindDescriptor(service);
            if (descriptor.Lifetime == LifeTime.Transient)
            {
                return CreateInstanceInternal(service);
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

        private object CreateInstanceInternal(Type service)
        {
            var result = _container.CreateInstance(service, this);

            if (result is IDisposable || result is IAsyncDisposable)
            {
                _disposables.Push(result);
            }

            return result;
        }

        public void Dispose()
        {
            foreach (var instance in _disposables)
            {
                if (instance is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                else if (instance is IAsyncDisposable asyncDisposable)
                {
                    asyncDisposable.DisposeAsync().GetAwaiter().GetResult();
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var instance in _disposables)
            {
                if (instance is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
                else if (instance is IDisposable disposable)
                {
                    disposable.Dispose();
                }
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

        return _builder.BuildActivation(descriptor);
    }

    private object CreateInstance(Type service, IScope scope)
    {
        return _buildActovators.GetOrAdd(service, service => BuildActivation(service))(scope);
    }

    public IScope CreateScope()
    {
        return new Scope(this);
    }

    public void Dispose() => _rootScope.Dispose();

    public ValueTask DisposeAsync() => _rootScope.DisposeAsync();
}