using MongoDB.Bson;
using MongoExample.Core.Models;
using MongoExample.Core.Repositories;
using StackExchange.Redis;
using System.Text.Json;

namespace MongoExample.Core.Services;

public class PostService
{
    private readonly PostRepository _postRepository;
    private readonly IRedisService _redisService;
    private readonly IConfiguration _configuration;

    public PostService(
        PostRepository postRepository, 
        IRedisService redisService,
        IConfiguration configuration)
    {
        _postRepository = postRepository;
        _redisService = redisService;
        _configuration = configuration;
    }

    public async Task<List<Post>> GetBlogPostsAsync(string blogId)
    {
        var cacheKey = $"blog:{blogId}:posts";
        
        // Try to get from cache first
        var cachedPosts = await _redisService.GetAsync<List<Post>>(cacheKey);
        if (cachedPosts != null)
        {
            return cachedPosts;
        }

        // If not in cache, get from database
        var posts = _postRepository.GetBlogPosts(
            ObjectId.Parse(blogId)
        );
        
        // Cache the results
        var cacheTimeout = TimeSpan.FromSeconds(
            _configuration.GetValue<int>("Redis:PostCacheTimeout", 3600));
        await _redisService.SetAsync(cacheKey, posts, cacheTimeout);
        
        return posts;
    }

    public async Task UpdatePostAsync(string postId, Post updatedPost)
    {
        _postRepository.Update(updatedPost);
        
        // Invalidate cache
        var cacheKey = $"blog:{updatedPost.BlogId}:posts";
        await _redisService.DeleteAsync(cacheKey);
    }

    public async Task AddCommentAsync(string postId, Comment comment)
    {
        var rateLimit = _configuration.GetSection("Redis:CommentRateLimit");
        var maxAttempts = rateLimit.GetValue<int>("MaxAttempts", 5);
        var timeWindow = TimeSpan.FromSeconds(rateLimit.GetValue<int>("TimeWindowSeconds", 300));
        
        var rateLimitKey = $"ratelimit:comment:{comment.UserId}";
        
        if (!await _redisService.CheckRateLimitAsync(rateLimitKey, maxAttempts, timeWindow))
        {
            throw new InvalidOperationException("Rate limit exceeded for comments");
        }

        _postRepository.AddComment(postId, comment);
        
        // Invalidate cache for the blog posts
        var post = _postRepository.Get(
            ObjectId.Parse(postId)
        );
        if (post != null)
        {
            var cacheKey = $"blog:{post.BlogId}:posts";
            await _redisService.DeleteAsync(cacheKey);
        }
    }
} 