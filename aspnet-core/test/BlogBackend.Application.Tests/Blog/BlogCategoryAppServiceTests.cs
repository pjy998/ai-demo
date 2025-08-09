using System;
using System.Threading.Tasks;
using BlogBackend.Blog;
using Shouldly;
using Xunit;

namespace BlogBackend.Application.Tests.Blog;

public class BlogCategoryAppServiceTests : BlogBackendApplicationTestBase<BlogBackendApplicationTestModule>
{
    private readonly IBlogCategoryAppService _blogCategoryAppService;

    public BlogCategoryAppServiceTests()
    {
        _blogCategoryAppService = GetRequiredService<IBlogCategoryAppService>();
    }

    [Fact]
    public async Task Should_Create_Blog_Category()
    {
        // Arrange
        var createDto = new CreateBlogCategoryDto
        {
            Name = "Test Category",
            Description = "Test category description",
            IsActive = true
        };

        // Act
        var result = await _blogCategoryAppService.CreateAsync(createDto);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Test Category");
        result.Description.ShouldBe("Test category description");
        result.IsActive.ShouldBe(true);
        result.Slug.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task Should_Get_Blog_Category_By_Id()
    {
        // Arrange
        var createDto = new CreateBlogCategoryDto
        {
            Name = "Test Category for Get",
            Description = "Test category description for get",
            IsActive = true
        };
        var createdCategory = await _blogCategoryAppService.CreateAsync(createDto);

        // Act
        var result = await _blogCategoryAppService.GetAsync(createdCategory.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(createdCategory.Id);
        result.Name.ShouldBe("Test Category for Get");
    }

    [Fact]
    public async Task Should_Update_Blog_Category()
    {
        // Arrange
        var createDto = new CreateBlogCategoryDto
        {
            Name = "Original Category Name",
            Description = "Original description",
            IsActive = true
        };
        var createdCategory = await _blogCategoryAppService.CreateAsync(createDto);

        var updateDto = new UpdateBlogCategoryDto
        {
            Name = "Updated Category Name",
            Description = "Updated description",
            IsActive = false
        };

        // Act
        var result = await _blogCategoryAppService.UpdateAsync(createdCategory.Id, updateDto);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(createdCategory.Id);
        result.Name.ShouldBe("Updated Category Name");
        result.Description.ShouldBe("Updated description");
        result.IsActive.ShouldBe(false);
    }

    [Fact]
    public async Task Should_Delete_Blog_Category()
    {
        // Arrange
        var createDto = new CreateBlogCategoryDto
        {
            Name = "Test Category for Delete",
            Description = "Test category description for delete",
            IsActive = true
        };
        var createdCategory = await _blogCategoryAppService.CreateAsync(createDto);

        // Act
        await _blogCategoryAppService.DeleteAsync(createdCategory.Id);

        // Assert
        await Should.ThrowAsync<EntityNotFoundException>(async () =>
        {
            await _blogCategoryAppService.GetAsync(createdCategory.Id);
        });
    }

    [Fact]
    public async Task Should_Create_Child_Category()
    {
        // Arrange
        var parentDto = new CreateBlogCategoryDto
        {
            Name = "Parent Category",
            Description = "Parent category description",
            IsActive = true
        };
        var parent = await _blogCategoryAppService.CreateAsync(parentDto);

        var childDto = new CreateBlogCategoryDto
        {
            Name = "Child Category",
            Description = "Child category description",
            ParentId = parent.Id,
            IsActive = true
        };

        // Act
        var result = await _blogCategoryAppService.CreateAsync(childDto);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Child Category");
        result.ParentId.ShouldBe(parent.Id);
    }

    [Fact]
    public async Task Should_Move_Category()
    {
        // Arrange
        var category1Dto = new CreateBlogCategoryDto
        {
            Name = "Category 1",
            Description = "Category 1 description",
            IsActive = true
        };
        var category1 = await _blogCategoryAppService.CreateAsync(category1Dto);

        var category2Dto = new CreateBlogCategoryDto
        {
            Name = "Category 2",
            Description = "Category 2 description",
            IsActive = true
        };
        var category2 = await _blogCategoryAppService.CreateAsync(category2Dto);

        var childDto = new CreateBlogCategoryDto
        {
            Name = "Child Category",
            Description = "Child category description",
            ParentId = category1.Id,
            IsActive = true
        };
        var child = await _blogCategoryAppService.CreateAsync(childDto);

        // Act
        var result = await _blogCategoryAppService.MoveAsync(child.Id, category2.Id);

        // Assert
        result.ShouldNotBeNull();
        result.ParentId.ShouldBe(category2.Id);
    }

    [Fact]
    public async Task Should_Activate_Category()
    {
        // Arrange
        var createDto = new CreateBlogCategoryDto
        {
            Name = "Inactive Category",
            Description = "Inactive category description",
            IsActive = false
        };
        var category = await _blogCategoryAppService.CreateAsync(createDto);

        // Act
        var result = await _blogCategoryAppService.ActivateAsync(category.Id);

        // Assert
        result.ShouldNotBeNull();
        result.IsActive.ShouldBe(true);
    }

    [Fact]
    public async Task Should_Deactivate_Category()
    {
        // Arrange
        var createDto = new CreateBlogCategoryDto
        {
            Name = "Active Category",
            Description = "Active category description",
            IsActive = true
        };
        var category = await _blogCategoryAppService.CreateAsync(createDto);

        // Act
        var result = await _blogCategoryAppService.DeactivateAsync(category.Id);

        // Assert
        result.ShouldNotBeNull();
        result.IsActive.ShouldBe(false);
    }

    [Fact]
    public async Task Should_Generate_Unique_Slug()
    {
        // Arrange
        var name = "Test Category Name";

        // Act
        var slug = await _blogCategoryAppService.GenerateSlugAsync(name);

        // Assert
        slug.ShouldNotBeNullOrEmpty();
        slug.ShouldBe("test-category-name");
    }

    [Fact]
    public async Task Should_Check_Slug_Availability()
    {
        // Arrange
        var createDto = new CreateBlogCategoryDto
        {
            Name = "Test Category for Slug Check",
            Description = "Description",
            IsActive = true
        };
        var createdCategory = await _blogCategoryAppService.CreateAsync(createDto);

        // Act
        var isAvailable = await _blogCategoryAppService.IsSlugAvailableAsync(createdCategory.Slug);
        var isAvailableExcludingSelf = await _blogCategoryAppService.IsSlugAvailableAsync(createdCategory.Slug, createdCategory.Id);

        // Assert
        isAvailable.ShouldBe(false); // Already taken
        isAvailableExcludingSelf.ShouldBe(true); // Available when excluding self
    }

    [Fact]
    public async Task Should_Get_Active_Categories()
    {
        // Arrange
        var activeDto = new CreateBlogCategoryDto
        {
            Name = "Active Category",
            Description = "Active category description",
            IsActive = true
        };
        await _blogCategoryAppService.CreateAsync(activeDto);

        var inactiveDto = new CreateBlogCategoryDto
        {
            Name = "Inactive Category",
            Description = "Inactive category description",
            IsActive = false
        };
        await _blogCategoryAppService.CreateAsync(inactiveDto);

        // Act
        var result = await _blogCategoryAppService.GetActiveCategoriesAsync();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.ShouldAllBe(c => c.IsActive);
    }

    [Fact]
    public async Task Should_Get_Statistics()
    {
        // Arrange
        await _blogCategoryAppService.CreateAsync(new CreateBlogCategoryDto { Name = "Cat1", IsActive = true });
        await _blogCategoryAppService.CreateAsync(new CreateBlogCategoryDto { Name = "Cat2", IsActive = false });

        // Act
        var result = await _blogCategoryAppService.GetStatisticsAsync();

        // Assert
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(2);
        result.ActiveCount.ShouldBeGreaterThanOrEqualTo(1);
        result.InactiveCount.ShouldBeGreaterThanOrEqualTo(1);
    }
}
