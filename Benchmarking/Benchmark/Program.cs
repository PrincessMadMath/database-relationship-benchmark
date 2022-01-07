// See https://aka.ms/new-console-template for more information

using Benchmark;
using Benchmark.Seed;
using BenchmarkDotNet.Running;
using Domain.Document;
using Domain.Relational;

var relationalRepository = new RelationalRepositoryContext(Configuration.PostgresOptions);
var mongoRepository = new MongoRepository();

// await Configuration.ResetDatabase(relationalRepository, mongoRepository);
// await Seeder.Seed(relationalRepository, mongoRepository);

// await Configuration.CreateMaterializedView(mongoRepository);
var summary = BenchmarkRunner.Run<QueryBenchmark>();
