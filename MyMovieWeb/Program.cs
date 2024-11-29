using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyMovieWeb.Application.Helper;
using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Application.Services;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;
using MyMovieWeb.Infrastructure.Data;
using MyMovieWeb.Infrastructure.Repositories;
using MyMovieWeb.Presentation.Auth0;
using Stripe;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins("http://localhost:5000") // Allowed origins
                  .AllowAnyMethod() // Allow any HTTP method
                  .AllowAnyHeader() // Allow any header
                  .AllowCredentials(); // Allow credentials (cookies, etc.)
        });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
    options.Audience = builder.Configuration["Auth0:Audience"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.NameIdentifier
    };
});
builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();



StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Value;


builder.Services.AddHttpClient<FileUploadHelper>();
builder.Services.AddSingleton<FileUploadHelper>();
builder.Services.AddAutoMapper(typeof(ApplicationMapper));
builder.Services.AddMemoryCache();
builder.Services.AddSignalR();

builder.Services.AddScoped<IRepository<Genre>, Repository<Genre>>();
builder.Services.AddScoped<IRepository<Movie>, Repository<Movie>>();
builder.Services.AddScoped<IRepository<MovieGenre>, Repository<MovieGenre>>();
builder.Services.AddScoped<IRepository<Episode>, Repository<Episode>>();
builder.Services.AddScoped<IRepository<Comment>, Repository<Comment>>();
builder.Services.AddScoped<IRepository<FollowedMovie>, Repository<FollowedMovie>>();
builder.Services.AddScoped<IRepository<WatchHistory>, Repository<WatchHistory>>();
builder.Services.AddScoped<IRepository<BlogPost>, Repository<BlogPost>>();
builder.Services.AddScoped<IRepository<BlogTag>, Repository<BlogTag>>();
builder.Services.AddScoped<IRepository<BlogPostTag>, Repository<BlogPostTag>>();
builder.Services.AddScoped<IRepository<Notification>, Repository<Notification>>();

builder.Services.AddScoped<IAuth0Services, Auth0Services>();
builder.Services.AddScoped<IGenreServices, GenreServices>();
builder.Services.AddScoped<IMovieService, MovieServices>();
builder.Services.AddScoped<IEpisodeServices, EpisodeServices>();
builder.Services.AddScoped<IWatchHistoryServices, WatchHistoryServices>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IBlogTagService, BlogTagService>();
builder.Services.AddScoped<IBlogPostService, BlogPostService>();
builder.Services.AddScoped<INotificationServices, NotificationServices>();
builder.Services.AddSingleton<IMessageServices, MessageServices>();
builder.Services.AddSingleton<IS3Services, S3Services>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
     .AddAuthorization(options =>
     {
         options.AddPolicy(
           "create:genre",
           policy => policy.Requirements.Add(
             new HasScopeRequirement("create:genre", $"https://{builder.Configuration["Auth0:Domain"]}/")
           )
         );
         options.AddPolicy(
           "update:genre",
           policy => policy.Requirements.Add(
             new HasScopeRequirement("update:genre", $"https://{builder.Configuration["Auth0:Domain"]}/")
           )
         );
         options.AddPolicy(
           "delete:genre",
           policy => policy.Requirements.Add(
             new HasScopeRequirement("delete:genre", $"https://{builder.Configuration["Auth0:Domain"]}/")
           )
         );
         options.AddPolicy(
           "create:movie",
           policy => policy.Requirements.Add(
             new HasScopeRequirement("create:movie", $"https://{builder.Configuration["Auth0:Domain"]}/")
           )
         );
         options.AddPolicy(
           "update:movie",
           policy => policy.Requirements.Add(
             new HasScopeRequirement("update:movie", $"https://{builder.Configuration["Auth0:Domain"]}/")
           )
         );
         options.AddPolicy(
           "delete:movie",
           policy => policy.Requirements.Add(
             new HasScopeRequirement("delete:movie", $"https://{builder.Configuration["Auth0:Domain"]}/")
           )
         );
         options.AddPolicy(
           "create:episode",
           policy => policy.Requirements.Add(
             new HasScopeRequirement("create:episode", $"https://{builder.Configuration["Auth0:Domain"]}/")
           )
         );
         options.AddPolicy(
           "update:episode",
           policy => policy.Requirements.Add(
             new HasScopeRequirement("update:episode", $"https://{builder.Configuration["Auth0:Domain"]}/")
           )
         );
         options.AddPolicy(
           "delete:episode",
           policy => policy.Requirements.Add(
             new HasScopeRequirement("delete:episode", $"https://{builder.Configuration["Auth0:Domain"]}/")
           )
         );
     });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<MessageHub>("/messageHub");

app.MapControllers();

app.Run();
