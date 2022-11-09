namespace DependencyInjection;

public interface IScope : IDisposable, IAsyncDisposable
{
    object Resolve(Type service);
}