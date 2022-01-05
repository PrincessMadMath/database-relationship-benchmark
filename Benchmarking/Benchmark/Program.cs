// See https://aka.ms/new-console-template for more information

using Benchmark;
using BenchmarkDotNet.Running;
using Domain.Document;
using Domain.Relational;

await using var db = new RelationalRepositoryContext(Configuration.PostgresOptions);

var documentRepository = new MongoRepository();


// Take 1-2 mins
async Task SeedDatabase()
{
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();

    await documentRepository.MongoDatabase.DropCollectionAsync(MongoRepository.GroupCollectionName);
    await documentRepository.MongoDatabase.DropCollectionAsync(MongoRepository.UserCollectionName);

    await documentRepository.GroupCollection.Indexes.CreateManyAsync(GroupDocument.GetIndexes());
    await documentRepository.UserCollection.Indexes.CreateManyAsync(UserDocument.GetIndexes());

    await Seeder.Seed(db, documentRepository);
}

void Benchmark()
{
    var summary = BenchmarkRunner.Run<QueryBenchmark>();
}


//await SeedDatabase();
Benchmark();
