using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoExample.Core.Models;
using MongoExample.Core.Services;

namespace MongoExample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly PostService _postService;

    public PostController(PostService postService)
    {
        _postService = postService;
    }

    [HttpGet("blog/{blogId}/posts")]
    public ActionResult<IEnumerable<Post>> GetBlogPosts(string blogId)
    {
        if (!ObjectId.TryParse(blogId, out ObjectId blogObjectId))
        {
            return BadRequest("Invalid blog ID format");
        }

        try
        {
            var posts = _postService.GetBlogPosts(blogObjectId);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("{postId}/comments")]
    public ActionResult<IEnumerable<Comment>> GetPostComments(string postId)
    {
        if (!ObjectId.TryParse(postId, out ObjectId postObjectId))
        {
            return BadRequest("Invalid post ID format");
        }

        try
        {
            var comments = _postService.GetPostComments(postObjectId);
            return Ok(comments);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPut("{postId}/content")]
    public IActionResult UpdatePostContent(string postId, [FromBody] UpdatePostContentRequest request)
    {
        if (!ObjectId.TryParse(postId, out ObjectId postObjectId))
        {
            return BadRequest("Invalid post ID format");
        }

        try
        {
            _postService.UpdatePostContent(postObjectId, request.Content);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("{postId}/comments")]
    public async Task<IActionResult> AddComment(string postId, [FromBody] Comment comment)
    {
        try
        {
            await _postService.AddCommentAsync(postId, comment);
            return Ok();
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Rate limit"))
        {
            return StatusCode(429, new { message = "Too many comments. Please try again later." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while adding the comment." });
        }
    }
}

public class UpdatePostContentRequest
{
    public string Content { get; set; }
} 