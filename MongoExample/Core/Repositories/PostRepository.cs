using MongoDB.Driver;
using MongoDB.Bson;
using MongoExample.Core.Models;

namespace MongoExample.Core.Repositories;

public class PostRepository
{
    private readonly IMongoCollection<Post> _posts;
    
    public PostRepository(IMongoClient client)
    {
        var database = client.GetDatabase("blog");
        _posts = database.GetCollection<Post>("posts");
    }

    public List<Post> GetBlogPosts(ObjectId blogId)
    {
        return _posts.Find(p => p.BlogId == blogId).ToList();
    }

    public Post Get(ObjectId id)
    {
        return _posts.Find(p => p.Id == id).FirstOrDefault();
    }

    public void Update(Post post)
    {
        post.UpdatedAt = DateTime.UtcNow;
        _posts.ReplaceOne(p => p.Id == post.Id, post);
    }

    public void UpdateUserName(string userId, string newUserName)
    {
        var update = Builders<Post>.Update
            .Set(p => p.UserName, newUserName)
            .Set("Comments.$[].UserName", newUserName);

        var filter = Builders<Post>.Filter.Or(
            Builders<Post>.Filter.Eq(p => p.UserId, userId),
            Builders<Post>.Filter.ElemMatch(p => p.Comments, c => c.UserId == userId)
        );

        _posts.UpdateMany(filter, update);
    }

    public void AddComment(string postId, Comment comment)
    {
        _posts.UpdateOne(p => p.Id == ObjectId.Parse(postId), Builders<Post>.Update.Push(p => p.Comments, comment));
    }
} 