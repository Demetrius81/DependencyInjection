namespace DependencyInjection;

public class InstanceBasedServiceDescriptor : ServiceDescriptor
{
    public object Instance { get; init; }

    public InstanceBasedServiceDescriptor(Type serviceType, object instance)
    {
        Lifetime = LifeTime.Singleton;
        ServiceType = serviceType;
        Instance = instance;
    }
}
