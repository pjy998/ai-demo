using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddBlogEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppBlogCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ParentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    Icon = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Color = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    MetaKeywords = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    MetaDescription = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    ExtraProperties = table.Column<string>(type: "TEXT", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatorId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppBlogCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppBlogCategories_AppBlogCategories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "AppBlogCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppBlogTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Color = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    UsageCount = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    ExtraProperties = table.Column<string>(type: "TEXT", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatorId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppBlogTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppBlogPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Summary = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    PublishedTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CategoryId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ViewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LikeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CoverImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    MetaKeywords = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    MetaDescription = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    AllowComments = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSticky = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    ExtraProperties = table.Column<string>(type: "TEXT", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatorId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppBlogPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppBlogPosts_AppBlogCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "AppBlogCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AppBlogComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BlogPostId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParentCommentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Content = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    AuthorName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AuthorEmail = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    AuthorWebsite = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    LikeCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Depth = table.Column<int>(type: "INTEGER", nullable: false),
                    IsFromRegisteredUser = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ExtraProperties = table.Column<string>(type: "TEXT", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatorId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppBlogComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppBlogComments_AppBlogComments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "AppBlogComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppBlogComments_AppBlogPosts_BlogPostId",
                        column: x => x.BlogPostId,
                        principalTable: "AppBlogPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppBlogPostTags",
                columns: table => new
                {
                    BlogPostId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BlogTagId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatorId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppBlogPostTags", x => new { x.BlogPostId, x.BlogTagId });
                    table.ForeignKey(
                        name: "FK_AppBlogPostTags_AppBlogPosts_BlogPostId",
                        column: x => x.BlogPostId,
                        principalTable: "AppBlogPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppBlogPostTags_AppBlogTags_BlogTagId",
                        column: x => x.BlogTagId,
                        principalTable: "AppBlogTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppBlogCategories_ParentId",
                table: "AppBlogCategories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppBlogCategories_Slug",
                table: "AppBlogCategories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppBlogComments_BlogPostId",
                table: "AppBlogComments",
                column: "BlogPostId");

            migrationBuilder.CreateIndex(
                name: "IX_AppBlogComments_CreationTime",
                table: "AppBlogComments",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_AppBlogComments_ParentCommentId",
                table: "AppBlogComments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppBlogComments_Status",
                table: "AppBlogComments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AppBlogPosts_CategoryId",
                table: "AppBlogPosts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AppBlogPosts_PublishedTime",
                table: "AppBlogPosts",
                column: "PublishedTime");

            migrationBuilder.CreateIndex(
                name: "IX_AppBlogPosts_Slug",
                table: "AppBlogPosts",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppBlogPosts_Status",
                table: "AppBlogPosts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AppBlogPostTags_BlogTagId",
                table: "AppBlogPostTags",
                column: "BlogTagId");

            migrationBuilder.CreateIndex(
                name: "IX_AppBlogTags_Name",
                table: "AppBlogTags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppBlogTags_Slug",
                table: "AppBlogTags",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppBlogComments");

            migrationBuilder.DropTable(
                name: "AppBlogPostTags");

            migrationBuilder.DropTable(
                name: "AppBlogPosts");

            migrationBuilder.DropTable(
                name: "AppBlogTags");

            migrationBuilder.DropTable(
                name: "AppBlogCategories");
        }
    }
}
