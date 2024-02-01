using MassTransit;
using Microsoft.OpenApi.Models;
using SearchService;
using SearchService.Core.Commands;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<SearchServices>();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.AddRequestClient<ProductSearchRequest>();
    x.AddRequestClient<UserSearchResponse>();
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ");
        cfg.Message<UserSearchRequest>(x => x.SetEntityName("UserSearchConsumerQueue"));
        cfg.Message<ProductSearchRequest>(x => x.SetEntityName("ProductSearchConsumerQueue"));
        cfg.Host(new Uri(rabbitMqConfig["Hostname"]!), h =>
        {
            h.Username(rabbitMqConfig["Username"]);
            h.Password(rabbitMqConfig["Password"]);
        });
    });
});



Log.Logger = new LoggerConfiguration()
.WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
.CreateLogger();
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddSerilog();
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
