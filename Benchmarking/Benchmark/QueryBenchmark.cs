using BenchmarkDotNet.Attributes;
using Domain.Relational;

namespace Benchmark;

public class QueryBenchmark
{
    private RepositoryContext _repositoryContext;

    [ParamsSource(nameof(Seeds))] public Seeder.SeedInfo SeedInfo { get; set; }

    public IEnumerable<Seeder.SeedInfo> Seeds => Seeder.Seeds;

    [GlobalSetup]
    public void Setup()
    {
        this._repositoryContext = new RepositoryContext(Configuration.PostgresOptions);
    }

    [Benchmark]
    public async Task PostgresEager()
    {
        var groups =
            await RelationalQuery.GetTenantGroupsWithEagerLoading(this._repositoryContext, this.SeedInfo.TenantId);
    }
}
