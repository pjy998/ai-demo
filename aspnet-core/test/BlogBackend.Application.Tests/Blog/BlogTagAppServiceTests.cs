using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlogBackend.Blog;
using Shouldly;
using Xunit;

namespace BlogBackend.Application.Tests.Blog;

public class BlogTagAppServiceTests : BlogBackendApplicationTestBase<BlogBackendApplicationTestModule>
{
    private readonly IBlogTagAppService _blogTagAppService;

    public BlogTagAppServiceTests()
    {
        _blogTagAppService = GetRequiredService<IBlogTagAppService>();
    }

    [Fact]
    public async Task Should_Create_Blog_Tag()
    {
        // Arrange
        var createDto = new CreateBlogTagDto
        {
            Name = "Test Tag",
            Description = "Test tag description",
            IsActive = true
        };

        // Act
        var result = await _blogTagAppService.CreateAsync(createDto);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Test Tag");
        result.Description.ShouldBe("Test tag description");
        result.IsActive.ShouldBe(true);
        result.Slug.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task Should_Get_Blog_Tag_By_Id()
    {
        // Arrange
        var createDto = new CreateBlogTagDto
        {
            Name = "Test Tag for Get",
            Description = "Test tag description for get",
            IsActive = true
        };
        var createdTag = await _blogTagAppService.CreateAsync(createDto);

        // Act
        var result = await _blogTagAppService.GetAsync(createdTag.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(createdTag.Id);
        result.Name.ShouldBe("Test Tag for Get");
    }

    [Fact]
    public async Task Should_Get_Blog_Tag_By_Name()
    {
        // Arrange
        var createDto = new CreateBlogTagDto
        {
            Name = "Unique Tag Name",
            Description = "Unique tag description",
            IsActive = true
        };
        var createdTag = await _blogTagAppService.CreateAsync(createDto);

        // Act
        var result = await _blogTagAppService.GetByNameAsync("Unique Tag Name");

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(createdTag.Id);
        result.Name.ShouldBe("Unique Tag Name");
    }

    [Fact]
    public async Task Should_Update_Blog_Tag()
    {
        // Arrange
        var createDto = new CreateBlogTagDto
        {
            Name = "Original Tag Name",
            Description = "Original description",
            IsActive = true
        };
        var createdTag = await _blogTagAppService.CreateAsync(createDto);

        var updateDto = new UpdateBlogTagDto
        {
            Name = "Updated Tag Name",
            Description = "Updated description",
            IsActive = false
        };

        // Act
        var result = await _blogTagAppService.UpdateAsync(createdTag.Id, updateDto);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(createdTag.Id);
        result.Name.ShouldBe("Updated Tag Name");
        result.Description.ShouldBe("Updated description");
        result.IsActive.ShouldBe(false);
    }

    [Fact]
    public async Task Should_Delete_Blog_Tag()
    {
        // Arrange
        var createDto = new CreateBlogTagDto
        {
            Name = "Test Tag for Delete",
            Description = "Test tag description for delete",
            IsActive = true
        };
        var createdTag = await _blogTagAppService.CreateAsync(createDto);

        // Act
        await _blogTagAppService.DeleteAsync(createdTag.Id);

        // Assert
        await Should.ThrowAsync<EntityNotFoundException>(async () =>
        {
            await _blogTagAppService.GetAsync(createdTag.Id);
        });
    }

    [Fact]
    public async Task Should_Activate_Tag()
    {
        // Arrange
        var createDto = new CreateBlogTagDto
        {
            Name = "Inactive Tag",
            Description = "Inactive tag description",
            IsActive = false
        };
        var tag = await _blogTagAppService.CreateAsync(createDto);

        // Act
        var result = await _blogTagAppService.ActivateAsync(tag.Id);

        // Assert
        result.ShouldNotBeNull();
        result.IsActive.ShouldBe(true);
    }

    [Fact]
    public async Task Should_Deactivate_Tag()
    {
        // Arrange
        var createDto = new CreateBlogTagDto
        {
            Name = "Active Tag",
            Description = "Active tag description",
            IsActive = true
        };
        var tag = await _blogTagAppService.CreateAsync(createDto);

        // Act
        var result = await _blogTagAppService.DeactivateAsync(tag.Id);

        // Assert
        result.ShouldNotBeNull();
        result.IsActive.ShouldBe(false);
    }

    [Fact]
    public async Task Should_Get_Or_Create_Tags_By_Names()
    {
        // Arrange
        var existingDto = new CreateBlogTagDto
        {
            Name = "Existing Tag",
            Description = "Existing tag description",
            IsActive = true
        };
        var existingTag = await _blogTagAppService.CreateAsync(existingDto);

        var tagNames = new List<string>
        {
            "Existing Tag",
            "New Tag 1",
            "New Tag 2"
        };

        // Act
        var result = await _blogTagAppService.GetOrCreateByNamesAsync(tagNames);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(3);
        result.ShouldContain(t => t.Id == existingTag.Id);
        result.ShouldContain(t => t.Name == "New Tag 1");
        result.ShouldContain(t => t.Name == "New Tag 2");
    }

    [Fact]
    public async Task Should_Generate_Unique_Slug()
    {
        // Arrange
        var name = "Test Tag Name";

        // Act
        var slug = await _blogTagAppService.GenerateSlugAsync(name);

        // Assert
        slug.ShouldNotBeNullOrEmpty();
        slug.ShouldBe("test-tag-name");
    }

    [Fact]
    public async Task Should_Check_Name_Availability()
    {
        // Arrange
        var createDto = new CreateBlogTagDto
        {
            Name = "Test Tag for Name Check",
            Description = "Description",
            IsActive = true
        };
        var createdTag = await _blogTagAppService.CreateAsync(createDto);

        // Act
        var isAvailable = await _blogTagAppService.IsNameAvailableAsync("Test Tag for Name Check");
        var isAvailableExcludingSelf = await _blogTagAppService.IsNameAvailableAsync("Test Tag for Name Check", createdTag.Id);

        // Assert
        isAvailable.ShouldBe(false); // Already taken
        isAvailableExcludingSelf.ShouldBe(true); // Available when excluding self
    }

    [Fact]
    public async Task Should_Check_Slug_Availability()
    {
        // Arrange
        var createDto = new CreateBlogTagDto
        {
            Name = "Test Tag for Slug Check",
            Description = "Description",
            IsActive = true
        };
        var createdTag = await _blogTagAppService.CreateAsync(createDto);

        // Act
        var isAvailable = await _blogTagAppService.IsSlugAvailableAsync(createdTag.Slug);
        var isAvailableExcludingSelf = await _blogTagAppService.IsSlugAvailableAsync(createdTag.Slug, createdTag.Id);

        // Assert
        isAvailable.ShouldBe(false); // Already taken
        isAvailableExcludingSelf.ShouldBe(true); // Available when excluding self
    }

    [Fact]
    public async Task Should_Get_Active_Tags()
    {
        // Arrange
        var activeDto = new CreateBlogTagDto
        {
            Name = "Active Tag",
            Description = "Active tag description",
            IsActive = true
        };
        await _blogTagAppService.CreateAsync(activeDto);

        var inactiveDto = new CreateBlogTagDto
        {
            Name = "Inactive Tag",
            Description = "Inactive tag description",
            IsActive = false
        };
        await _blogTagAppService.CreateAsync(inactiveDto);

        // Act
        var result = await _blogTagAppService.GetActiveTagsAsync();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.ShouldAllBe(t => t.IsActive);
    }

    [Fact]
    public async Task Should_Get_Statistics()
    {
        // Arrange
        await _blogTagAppService.CreateAsync(new CreateBlogTagDto { Name = "Tag1", IsActive = true });
        await _blogTagAppService.CreateAsync(new CreateBlogTagDto { Name = "Tag2", IsActive = false });

        // Act
        var result = await _blogTagAppService.GetStatisticsAsync();

        // Assert
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(2);
        result.ActiveCount.ShouldBeGreaterThanOrEqualTo(1);
        result.InactiveCount.ShouldBeGreaterThanOrEqualTo(1);
    }
}
