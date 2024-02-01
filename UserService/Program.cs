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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AddDbContext>(options =>
          options.UseSqlServer(
          builder.Configuration.GetConnectionString("UserConnection"),
          sqlOptions => sqlOptions.CommandTimeout(60)));
builder.Services.AddScoped<UserServices>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserService", Version = "v1" });


    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
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
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
