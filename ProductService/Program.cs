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


builder.Services.AddDbContext<AddProductDbContext>(options =>
           options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ProductServices>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new OpenApiInfo { Description = "TEST" });

    var entryAssembly = Assembly.GetEntryAssembly();
    var xmlDocs = entryAssembly!.GetReferencedAssemblies()
    .Union(new AssemblyName[] { entryAssembly.GetName() })
    .Select(a => Path.Combine(Path.GetDirectoryName(entryAssembly.Location)!, $"{a.Name}.xml"))
    .Where(f => File.Exists(f))
    .ToArray();
    foreach (var item in xmlDocs)
    {
        x.IncludeXmlComments(item);
    }
});


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
         cfg.Host(rabbitMqConfig.GetValue<string>("Hostname"), rabbitMqConfig.GetValue<ushort>("Port"), "/", h =>
    {
        h.Username(rabbitMqConfig.GetValue<string>("Username"));
        h.Password(rabbitMqConfig.GetValue<string>("Password"));
    });
    });

});


var app = builder.Build();


    app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
