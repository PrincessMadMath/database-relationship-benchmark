using BenchmarkDotNet.Attributes;

namespace Benchmark.Seed;

[DryJob]
[WarmupCount(1)]
[IterationCount(5)]
public class FakeGeneratorBenchmark
{
    [Params(100, 1000, 10000, 10000)] public int GroupCount { get; set; }

    public int OwnerCount { get; set; } = 5;

    [Params(10, 1000)] public int LinkCount { get; set; }

    public int UserCount { get; set; } = 5000;


    [GlobalSetup]
    public void Setup()
    {
    }

    [Benchmark]
    public void DefaultGenerator()
    {
        FakeDataGenerator.Generate(Guid.NewGuid(),
            this.GroupCount,
            this.OwnerCount,
            this.LinkCount);
    }
}
