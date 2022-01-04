using Domain.Relational;
using MongoDB.Driver;

namespace Domain.Document;

public class DocumentRepository
{
    // public DocumentRepository()
    // {
    //     var mongoClient = new MongoClient("mongodb://localhost:27017");
    //
    //     var mongoDatabase = mongoClient.GetDatabase("default");
    //
    //     this.GroupCollection = mongoDatabase.GetCollection<GroupDocument>("Groups");
    //     this.UserCollection = mongoDatabase.GetCollection<GroupDocument>("Users");
    // }
    //
    // public IMongoCollection<GroupDocument> GroupCollection { get; set; }
    //
    // public IMongoCollection<GroupDocument> UserCollection { get; set; }
    //
    // public Task SeedGroup(Group group)
    // {
    //     this.GroupCollection.InsertOne();
    // }
}
