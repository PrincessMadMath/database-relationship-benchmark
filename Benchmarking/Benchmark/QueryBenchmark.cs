using BenchmarkDotNet.Attributes;
using Domain.Document;
using Domain.Relational;

namespace Benchmark;

[DryJob]
[WarmupCount(1)]
[IterationCount(5)]
public class QueryBenchmark
{
    private RelationalRepositoryContext _relationalRepositoryContext;
    private MongoRepository _mongoRepository;

    [ParamsSource(nameof(Seeds))] public Seeder.SeedInfo SeedInfo { get; set; }

    public IEnumerable<Seeder.SeedInfo> Seeds => Seeder.Seeds;

    [GlobalSetup]
    public void Setup()
    {
        this._relationalRepositoryContext = new RelationalRepositoryContext(Configuration.PostgresOptions);
        this._mongoRepository = new MongoRepository();
    }

    [Benchmark]
    public async Task PostgresEager()
    {
        var groups =
            await RelationalQuery.GetTenantGroupsWithEagerLoading(this._relationalRepositoryContext,
                this.SeedInfo.TenantId);
    }

    // [Benchmark]
    // public async Task MongoForeach()
    // {
    //     var groups =
    //         await DocumentQuery.GetTenantForeach(this._mongoRepository,
    //             this.SeedInfo.TenantId);
    // }

    [Benchmark]
    //[Arguments(1)]
    //[Arguments(2)]
    [Arguments(4)]
    //[Arguments(8)]
    //[Arguments(16)]
    //[Arguments(32)]
    public async Task MongoChannel(int maxConcurrency)
    {
        var groups =
            await DocumentQuery.GetTenantWithChannel(this._mongoRepository,
                this.SeedInfo.TenantId,
                maxConcurrency);
    }

    [Benchmark]
    public async Task MongoLookup()
    {
        var groups =
            await DocumentQuery.GetTenantWithLookup(this._mongoRepository,
                this.SeedInfo.TenantId);
    }
}
