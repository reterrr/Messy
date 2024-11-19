using Messy.Contexts;
using Messy.Helpers;
using Messy.Helpers.Interceptors;
using Messy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<UpdateInterceptor>();
builder.Services.AddSingleton<SoftDeleteInterceptor>();

FileSettings.initialize(builder.Configuration);


var connectionString = builder.Configuration.GetConnectionString("DockerCommandsConnectionString") ??
                       throw new InvalidOperationException("Missing docker connection string");

builder.Services.AddDbContext<MessyDbContext>(
    (sp, options) =>
        options
            .UseNpgsql(connectionString)
            .AddInterceptors(
                sp.GetRequiredService<SoftDeleteInterceptor>(),
                sp.GetRequiredService<UpdateInterceptor>())
);


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllerRoute(name: "default", pattern: "{controller}/{action}");

app.UseHttpsRedirection();
app.Run();