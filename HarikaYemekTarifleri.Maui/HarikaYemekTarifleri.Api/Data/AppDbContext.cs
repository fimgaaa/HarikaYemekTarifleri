using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using HarikaYemekTarifleri.Api.Models;
namespace HarikaYemekTarifleri.Api.Data;

public class AppDbContext : DbContext
{
    private readonly IHttpContextAccessor _http;

    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor http) : base(options)
        => _http = http;

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<RecipeCategory> RecipeCategories => Set<RecipeCategory>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<RecipeCategory>().HasKey(rc => new { rc.RecipeId, rc.CategoryId });
        // Fluent API ile ilişkiler burada…
        base.OnModelCreating(mb);
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var userName = _http.HttpContext?.User?.FindFirstValue(ClaimTypes.Name) ?? "system";
        var now = DateTime.UtcNow;

        foreach (var e in ChangeTracker.Entries<BaseEntity>())
        {
            if (e.State == EntityState.Added)
            {
                e.Entity.CreatedAt = now;
                e.Entity.CreatedBy = userName;
            }
            else if (e.State == EntityState.Modified)
            {
                e.Entity.UpdatedAt = now;
                e.Entity.UpdatedBy = userName;
            }
        }
        return base.SaveChangesAsync(ct);
    }
}
