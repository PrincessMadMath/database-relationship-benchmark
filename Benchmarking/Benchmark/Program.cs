// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Benchmark;
using Benchmark.Seed;
using BenchmarkDotNet.Running;
using Domain.Document;
using Domain.Relational;
using MongoDB.Driver;

await using var db = new RelationalRepositoryContext(Configuration.PostgresOptions);

var documentRepository = new MongoRepository();


// Take 1-2 mins
async Task SeedDatabase()
{
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();

    await documentRepository.MongoDatabase.DropCollectionAsync(MongoRepository.GroupCollectionName);
    await documentRepository.MongoDatabase.DropCollectionAsync(MongoRepository.UserCollectionName);
    await documentRepository.MongoDatabase.DropCollectionAsync(MongoRepository.LinkCollectionName);
    await documentRepository.MongoDatabase.DropCollectionAsync(MongoRepository.GroupViewCollectionName);
    await documentRepository.MongoDatabase.DropCollectionAsync(MongoRepository.GroupViewMaterializedCollectionName);

    await documentRepository.GroupCollection.Indexes.CreateManyAsync(GroupDocument.GetIndexes());
    await documentRepository.UserCollection.Indexes.CreateManyAsync(UserDocument.GetIndexes());
    await documentRepository.LinkCollection.Indexes.CreateManyAsync(LinkDocument.GetIndexes());

    // TODO: Remove duplicate of aggregation
    await documentRepository.MongoDatabase.CreateViewAsync(
        MongoRepository.GroupViewCollectionName,
        MongoRepository.GroupCollectionName,
        new EmptyPipelineDefinition<GroupDocument>()
            .Lookup<GroupDocument, GroupDocument, UserDocument, DocumentQuery.GroupWithUser>(
                documentRepository.UserCollection, groupDocument => groupDocument.OwnersId,
                userDocument => userDocument.UserId, x => x.Owners)
            .Lookup<GroupDocument, DocumentQuery.GroupWithUser, LinkDocument, GroupViewDocument>(
                documentRepository.LinkCollection, groupDocument => groupDocument.GroupId,
                linkDocument => linkDocument.GroupId, x => x.Links)
    );

    await Seeder.Seed(db, documentRepository);

    // Create on demand materialized view to simulate denormalized
    await documentRepository.AggregateAsyncEnumerable(
        documentRepository.GroupCollection,
        new EmptyPipelineDefinition<GroupDocument>()
            .Lookup<GroupDocument, GroupDocument, UserDocument, DocumentQuery.GroupWithUser>(
                documentRepository.UserCollection, groupDocument => groupDocument.OwnersId,
                userDocument => userDocument.UserId, x => x.Owners)
            .Lookup<GroupDocument, DocumentQuery.GroupWithUser, LinkDocument, GroupViewDocument>(
                documentRepository.LinkCollection, groupDocument => groupDocument.GroupId,
                linkDocument => linkDocument.GroupId, x => x.Links)
            .Merge(documentRepository.GroupViewMaterializedCollection, new MergeStageOptions<GroupViewDocument>
            {
                WhenMatched = MergeStageWhenMatched.Replace
            } )
    ).ToListAsync();
}

void Benchmark()
{
    var summary = BenchmarkRunner.Run<QueryBenchmark>();
}


//  await SeedDatabase();
// Benchmark();
// var sw = new Stopwatch();
// foreach (var seed in Seeder.Seeds)
// {
//     sw.Restart();
//     FakeDataGenerator.GenerateGroups(seed);
//     Console.WriteLine($"Create {seed.GroupCount} groups in {sw.ElapsedMilliseconds}ms");
// }

var summary = BenchmarkRunner.Run<FakeGeneratorBenchmark>();
