// See https://aka.ms/new-console-template for more information

using Benchmark;
using Benchmark.Seed;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Running;
using Domain.Document;
using Domain.Relational;

var relationalRepository = new RelationalRepositoryContext(Configuration.PostgresOptions);
var mongoRepository = new MongoRepository();

// await Configuration.ResetDatabase(relationalRepository, mongoRepository);
// await Seeder.Seed(relationalRepository, mongoRepository);

// await Configuration.CreateMaterializedView(mongoRepository);

var config = ManualConfig.CreateMinimumViable();
config.AddExporter(CsvMeasurementsExporter.Default);
config.AddExporter(RPlotExporter.Default);
var summary = BenchmarkRunner.Run<MongoQueryBenchmark>(config);
