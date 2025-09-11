//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using HarikaYemekTarifleri.Api.DTOs;
using Microsoft.OpenApi.Models;

using HarikaYemekTarifleri.Api.Data;    // AppDbContext
using HarikaYemekTarifleri.Api.Models;  // AppUser, Recipe, Category, Comment, RecipeCategory

var builder = WebApplication.CreateBuilder(args);

// DbContext + HttpContextAccessor (audit için gerekli)
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

// JWT ayarları
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = key
        };
    });

builder.Services.AddAuthorization();

// Swagger
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HarikaYemekTarifleri API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Bearer şeması. Örn: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

/* ============================
   AUTH ENDPOINTLERİ (/auth)
   ============================ */
var auth = app.MapGroup("/auth");

// Register
auth.MapPost("/register", async (AppDbContext db, AppUser dto) =>
{
    if (string.IsNullOrWhiteSpace(dto.UserName) || string.IsNullOrWhiteSpace(dto.PasswordHash))
        return Results.BadRequest("Kullanıcı adı/parola boş olamaz");

    if (await db.Users.AnyAsync(u => u.UserName == dto.UserName))
        return Results.BadRequest("Kullanıcı zaten var");

    var user = new AppUser
    {
        UserName = dto.UserName,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.PasswordHash)
    };
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Ok();
});

// Login – İstemci { userName, password } gönderir; burada PasswordHash alanını password olarak kullanıyoruz
auth.MapPost("/login", async (AppDbContext db, AppUser dto) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.UserName == dto.UserName);
    if (user is null || !BCrypt.Net.BCrypt.Verify(dto.PasswordHash, user.PasswordHash))
        return Results.Unauthorized();


    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.UserName)
    };

    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(
        issuer: jwtSection["Issuer"],
        audience: jwtSection["Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddHours(12),
        signingCredentials: creds);

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
    return Results.Ok(new { token = tokenString });
});

// Şifre değiştir
auth.MapPost("/change-password", async (AppDbContext db, ClaimsPrincipal user, string oldPassword, string newPassword) =>
{
    var name = user.Identity?.Name;
    var entity = await db.Users.FirstOrDefaultAsync(u => u.UserName == name);
    if (entity is null) return Results.Unauthorized();

    if (!BCrypt.Net.BCrypt.Verify(oldPassword, entity.PasswordHash))
        return Results.BadRequest("Eski şifre yanlış.");

    entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
    await db.SaveChangesAsync();
    return Results.Ok();
}).RequireAuthorization();

/* =======================================
   TARİF / KATEGORİ / YORUM ENDPOINTLERİ
   ======================================= */

// Tarifler
var recipes = app.MapGroup("/api/recipes").RequireAuthorization();

recipes.MapGet("/", async (AppDbContext db, string? q, int? categoryId, bool? vegetarian,
                           Difficulty? difficulty, DateTime? fromDate, TimeSpan? maxPrep) =>
{
    var query = db.Recipes
        .Include(r => r.RecipeCategories).ThenInclude(rc => rc.Category)
        .Include(r => r.Comments)
        .AsQueryable();

    if (!string.IsNullOrWhiteSpace(q))
        query = query.Where(r => r.Title.Contains(q) || r.Content.Contains(q));

    if (categoryId.HasValue)
        query = query.Where(r => r.RecipeCategories.Any(rc => rc.CategoryId == categoryId.Value));

    if (vegetarian.HasValue)
        query = query.Where(r => r.IsVegetarian == vegetarian.Value);

    if (difficulty.HasValue)
        query = query.Where(r => r.Difficulty == difficulty.Value);

    if (fromDate.HasValue)
        query = query.Where(r => r.PublishDate >= fromDate.Value);

    if (maxPrep.HasValue)
        query = query.Where(r => r.PrepTime <= maxPrep.Value);

    var list = await query
        .OrderByDescending(r => r.PublishDate)
        .Select(r => new {
            r.Id,
            r.Title,
            r.IsVegetarian,
            r.Difficulty,
            r.PrepTime,
            r.PublishDate,
            Categories = r.RecipeCategories.Select(rc => rc.Category.Name),
            CommentsCount = r.Comments.Count
        })
        .ToListAsync();

    return Results.Ok(list);
});

recipes.MapGet("/{id:int}", async (AppDbContext db, int id) =>
{
    var r = await db.Recipes
        .Include(x => x.RecipeCategories).ThenInclude(rc => rc.Category)
        .Include(x => x.Comments)
        .FirstOrDefaultAsync(x => x.Id == id);
    return r is null ? Results.NotFound() : Results.Ok(r);
});

// POST /api/recipes
//recipes.MapPost("/", async (AppDbContext db, ClaimsPrincipal user, RecipeCreateDto dto) =>
recipes.MapPost("/", async ([FromBody] RecipeCreateDto dto, AppDbContext db, ClaimsPrincipal user) =>
{
    if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Content))
        return Results.BadRequest("Başlık ve içerik boş olamaz.");

    var uid = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

    var entity = new Recipe
    {
        Title = dto.Title,
        Content = dto.Content,
        IsVegetarian = dto.IsVegetarian,
        Difficulty = dto.Difficulty,
        PrepTime = dto.PrepTime,
        PublishDate = dto.PublishDate,
        UserId = uid
    };

    foreach (var cid in dto.CategoryIds.Distinct())
        entity.RecipeCategories.Add(new RecipeCategory { CategoryId = cid });

    db.Recipes.Add(entity);
    await db.SaveChangesAsync();
    return Results.Created($"/api/recipes/{entity.Id}", entity);
});

// PUT /api/recipes/{id}
recipes.MapPut("/{id:int}", async (AppDbContext db, int id,  RecipeUpdateDto dto) =>
{
    var r = await db.Recipes.Include(x => x.RecipeCategories)
                            .FirstOrDefaultAsync(x => x.Id == id);
    if (r is null) return Results.NotFound();

    r.Title = dto.Title;
    r.Content = dto.Content;
    r.IsVegetarian = dto.IsVegetarian;
    r.Difficulty = dto.Difficulty;
    r.PrepTime = dto.PrepTime;
    r.PublishDate = dto.PublishDate;

    r.RecipeCategories.Clear();
    foreach (var cid in dto.CategoryIds.Distinct())
        r.RecipeCategories.Add(new RecipeCategory { RecipeId = r.Id, CategoryId = cid });

    await db.SaveChangesAsync();
    return Results.NoContent();
});


//recipes.MapPost("/", async (AppDbContext db, ClaimsPrincipal user, Recipe dto, int[] categoryIds) =>
//{
//    if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Content))
//        return Results.BadRequest("Başlık ve içerik boş olamaz.");

//    var uid = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
//    dto.UserId = uid;

//    foreach (var cid in categoryIds.Distinct())
//        dto.RecipeCategories.Add(new RecipeCategory { CategoryId = cid });

//    db.Recipes.Add(dto);
//    await db.SaveChangesAsync();
//    return Results.Created($"/api/recipes/{dto.Id}", dto);
//});

//recipes.MapPut("/{id:int}", async (AppDbContext db, int id, Recipe dto, int[] categoryIds) =>
//{
//    var r = await db.Recipes.Include(x => x.RecipeCategories).FirstOrDefaultAsync(x => x.Id == id);
//    if (r is null) return Results.NotFound();

//    r.Title = dto.Title;
//    r.Content = dto.Content;
//    r.IsVegetarian = dto.IsVegetarian;
//    r.Difficulty = dto.Difficulty;
//    r.PrepTime = dto.PrepTime;
//    r.PublishDate = dto.PublishDate;

//    r.RecipeCategories.Clear();
//    foreach (var cid in categoryIds.Distinct())
//        r.RecipeCategories.Add(new RecipeCategory { RecipeId = r.Id, CategoryId = cid });

//    await db.SaveChangesAsync();
//    return Results.NoContent();
//});

recipes.MapDelete("/{id:int}", async (AppDbContext db, int id) =>
{
    var r = await db.Recipes.FindAsync(id);
    if (r is null) return Results.NotFound();
    db.Remove(r);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Kategoriler
var cats = app.MapGroup("/api/categories").RequireAuthorization();
cats.MapGet("/", async (AppDbContext db) => await db.Categories.OrderBy(x => x.Name).ToListAsync());
cats.MapPost("/", async (AppDbContext db, Category c) =>
{
    db.Categories.Add(c);
    await db.SaveChangesAsync();
    return Results.Ok(c);
});

// Yorumlar
var comments = app.MapGroup("/api/comments").RequireAuthorization();
comments.MapPost("/", async (AppDbContext db, ClaimsPrincipal u, int recipeId, string text) =>
{
    var userName = u.Identity?.Name ?? "anon";
    var c = new Comment { RecipeId = recipeId, Text = text, CreatedBy = userName, CreatedAt = DateTime.UtcNow };
    db.Comments.Add(c);
    await db.SaveChangesAsync();
    return Results.Ok(c);
});

app.Run();
