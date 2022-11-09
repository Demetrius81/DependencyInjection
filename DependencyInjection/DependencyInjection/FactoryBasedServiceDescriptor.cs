namespace DependencyInjection;

public class FactoryBasedServiceDescriptor : ServiceDescriptor
{
    public Func<IScope, object>? Factory { get; init; }
}
