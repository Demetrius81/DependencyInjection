using BenchmarkDotNet.Running;

namespace DependencyInjection;

public class Run
{
    public void RunTest()
    {
        BenchmarkRunner.Run<ContainerBenchmark>();

    }

}