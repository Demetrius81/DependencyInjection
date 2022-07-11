using DependencyInjection;

IContainerBuilder builder;
builder.RegisterScoped<IService, Service>()
       .RegisterTranscient<IService>(s => new Service())
       .RegisterSingleton<IService>(anotherServiceInstance);

class Registration
{
    public Container ConfigureServices()
    {
        var builder = new ContainerBuilder();
        builder.RegisterTransient<IService, Service>();
        builder.RegisterScoped<Controller, Controller>();
        return builder.Build();
    }
}

class Controller
{
    private readonly IService _service;

    public Controller(IService service)
    {
        _service = service;
    }

    public void Do()
    {

    }
}

interface IService
{

}

class Service : IService
{

}