namespace DependencyInjection;

public abstract class ServiceDescriptor
{
    public Type? ServiceType { get; init; }
    public LifeTime Lifetime { get; init; }
}