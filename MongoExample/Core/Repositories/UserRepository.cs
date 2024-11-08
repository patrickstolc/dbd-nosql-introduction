using MongoDB.Driver;
using MongoDB.Bson;
using MongoExample.Core.Models;

namespace MongoExample.Core.Repositories;

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