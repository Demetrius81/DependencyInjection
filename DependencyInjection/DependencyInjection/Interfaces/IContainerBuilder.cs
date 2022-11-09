namespace DependencyInjection;

public interface IContainerBuilder
{
    void Register(ServiceDescriptor descriptor);
    IContainer Build();
}