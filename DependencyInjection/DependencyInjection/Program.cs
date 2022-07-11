﻿using DependencyInjection;

//IContainerBuilder builder = new ContainerBuilder();
//builder.RegisterScoped<IService, Service>()
//       .RegisterTranscient<IHelper>(s => new Helper())
//       .RegisterSingleton<IAnotherService>(AnotherService.Instance);

IContainerBuilder builder = new ContainerBuilder();

var container = builder
    .RegisterTranscient<IService, Service>()
    .RegisterScoped<Controller, Controller>()
    .Build();

var scope = container.CreateScope();

var controller1 = scope.Resolve(typeof(Controller));
var controller2 = scope.Resolve(typeof(Controller));

if (controller1 != controller2)
{
    throw new InvalidOperationException();
}
Console.ReadKey(true);





interface IAnotherService
{
    //Интерфейс сервиса - заглушка
}

class AnotherService : IAnotherService
{
    //Сервис - заглушка
    private AnotherService()
    {

    }
    public static AnotherService Instance = new();
}

interface IHelper
{
    //Интерфейс - заглушка
}

class Helper : IHelper
{
    //Класс - заглушка
}










class Registration
{
    public IContainer ConfigureServices()
    {
        var builder = new ContainerBuilder();
        builder.RegisterTranscient<IService, Service>();
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