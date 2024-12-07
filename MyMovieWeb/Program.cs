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
            policy.WithOrigins("http://localhost:5000", "https://my-movie-web-client-eight.vercel.app") 
                  .AllowAnyMethod() 
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("RDSConnection"), 
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    )
);

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

//Stripe config
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Value;

builder.Services.AddHttpClient<Auth0Services>();
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
builder.Services.AddScoped<IRepository<Post>, Repository<Post>>();
builder.Services.AddScoped<IRepository<Tag>, Repository<Tag>>();
builder.Services.AddScoped<IRepository<PostTags>, Repository<PostTags>>();
builder.Services.AddScoped<IRepository<Notification>, Repository<Notification>>();
builder.Services.AddScoped<IRepository<Order>, Repository<Order>>();

builder.Services.AddScoped<IAuth0Services, Auth0Services>();
builder.Services.AddScoped<IGenreServices, GenreServices>();
builder.Services.AddScoped<IMovieService, MovieServices>();
builder.Services.AddScoped<IEpisodeServices, EpisodeServices>();
builder.Services.AddScoped<IWatchHistoryServices, WatchHistoryServices>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ITagServices, TagService>();
builder.Services.AddScoped<IPostServices, PostService>();
builder.Services.AddScoped<INotificationServices, NotificationServices>();
builder.Services.AddSingleton<IS3Services, S3Services>();
builder.Services.AddScoped<IOrderServices, OrderServices>();
builder.Services.AddScoped<IStatisticServices, StatisticServices>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Perrmission
builder.Services
     .AddAuthorization(options =>
     {
         options.AddPolicy(
           "create:data",
           policy => policy.Requirements.Add(
             new HasScopeRequirement("create:data", $"https://{builder.Configuration["Auth0:Domain"]}/")
           )
         );
         options.AddPolicy(
         "read:data",
            policy => policy.Requirements.Add(
                new HasScopeRequirement("read:data", $"https://{builder.Configuration["Auth0:Domain"]}/")
            )
         );
         options.AddPolicy(
           "update:data",
           policy => policy.Requirements.Add(
             new HasScopeRequirement("update:data", $"https://{builder.Configuration["Auth0:Domain"]}/")
           )
         );
         options.AddPolicy(
           "delete:data",
           policy => policy.Requirements.Add(
             new HasScopeRequirement("delete:data", $"https://{builder.Configuration["Auth0:Domain"]}/")
           )
         );
     });

var app = builder.Build();
app.MapGet("/", async (context) =>
{
    await context.Response.WriteAsync("Hello, World!");
});

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

app.MapHub<VisitHub>("/visit");

app.MapControllers();

app.Run();
