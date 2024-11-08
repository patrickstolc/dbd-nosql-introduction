using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoExample.Core.Models;

public class UserModel
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public string Handle { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UserRepository
{
    private readonly IMongoCollection<UserModel> _users;
    private readonly PostRepository _postRepository;

    public UserRepository(IMongoClient client, PostRepository postRepository)
    {
        var database = client.GetDatabase("blog");
        _users = database.GetCollection<UserModel>("users");
        _postRepository = postRepository;
    }

    public void UpdateUserName(ObjectId userId, string newName)
    {
        var update = Builders<UserModel>.Update
            .Set(u => u.Name, newName)
            .Set(u => u.UpdatedAt, DateTime.UtcNow);

        _users.UpdateOne(u => u.Id == userId, update);
        
        // Update username in all posts and comments
        _postRepository.UpdateUserName(userId.ToString(), newName);
    }
}