using Benchmark.Seed;
using BenchmarkDotNet.Attributes;
using Domain.Document;
using Domain.Relational;

namespace Benchmark;

[DryJob]
[RPlotExporter]
[MarkdownExporterAttribute.GitHub]
[WarmupCount(1)]
[IterationCount(5)]
public class MongoQueryBenchmark
{
    private MongoRepository _mongoRepository;
    private List<Group> _groups;
    private List<User> _users;

    public int GroupCount { get; set; } = 1000;

    // [Params(0, 10, 100, 1000)]
    public int OwnerCount { get; set; } = 5;

    [Params(0, 10, 100, 1000)]
    public int LinkCount { get; set; } = 10;

    [GlobalSetup]
    public async Task Setup()
    {
        this._mongoRepository = new MongoRepository();

        await Configuration.ResetDatabase(this._mongoRepository);
        
        this.TenantId = Guid.NewGuid();
        var (groups, users) =
            FakeDataGenerator.Generate(
                this.TenantId,
                this.GroupCount,
                this.OwnerCount,
                this.LinkCount
            );
        
        this._groups = groups;
        this._users = users;
        
        await Seeder.Seed(
            this._mongoRepository,
            this._groups,
            this._users);

        await Configuration.CreateMaterializedView(this._mongoRepository);
    }
    
    [GlobalCleanup]
    public Task Cleanup()
    {
        return Configuration.ResetDatabase(this._mongoRepository);
    }

    public Guid TenantId { get; set; }
    
    [Benchmark]
    public async Task ForeachChannel()
    {
        var groups =
            await DocumentQuery.GetTenantWithChannel(this._mongoRepository,
                this.TenantId,
                4);
    }

    [Benchmark]
    public async Task Lookup()
    {
        var groups =
            await DocumentQuery.GetTenantWithLookup(this._mongoRepository,
                this.TenantId);
    }

    [Benchmark]
    public async Task View()
    {
        var groups =
            await DocumentQuery.GetTenantOnView(this._mongoRepository,
                this.TenantId);
    }

    [Benchmark]
    public async Task ViewMaterialized()
    {
        var groups =
            await DocumentQuery.GetTenantOnViewMaterialized(this._mongoRepository,
                this.TenantId);
    }
}
