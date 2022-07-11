
namespace DependencyInjection
{
    public interface IActivationBuilder
    {
        Func<IScope, object> BuildActivation(ServiceDescriptor descriptor);
    }
}
