using DependencyInjection;





Run run = new Run();
run.RunTest();













//IContainerBuilder builder = new ContainerBuilder(new LambdaBasedActivationBuilder());
//using (var container = builder
//        .RegisterTranscient<IService, Service>()
//        .RegisterScoped<Controller, Controller>()
//        .RegisterSingleton<IAnotherService>(AnotherService.Instance)
//        .Build())
//{

//    var scope = container.CreateScope();

//    var controller1 = scope.Resolve(typeof(Controller));
//    var controller2 = scope.Resolve(typeof(Controller));
//    var i1 = scope.Resolve(typeof(IAnotherService));

//    var scope2 = container.CreateScope();
//    var controller3 = scope2.Resolve(typeof(Controller));
//    var i2 = scope2.Resolve(typeof(IAnotherService));

//    if (controller1 != controller2 
//        || controller1 == controller3 
//        || controller2 == controller3 
//        || i1 != i2 
//        || scope == scope2)
//    {
//        throw new InvalidOperationException();
//    }
//    Console.ReadKey(true);
//}

//interface IAnotherService
//{
//    //Интерфейс сервиса - заглушка
//}

//class AnotherService : IAnotherService
//{
//    //Сервис - заглушка
//    private AnotherService()
//    {

//    }
//    public static AnotherService Instance = new();
//}

//interface IHelper
//{
//    //Интерфейс - заглушка
//}

//class Helper : IHelper
//{
//    //Класс - заглушка
//}

//class Registration
//{
//    public IContainer ConfigureServices()
//    {
//        var builder = new ContainerBuilder(new LambdaBasedActivationBuilder());
//        builder.RegisterTranscient<IService, Service>();
//        builder.RegisterScoped<Controller, Controller>();
//        return builder.Build();
//    }
//}



