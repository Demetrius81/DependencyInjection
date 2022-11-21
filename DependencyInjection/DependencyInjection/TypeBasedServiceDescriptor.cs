namespace DependencyInjection;

public class TypeBasedServiceDescriptor : ServiceDescriptor
{
    public Type? ImplementationType { get; init; }
}
