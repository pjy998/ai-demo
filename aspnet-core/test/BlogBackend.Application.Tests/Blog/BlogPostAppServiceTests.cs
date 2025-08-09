using System;
using System.Threading.Tasks;
using BlogBackend.Blog;
using Shouldly;
using Xunit;

namespace BlogBackend.Application.Tests.Blog;

public class BlogPostAppServiceTests : BlogBackendApplicationTestBase<BlogBackendApplicationTestModule>
{
    private readonly IBlogPostAppService _blogPostAppService;

    public BlogPostAppServiceTests()
    {
        _blogPostAppService = GetRequiredService<IBlogPostAppService>();
    }

    [Fact]
    public async Task Should_Create_Blog_Post()
    {
        // Arrange
        var createDto = new CreateBlogPostDto
        {
            Title = "Test Blog Post",
            Content = "This is a test blog post content",
            Summary = "Test summary",
            IsPublished = false
        };

        // Act
        var result = await _blogPostAppService.CreateAsync(createDto);

        // Assert
        result.ShouldNotBeNull();
        result.Title.ShouldBe("Test Blog Post");
        result.Content.ShouldBe("This is a test blog post content");
        result.Summary.ShouldBe("Test summary");
        result.IsPublished.ShouldBe(false);
        result.Slug.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task Should_Get_Blog_Post_By_Id()
    {
        // Arrange
        var createDto = new CreateBlogPostDto
        {
            Title = "Test Blog Post for Get",
            Content = "This is a test blog post content for get",
            Summary = "Test summary for get",
            IsPublished = true
        };
        var createdPost = await _blogPostAppService.CreateAsync(createDto);

        // Act
        var result = await _blogPostAppService.GetAsync(createdPost.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(createdPost.Id);
        result.Title.ShouldBe("Test Blog Post for Get");
    }

    [Fact]
    public async Task Should_Update_Blog_Post()
    {
        // Arrange
        var createDto = new CreateBlogPostDto
        {
            Title = "Original Title",
            Content = "Original content",
            Summary = "Original summary",
            IsPublished = false
        };
        var createdPost = await _blogPostAppService.CreateAsync(createDto);

        var updateDto = new UpdateBlogPostDto
        {
            Title = "Updated Title",
            Content = "Updated content",
            Summary = "Updated summary",
            IsPublished = true
        };

        // Act
        var result = await _blogPostAppService.UpdateAsync(createdPost.Id, updateDto);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(createdPost.Id);
        result.Title.ShouldBe("Updated Title");
        result.Content.ShouldBe("Updated content");
        result.Summary.ShouldBe("Updated summary");
        result.IsPublished.ShouldBe(true);
    }

    [Fact]
    public async Task Should_Delete_Blog_Post()
    {
        // Arrange
        var createDto = new CreateBlogPostDto
        {
            Title = "Test Blog Post for Delete",
            Content = "This is a test blog post content for delete",
            Summary = "Test summary for delete",
            IsPublished = false
        };
        var createdPost = await _blogPostAppService.CreateAsync(createDto);

        // Act
        await _blogPostAppService.DeleteAsync(createdPost.Id);

        // Assert
        await Should.ThrowAsync<EntityNotFoundException>(async () =>
        {
            await _blogPostAppService.GetAsync(createdPost.Id);
        });
    }

    [Fact]
    public async Task Should_Publish_Blog_Post()
    {
        // Arrange
        var createDto = new CreateBlogPostDto
        {
            Title = "Test Blog Post for Publish",
            Content = "This is a test blog post content for publish",
            Summary = "Test summary for publish",
            IsPublished = false
        };
        var createdPost = await _blogPostAppService.CreateAsync(createDto);

        var publishDto = new PublishBlogPostDto
        {
            PublishedTime = DateTime.UtcNow
        };

        // Act
        var result = await _blogPostAppService.PublishAsync(createdPost.Id, publishDto);

        // Assert
        result.ShouldNotBeNull();
        result.IsPublished.ShouldBe(true);
        result.PublishedTime.ShouldNotBeNull();
    }

    [Fact]
    public async Task Should_Withdraw_Blog_Post()
    {
        // Arrange
        var createDto = new CreateBlogPostDto
        {
            Title = "Test Blog Post for Withdraw",
            Content = "This is a test blog post content for withdraw",
            Summary = "Test summary for withdraw",
            IsPublished = true
        };
        var createdPost = await _blogPostAppService.CreateAsync(createDto);

        // Act
        var result = await _blogPostAppService.WithdrawAsync(createdPost.Id);

        // Assert
        result.ShouldNotBeNull();
        result.IsPublished.ShouldBe(false);
    }

    [Fact]
    public async Task Should_Increment_View_Count()
    {
        // Arrange
        var createDto = new CreateBlogPostDto
        {
            Title = "Test Blog Post for View Count",
            Content = "This is a test blog post content for view count",
            Summary = "Test summary for view count",
            IsPublished = true
        };
        var createdPost = await _blogPostAppService.CreateAsync(createDto);
        var originalViewCount = createdPost.ViewCount;

        // Act
        await _blogPostAppService.IncrementViewCountAsync(createdPost.Id);

        // Assert
        var updatedPost = await _blogPostAppService.GetAsync(createdPost.Id);
        updatedPost.ViewCount.ShouldBe(originalViewCount + 1);
    }

    [Fact]
    public async Task Should_Generate_Unique_Slug()
    {
        // Arrange
        var title = "Test Blog Post Title";

        // Act
        var slug = await _blogPostAppService.GenerateSlugAsync(title);

        // Assert
        slug.ShouldNotBeNullOrEmpty();
        slug.ShouldBe("test-blog-post-title");
    }

    [Fact]
    public async Task Should_Check_Slug_Availability()
    {
        // Arrange
        var createDto = new CreateBlogPostDto
        {
            Title = "Test Blog Post for Slug Check",
            Content = "Content",
            Summary = "Summary",
            IsPublished = false
        };
        var createdPost = await _blogPostAppService.CreateAsync(createDto);

        // Act
        var isAvailable = await _blogPostAppService.IsSlugAvailableAsync(createdPost.Slug);
        var isAvailableExcludingSelf = await _blogPostAppService.IsSlugAvailableAsync(createdPost.Slug, createdPost.Id);

        // Assert
        isAvailable.ShouldBe(false); // Already taken
        isAvailableExcludingSelf.ShouldBe(true); // Available when excluding self
    }
}
