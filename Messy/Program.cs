using System.Text;
using Messy.Helpers;
using Messy.Requests;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new RoutePrefixConvention("api"));
    options.ReturnHttpNotAcceptable = true;
}).AddJsonOptions(option =>
{
    option.JsonSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.AddHttpContextAccessor();

FileSettings.initialize(builder.Configuration);
ConfigAccesser.Init(builder.Configuration);

var connectionString = builder.Configuration.GetConnectionString("DockerCommandsConnectionString") ??
                       throw new InvalidOperationException("Missing Docker connection string");

NpgslqConnector.setConnectionString(connectionString);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

Request.Init(builder.Services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>());

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.AddPolicy("AllowAnonymous", policy => policy.RequireAssertion(_ => true));
});

// Add HTTP context accessor
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseWebSockets();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();