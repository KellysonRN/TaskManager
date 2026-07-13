using System.Text;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TaskManager.Api.Middleware;
using TaskManager.Api.Models;
using TaskManager.Application;
using TaskManager.Infrastructure;
using TaskManager.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddLogging();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
// Also configure the Application-layer JwtSettings used by JwtTokenService
builder.Services.Configure<TaskManager.Application.Configuration.JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings?.Issuer,
        ValidAudience = jwtSettings?.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Secret ?? string.Empty))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<TaskManager.Infrastructure.Persistence.TaskManagerDbContext>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TaskManager API",
        Version = "v1",
        Description = "Clean Architecture Task Manager API"
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and token."
    };

    options.AddSecurityDefinition("Bearer", securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagerDbContext>();

    try
    {
        dbContext.Database.EnsureCreated();

        // Probe the Users table to detect schema drift (e.g. DB created before
        // UserEntity was added). If it is missing, drop and recreate the whole schema.
        _ = dbContext.Users.Any();
    }
    catch (Microsoft.Data.Sqlite.SqliteException)
    {
        // Schema is outdated — recreate so all tables exist.
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
    }

    // Seed initial data. The seeder is idempotent (skips if data already exists).
    // In integration tests the 'Testing' environment skips this to allow explicit control.
    if (!app.Environment.IsEnvironment("Testing"))
    {
        var seeder = scope.ServiceProvider.GetRequiredService<TaskManager.Infrastructure.Persistence.DataSeeder>();
        await seeder.SeedAsync();
    }
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program { }
