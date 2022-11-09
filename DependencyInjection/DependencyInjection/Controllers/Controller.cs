namespace DependencyInjection;

public class Controller  // Контроллер - пустышка
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