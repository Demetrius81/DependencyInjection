using Autofac;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection
{
    public class ContainerBenchmark
    {
        private readonly IScope _reflectionBased, _lambdaBased;
        private readonly ILifetimeScope _scope;
        private readonly IServiceScope _msdi;

        public ContainerBenchmark()
        {
            IContainerBuilder lambdaBasedBuilder = new ContainerBuilder(new LambdaBasedActivationBuilder());
            IContainerBuilder reflectionBasedBuilder = new ContainerBuilder(new ReflectionBasedActivationBuilder());
            InitContainer(lambdaBasedBuilder);
            InitContainer(reflectionBasedBuilder);
            _reflectionBased = reflectionBasedBuilder.Build().CreateScope();
            _lambdaBased = lambdaBasedBuilder.Build().CreateScope();
            _scope = InitAutofac();
            _msdi = InitMSDI();
        }

        private void InitContainer(IContainerBuilder builder)
        {
            builder.RegisterTranscient<IService, Service>()
                   .RegisterTranscient<Controller, Controller>();
        }

        private IServiceScope InitMSDI()
        {
            var collection = new ServiceCollection();
            collection.AddTransient<IService, Service>();
            collection.AddTransient<Controller, Controller>();
            return collection.BuildServiceProvider().CreateScope();
        }

        private ILifetimeScope InitAutofac()
        {
            var containerBuilder = new Autofac.ContainerBuilder();
            containerBuilder.RegisterType<Service>().As<IService>();
            containerBuilder.RegisterType<Service>().AsSelf();
            return containerBuilder.Build().BeginLifetimeScope();
        }

        [Benchmark(Baseline = true)]
        public Controller GetControllerByHands() => new Controller(new Service());
        [Benchmark]
        public Controller ReflectionGetController() => (Controller)_reflectionBased.Resolve(typeof(Controller));
        [Benchmark]
        public Controller LambdaGetController() => (Controller)_lambdaBased.Resolve(typeof(Controller));
        [Benchmark]
        public Controller AutofacGetController() => _scope.Resolve<Controller>();
        [Benchmark]
        public Controller MSDI_GetController() => _msdi.ServiceProvider.GetRequiredService<Controller>();
    }
}
