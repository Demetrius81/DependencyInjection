namespace DependencyInjection;
public interface IContainer : IDisposable, IAsyncDisposable
{
    IScope CreateScope();
}