using MongoDB.Bson;
using MongoExample.Core.Models;
using MongoExample.Core.Repositories;

namespace MongoExample.Core.Services;

public class PostService
{
    private readonly PostRepository _postRepository;

    public PostService(PostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public List<Post> GetBlogPosts(ObjectId blogId)
    {
        return _postRepository.GetBlogPosts(blogId);
    }

    public List<Comment> GetPostComments(ObjectId postId)
    {
        var post = _postRepository.Get(postId);
        return post?.Comments ?? new List<Comment>();
    }

    public void UpdatePostContent(ObjectId postId, string newContent)
    {
        var post = _postRepository.Get(postId);
        if (post != null)
        {
            post.Content = newContent;
            _postRepository.Update(post);
        }
    }

    public void AddComment(ObjectId postId, Comment comment)
    {
        var post = _postRepository.Get(postId);
        if (post != null)
        {
            comment.CreatedAt = DateTime.UtcNow;
            comment.UpdatedAt = DateTime.UtcNow;
            post.Comments.Add(comment);
            _postRepository.Update(post);
        }
    }
} 