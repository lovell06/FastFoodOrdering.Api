using FastFoodOrdering.Api.Authorization;
using FastFoodOrdering.Api.Data;
using FastFoodOrdering.Api.Data.Seeders;
using FastFoodOrdering.Api.Models;
using FastFoodOrdering.Api.Services.Implementations;
using FastFoodOrdering.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var jwtSection = builder.Configuration.GetSection("Jwt");

builder.Services.AddControllers();

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});

// Đăng ký ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        .UseSeeding((context, _) =>
        {
            DbSeeder.Seed((ApplicationDbContext)context);
        })
        .UseAsyncSeeding(async (context, _, cancellationToken) =>
        {
            await DbSeeder.SeedAsync((ApplicationDbContext)context, cancellationToken);
        }));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Set to true to enforce validation
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        // Set the expected values from your configuration
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!))
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Authentication is required to access this resource."
            });
        },
        OnForbidden = async context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Admin permission is required to access this resource."
            });
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthorizationPolicies.AdminOnly, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new AdminAccessRequirement());
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IAuthorizationHandler, AdminAccessHandler>();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(policy =>
{
    policy.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
});

app.MapScalarApiReference();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.Run();
