using BenchmarkDotNet.Attributes;
using Domain.Document;
using Domain.Relational;

namespace Benchmark.Seed;

[DryJob]
[WarmupCount(1)]
[IterationCount(1)]
public class SeederBenchmark
{
    private MongoRepository _mongoRepository;
    private RelationalRepositoryContext _postgresRepository;
    private List<Group> _groups;
    public Guid TenantId = Guid.NewGuid();
    private List<User> _users;

    [Params(100, 1000)] public int GroupCount { get; set; }

    [Params(5)]
    public int OwnerCount { get; set; }

    [Params(10)]
    public int LinkCount { get; set; }


    [GlobalSetup]
    public async Task Setup()
    {
        this._postgresRepository = new RelationalRepositoryContext(Configuration.PostgresOptions);
        this._mongoRepository = new MongoRepository();

        await Configuration.ResetDatabase(this._postgresRepository, this._mongoRepository);
    }

    [IterationSetup]
    public void IterationSetup()
    {
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
    }

    [GlobalCleanup]
    public Task Cleanup()
    {
        return Configuration.ResetDatabase(this._postgresRepository, this._mongoRepository);
    }

    [Benchmark]
    public Task DefaultSeeder()
    {
        return Seeder.Seed(
            this._postgresRepository,
            this._mongoRepository,
            this._groups,
            this._users);
    }
}
