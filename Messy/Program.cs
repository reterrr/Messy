using System.Text;
using System.Text.Json.Serialization;
using Messy.Actions;
using Messy.Actions.Auth;
using Messy.Contexts;
using Messy.Helpers;
using Messy.Helpers.Interceptors;
using Messy.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DbContext = Messy.Actions.DbContext;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddControllers();
builder.Services.AddSingleton<UpdateInterceptor>();
builder.Services.AddSingleton<SoftDeleteInterceptor>();

FileSettings.initialize(builder.Configuration);
ConfigAccesser.Init(builder.Configuration);

var connectionString = builder.Configuration.GetConnectionString("DockerCommandsConnectionString") ??
                       throw new InvalidOperationException("Missing Docker connection string");

MessyDbContextFactory.SetConnectionString(connectionString);
builder.Services.AddDbContext<MessyDbContext>(
    options =>
        options
            .UseNpgsql(connectionString)
);

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(ConfigAccesser.Configuration.GetValue<string>("JwtSettings:SecretKey"))),
        };
    });

// Add authorization
builder.Services.AddAuthorization(options =>
{
    // Set a default policy requiring authentication
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.AddPolicy("AllowAnonymous", policy => policy.RequireAssertion(_ => true));
});

// Add HTTP context accessor
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});


app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.Run();