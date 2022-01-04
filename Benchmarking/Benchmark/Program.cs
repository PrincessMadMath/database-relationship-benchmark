// See https://aka.ms/new-console-template for more information

using Benchmark;
using BenchmarkDotNet.Running;
using Domain.Relational;

await using var db = new RepositoryContext(Configuration.PostgresOptions);


// Take 1-2 mins
Task SeedDatabase()
{
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();
    return Seeder.Seed(db);
}

void Benchmark()
{
    var summary = BenchmarkRunner.Run<QueryBenchmark>();
}


//await SeedDatabase();
Benchmark();
