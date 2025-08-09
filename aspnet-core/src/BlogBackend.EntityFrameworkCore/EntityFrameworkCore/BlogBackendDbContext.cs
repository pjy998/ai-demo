using BlogBackend.Entities;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace BlogBackend.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class BlogBackendDbContext :
    AbpDbContext<BlogBackendDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */
    
    #region Blog Entities
    
    public DbSet<BlogPost> BlogPosts { get; set; }
    public DbSet<BlogCategory> BlogCategories { get; set; }
    public DbSet<BlogTag> BlogTags { get; set; }
    public DbSet<BlogPostTag> BlogPostTags { get; set; }
    public DbSet<BlogComment> BlogComments { get; set; }
    
    #endregion

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }
    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public BlogBackendDbContext(DbContextOptions<BlogBackendDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        /* Configure your own tables/entities inside here */

        // 配置博客实体
        builder.Entity<BlogPost>(b =>
        {
            b.ToTable(BlogBackendConsts.DbTablePrefix + "BlogPosts", BlogBackendConsts.DbSchema);
            b.ConfigureByConvention();
            
            // 配置属性
            b.Property(x => x.Title).IsRequired().HasMaxLength(200);
            b.Property(x => x.Summary).HasMaxLength(500);
            b.Property(x => x.Content).IsRequired();
            b.Property(x => x.Slug).IsRequired().HasMaxLength(200);
            b.Property(x => x.CoverImageUrl).HasMaxLength(500);
            b.Property(x => x.MetaKeywords).HasMaxLength(200);
            b.Property(x => x.MetaDescription).HasMaxLength(300);
            
            // 配置索引
            b.HasIndex(x => x.Slug).IsUnique();
            b.HasIndex(x => x.CategoryId);
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.PublishedTime);
            
            // 配置关系
            b.HasOne(x => x.Category)
                .WithMany(x => x.Posts)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<BlogCategory>(b =>
        {
            b.ToTable(BlogBackendConsts.DbTablePrefix + "BlogCategories", BlogBackendConsts.DbSchema);
            b.ConfigureByConvention();
            
            // 配置属性
            b.Property(x => x.Name).IsRequired().HasMaxLength(100);
            b.Property(x => x.Description).HasMaxLength(500);
            b.Property(x => x.Slug).IsRequired().HasMaxLength(100);
            b.Property(x => x.Icon).HasMaxLength(100);
            b.Property(x => x.Color).HasMaxLength(20);
            b.Property(x => x.MetaKeywords).HasMaxLength(200);
            b.Property(x => x.MetaDescription).HasMaxLength(300);
            
            // 配置索引
            b.HasIndex(x => x.Slug).IsUnique();
            b.HasIndex(x => x.ParentId);
            
            // 配置自关联
            b.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<BlogTag>(b =>
        {
            b.ToTable(BlogBackendConsts.DbTablePrefix + "BlogTags", BlogBackendConsts.DbSchema);
            b.ConfigureByConvention();
            
            // 配置属性
            b.Property(x => x.Name).IsRequired().HasMaxLength(50);
            b.Property(x => x.Slug).IsRequired().HasMaxLength(50);
            b.Property(x => x.Description).HasMaxLength(200);
            b.Property(x => x.Color).HasMaxLength(20);
            
            // 配置索引
            b.HasIndex(x => x.Slug).IsUnique();
            b.HasIndex(x => x.Name).IsUnique();
        });

        builder.Entity<BlogPostTag>(b =>
        {
            b.ToTable(BlogBackendConsts.DbTablePrefix + "BlogPostTags", BlogBackendConsts.DbSchema);
            b.ConfigureByConvention();
            
            // 配置组合主键
            b.HasKey(x => new { x.BlogPostId, x.BlogTagId });
            
            // 配置关系
            b.HasOne(x => x.BlogPost)
                .WithMany(x => x.Tags)
                .HasForeignKey(x => x.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);
                
            b.HasOne(x => x.BlogTag)
                .WithMany(x => x.Posts)
                .HasForeignKey(x => x.BlogTagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<BlogComment>(b =>
        {
            b.ToTable(BlogBackendConsts.DbTablePrefix + "BlogComments", BlogBackendConsts.DbSchema);
            b.ConfigureByConvention();
            
            // 配置属性
            b.Property(x => x.Content).IsRequired().HasMaxLength(2000);
            b.Property(x => x.AuthorName).IsRequired().HasMaxLength(100);
            b.Property(x => x.AuthorEmail).IsRequired().HasMaxLength(200);
            b.Property(x => x.AuthorWebsite).HasMaxLength(200);
            b.Property(x => x.IpAddress).HasMaxLength(50);
            b.Property(x => x.UserAgent).HasMaxLength(500);
            
            // 配置索引
            b.HasIndex(x => x.BlogPostId);
            b.HasIndex(x => x.ParentCommentId);
            b.HasIndex(x => x.Status);
            b.HasIndex(x => x.CreationTime);
            
            // 配置关系
            b.HasOne(x => x.BlogPost)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);
                
            b.HasOne(x => x.ParentComment)
                .WithMany(x => x.Replies)
                .HasForeignKey(x => x.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
