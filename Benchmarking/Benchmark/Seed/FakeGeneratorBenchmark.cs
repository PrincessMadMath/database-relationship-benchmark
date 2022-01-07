using BenchmarkDotNet.Attributes;

namespace Benchmark.Seed;

[DryJob]
[WarmupCount(1)]
[IterationCount(5)]
public class FakeGeneratorBenchmark
{
    [ParamsSource(nameof(Seeds))] public SeedInfo SeedInfo { get; set; }

    public IEnumerable<SeedInfo> Seeds => Seeder.Seeds;

    [GlobalSetup]
    public void Setup()
    {
    }

    // [Benchmark]
    // public void DefaultGenerator()
    // {
    //     FakeDataGenerator.GenerateGroups(this.SeedInfo);
    // }

    [Benchmark]
    public void GeneratorConstName()
    {
        FakeDataGenerator.GenerateWithConstStringAndSameOwners(this.SeedInfo);
    }

    [Benchmark]
    public void GeneratorConstNameSameOwner()
    {
        FakeDataGenerator.GenerateWithConstStringAndSameOwners(this.SeedInfo);
    }
}
