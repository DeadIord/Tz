using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrderService.Core.Commands;
using Serilog;
using System.Reflection;
using System.Text;
using UserService.Data;
using UserService.Models;
using UserService.Rabbit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


builder.Services.AddDbContext<AddDbContext>(options =>
          options.UseSqlServer(
          builder.Configuration.GetConnectionString("UserConnection"),
          sqlOptions => sqlOptions.CommandTimeout(60)));
builder.Services.AddScoped<UserServices>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:SecretKey"]!)),
                   ValidateIssuer = false,
                   ValidateAudience = false
               };
           });

builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<SearchRequestHandler>();
    config.AddRequestClient<RequestForOrderHistory>();

    config.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ");
        cfg.Message<RequestForOrderHistory>(x => x.SetEntityName("ListProductsOrderConsumer"));


        cfg.ReceiveEndpoint("UserSearchConsumerQueue", x =>
        {
            x.ConfigureConsumer<SearchRequestHandler>(context);
            x.ConfigureConsumeTopology = false;
            x.Bind("SearchConsumerExchange");
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
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
