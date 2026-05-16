using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

using backend.Services;
using backend.Data;
using Microsoft.EntityFrameworkCore;

// Load .env file variables
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host=localhost;Port=5432;Database=rbacdb;Username=postgres;Password=P@rth990"));

// Read from Configuration or directly from Environment
var key = builder.Configuration["JWT_SECRET"] ?? Environment.GetEnvironmentVariable("JWT_SECRET");

if (string.IsNullOrEmpty(key))
{
    throw new Exception("JWT_SECRET is not set in the .env file!");
}

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//jwt-service-configuration 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });



builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});   

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("create_lead", policy => policy.RequireClaim("permission", "create_lead"));
    options.AddPolicy("assign_lead", policy => policy.RequireClaim("permission", "assign_lead"));
    options.AddPolicy("view_lead", policy => policy.RequireClaim("permission", "view_lead"));
    options.AddPolicy("update_lead_status", policy => policy.RequireClaim("permission", "update_lead_status"));

    options.AddPolicy("create_property", policy => policy.RequireClaim("permission", "create_property"));
    options.AddPolicy("assign_property", policy => policy.RequireClaim("permission", "assign_property"));
    options.AddPolicy("view_property", policy => policy.RequireClaim("permission", "view_property"));
    options.AddPolicy("update_property_status", policy => policy.RequireClaim("permission", "update_property_status"));
});

// Dependency Injection
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<LeadService>();
builder.Services.AddScoped<PropertyService>();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

app.MapGet("/api/secure", () => "This is protected data")
   .RequireAuthorization();


// Admin only
app.MapGet("/api/admin", () => "Admin data")
   .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });

// Agent + Admin
app.MapGet("/api/agent", () => "Agent data")
   .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin,Agent" });

// Viewer + all
app.MapGet("/api/view", () => "Viewer data")
   .RequireAuthorization();


app.Run();