using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlogBackend.Blog;
using Shouldly;
using Xunit;

namespace BlogBackend.Application.Tests.Blog;

public class BlogCommentAppServiceTests : BlogBackendApplicationTestBase<BlogBackendApplicationTestModule>
{
    private readonly IBlogCommentAppService _blogCommentAppService;
    private readonly IBlogPostAppService _blogPostAppService;

    public BlogCommentAppServiceTests()
    {
        _blogCommentAppService = GetRequiredService<IBlogCommentAppService>();
        _blogPostAppService = GetRequiredService<IBlogPostAppService>();
    }

    private async Task<BlogPostDto> CreateTestBlogPost()
    {
        var createDto = new CreateBlogPostDto
        {
            Title = "Test Blog Post for Comments",
            Content = "Test content",
            Summary = "Test summary",
            IsPublished = true
        };
        return await _blogPostAppService.CreateAsync(createDto);
    }

    [Fact]
    public async Task Should_Create_Blog_Comment()
    {
        // Arrange
        var blogPost = await CreateTestBlogPost();
        var createDto = new CreateBlogCommentDto
        {
            PostId = blogPost.Id,
            AuthorName = "Test Author",
            AuthorEmail = "test@example.com",
            Content = "This is a test comment",
            IpAddress = "127.0.0.1"
        };

        // Act
        var result = await _blogCommentAppService.CreateAsync(createDto);

        // Assert
        result.ShouldNotBeNull();
        result.PostId.ShouldBe(blogPost.Id);
        result.AuthorName.ShouldBe("Test Author");
        result.AuthorEmail.ShouldBe("test@example.com");
        result.Content.ShouldBe("This is a test comment");
        result.Status.ShouldBe(CommentStatus.Pending);
    }

    [Fact]
    public async Task Should_Get_Blog_Comment_By_Id()
    {
        // Arrange
        var blogPost = await CreateTestBlogPost();
        var createDto = new CreateBlogCommentDto
        {
            PostId = blogPost.Id,
            AuthorName = "Test Author for Get",
            AuthorEmail = "testget@example.com",
            Content = "This is a test comment for get",
            IpAddress = "127.0.0.1"
        };
        var createdComment = await _blogCommentAppService.CreateAsync(createDto);

        // Act
        var result = await _blogCommentAppService.GetAsync(createdComment.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(createdComment.Id);
        result.AuthorName.ShouldBe("Test Author for Get");
    }

    [Fact]
    public async Task Should_Update_Blog_Comment()
    {
        // Arrange
        var blogPost = await CreateTestBlogPost();
        var createDto = new CreateBlogCommentDto
        {
            PostId = blogPost.Id,
            AuthorName = "Original Author",
            AuthorEmail = "original@example.com",
            Content = "Original content",
            IpAddress = "127.0.0.1"
        };
        var createdComment = await _blogCommentAppService.CreateAsync(createDto);

        var updateDto = new UpdateBlogCommentDto
        {
            Content = "Updated content"
        };

        // Act
        var result = await _blogCommentAppService.UpdateAsync(createdComment.Id, updateDto);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(createdComment.Id);
        result.Content.ShouldBe("Updated content");
    }

    [Fact]
    public async Task Should_Delete_Blog_Comment()
    {
        // Arrange
        var blogPost = await CreateTestBlogPost();
        var createDto = new CreateBlogCommentDto
        {
            PostId = blogPost.Id,
            AuthorName = "Test Author for Delete",
            AuthorEmail = "testdelete@example.com",
            Content = "This is a test comment for delete",
            IpAddress = "127.0.0.1"
        };
        var createdComment = await _blogCommentAppService.CreateAsync(createDto);

        // Act
        await _blogCommentAppService.DeleteAsync(createdComment.Id);

        // Assert
        await Should.ThrowAsync<EntityNotFoundException>(async () =>
        {
            await _blogCommentAppService.GetAsync(createdComment.Id);
        });
    }

    [Fact]
    public async Task Should_Moderate_Blog_Comment()
    {
        // Arrange
        var blogPost = await CreateTestBlogPost();
        var createDto = new CreateBlogCommentDto
        {
            PostId = blogPost.Id,
            AuthorName = "Test Author for Moderation",
            AuthorEmail = "testmod@example.com",
            Content = "This is a test comment for moderation",
            IpAddress = "127.0.0.1"
        };
        var createdComment = await _blogCommentAppService.CreateAsync(createDto);

        var moderateDto = new ModerateBlogCommentDto
        {
            Status = CommentStatus.Approved,
            ModerationReason = "Test approval"
        };

        // Act
        var result = await _blogCommentAppService.ModerateAsync(createdComment.Id, moderateDto);

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe(CommentStatus.Approved);
        result.ModerationReason.ShouldBe("Test approval");
    }

    [Fact]
    public async Task Should_Get_Comments_By_Post_Id()
    {
        // Arrange
        var blogPost = await CreateTestBlogPost();
        
        var comment1 = new CreateBlogCommentDto
        {
            PostId = blogPost.Id,
            AuthorName = "Author 1",
            AuthorEmail = "author1@example.com",
            Content = "Comment 1",
            IpAddress = "127.0.0.1"
        };
        await _blogCommentAppService.CreateAsync(comment1);

        var comment2 = new CreateBlogCommentDto
        {
            PostId = blogPost.Id,
            AuthorName = "Author 2",
            AuthorEmail = "author2@example.com",
            Content = "Comment 2",
            IpAddress = "127.0.0.1"
        };
        await _blogCommentAppService.CreateAsync(comment2);

        // Act
        var result = await _blogCommentAppService.GetCommentsByPostIdAsync(blogPost.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result.ShouldAllBe(c => c.PostId == blogPost.Id);
    }

    [Fact]
    public async Task Should_Create_Reply_Comment()
    {
        // Arrange
        var blogPost = await CreateTestBlogPost();
        
        var parentCommentDto = new CreateBlogCommentDto
        {
            PostId = blogPost.Id,
            AuthorName = "Parent Author",
            AuthorEmail = "parent@example.com",
            Content = "Parent comment",
            IpAddress = "127.0.0.1"
        };
        var parentComment = await _blogCommentAppService.CreateAsync(parentCommentDto);

        var replyDto = new CreateBlogCommentDto
        {
            PostId = blogPost.Id,
            ParentId = parentComment.Id,
            AuthorName = "Reply Author",
            AuthorEmail = "reply@example.com",
            Content = "Reply comment",
            IpAddress = "127.0.0.1"
        };

        // Act
        var result = await _blogCommentAppService.CreateAsync(replyDto);

        // Assert
        result.ShouldNotBeNull();
        result.ParentId.ShouldBe(parentComment.Id);
        result.AuthorName.ShouldBe("Reply Author");
    }

    [Fact]
    public async Task Should_Get_Replies_By_Comment_Id()
    {
        // Arrange
        var blogPost = await CreateTestBlogPost();
        
        var parentCommentDto = new CreateBlogCommentDto
        {
            PostId = blogPost.Id,
            AuthorName = "Parent Author",
            AuthorEmail = "parent@example.com",
            Content = "Parent comment",
            IpAddress = "127.0.0.1"
        };
        var parentComment = await _blogCommentAppService.CreateAsync(parentCommentDto);

        var reply1Dto = new CreateBlogCommentDto
        {
            PostId = blogPost.Id,
            ParentId = parentComment.Id,
            AuthorName = "Reply Author 1",
            AuthorEmail = "reply1@example.com",
            Content = "Reply 1",
            IpAddress = "127.0.0.1"
        };
        await _blogCommentAppService.CreateAsync(reply1Dto);

        var reply2Dto = new CreateBlogCommentDto
        {
            PostId = blogPost.Id,
            ParentId = parentComment.Id,
            AuthorName = "Reply Author 2",
            AuthorEmail = "reply2@example.com",
            Content = "Reply 2",
            IpAddress = "127.0.0.1"
        };
        await _blogCommentAppService.CreateAsync(reply2Dto);

        // Act
        var result = await _blogCommentAppService.GetRepliesByCommentIdAsync(parentComment.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result.ShouldAllBe(c => c.ParentId == parentComment.Id);
    }

    [Fact]
    public async Task Should_Get_Pending_Comments()
    {
        // Arrange
        var blogPost = await CreateTestBlogPost();
        
        var pendingDto = new CreateBlogCommentDto
        {
            PostId = blogPost.Id,
            AuthorName = "Pending Author",
            AuthorEmail = "pending@example.com",
            Content = "Pending comment",
            IpAddress = "127.0.0.1"
        };
        await _blogCommentAppService.CreateAsync(pendingDto);

        // Act
        var result = await _blogCommentAppService.GetPendingCommentsAsync();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.ShouldAllBe(c => c.Status == CommentStatus.Pending);
    }

    [Fact]
    public async Task Should_Get_Comment_Count_By_Post_Id()
    {
        // Arrange
        var blogPost = await CreateTestBlogPost();
        
        await _blogCommentAppService.CreateAsync(new CreateBlogCommentDto
        {
            PostId = blogPost.Id,
            AuthorName = "Author 1",
            AuthorEmail = "author1@example.com",
            Content = "Comment 1",
            IpAddress = "127.0.0.1"
        });

        await _blogCommentAppService.CreateAsync(new CreateBlogCommentDto
        {
            PostId = blogPost.Id,
            AuthorName = "Author 2",
            AuthorEmail = "author2@example.com",
            Content = "Comment 2",
            IpAddress = "127.0.0.1"
        });

        // Act
        var result = await _blogCommentAppService.GetCommentCountByPostIdAsync(blogPost.Id);

        // Assert
        result.ShouldBe(2);
    }

    [Fact]
    public async Task Should_Get_Statistics()
    {
        // Arrange
        var blogPost = await CreateTestBlogPost();
        
        await _blogCommentAppService.CreateAsync(new CreateBlogCommentDto
        {
            PostId = blogPost.Id,
            AuthorName = "Author 1",
            AuthorEmail = "author1@example.com",
            Content = "Comment 1",
            IpAddress = "127.0.0.1"
        });

        // Act
        var result = await _blogCommentAppService.GetStatisticsAsync();

        // Assert
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBeGreaterThan(0);
        result.PendingCount.ShouldBeGreaterThan(0);
    }
}
