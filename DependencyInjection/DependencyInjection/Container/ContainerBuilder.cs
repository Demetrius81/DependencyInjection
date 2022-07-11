
namespace DependencyInjection
{
    public class ContainerBuilder : IContainerBuilder
    {
        private readonly List<ServiceDescriptor> _descriptors = new();

        private readonly IActivationBuilder _builder;

        public ContainerBuilder(IActivationBuilder builder)
        {
            _builder = builder;
        }

        public IContainer Build()
        {
            return new Container(_descriptors, _builder);
        }

        public void Register(ServiceDescriptor descriptor)
        {
            _descriptors.Add(descriptor);
        }
    }
}
