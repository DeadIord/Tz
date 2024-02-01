using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using ProductService.Data;
using ProductService.Models;
using ProductService.Rabbit;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AddProductDbContext>(options =>
           options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ProductServices>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddSwaggerGen();


builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<OrderRequestHandler>();
    config.AddConsumer<SearchRequestHandler>();
    config.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ");

        cfg.ReceiveEndpoint("ProductSearchConsumerQueue", x =>
        {
            x.ConfigureConsumer<SearchRequestHandler>(context);
            x.ConfigureConsumeTopology = false;
            x.Bind("SearchConsumerExchange");
        });
        cfg.ReceiveEndpoint("ListProductsOrderConsumer", x =>
        {
            x.ConfigureConsumer<OrderRequestHandler>(context);
            x.ConfigureConsumeTopology = false;
            x.Bind("ListProductsOrderConsumerExchange");
        });
        cfg.Host(new Uri(rabbitMqConfig["Hostname"]!), h =>
        {
            h.Username(rabbitMqConfig["Username"]);
            h.Password(rabbitMqConfig["Password"]);
        });
    });

});

Log.Logger = new LoggerConfiguration()

      .WriteTo.File("logses.txt", rollingInterval: RollingInterval.Day)
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

app.UseAuthorization();

app.MapControllers();

app.Run();
