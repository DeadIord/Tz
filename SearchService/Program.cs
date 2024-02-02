using MassTransit;
using Microsoft.OpenApi.Models;
using SearchService;
using SearchService.Core.Commands;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();builder.Services.AddSwaggerGen(x =>
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
        x.IncludeXmlComments(item);     }
});

builder.Services.AddTransient<SearchServices>();

builder.Services.AddMassTransit(x =>
{
    x.AddRequestClient<ProductSearchRequest>();
    x.AddRequestClient<UserSearchResponse>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ");

        cfg.Message<UserSearchRequest>(x => x.SetEntityName("UserSearchConsumerQueue"));
        cfg.Message<ProductSearchRequest>(x => x.SetEntityName("ProductSearchConsumerQueue"));

        int portValue = rabbitMqConfig.GetValue<int>("Port");

        // Преобразование в ushort
        ushort port = Convert.ToUInt16(portValue);

        cfg.Host(rabbitMqConfig.GetValue<string>("Hostname"), port, "/", h =>
        {
            h.Username(rabbitMqConfig.GetValue<string>("Username"));
            h.Password(rabbitMqConfig.GetValue<string>("Password"));
        });
    });
});



var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", "SearchService"));

app.UseHttpsRedirection(); 
app.MapControllers();

app.Run();