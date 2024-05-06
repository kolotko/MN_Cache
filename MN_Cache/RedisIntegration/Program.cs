using Redis.OM;
using Redis.OM.Contracts;
using RedisIntegration.BackgroundTasks;
using RedisIntegration.Models;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IConnectionMultiplexer>(x =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));

builder.Services.AddTransient<IRedisConnectionProvider>(x =>
    new RedisConnectionProvider(builder.Configuration.GetConnectionString("RedisOm")));

builder.Services.AddHostedService<RedisSubscriber>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/messages", async (string message, IConnectionMultiplexer connectionMultiplexer, CancellationToken cancellationToken) =>
    {
        var subscriber = connectionMultiplexer.GetSubscriber();
        await subscriber.PublishAsync("messages", message);
        return Results.Ok();
    })
    .WithOpenApi();

app.MapGet("/ping", async (IRedisConnectionProvider redisConnectionProvider ) =>
    {
        var response = redisConnectionProvider.Connection.Execute("PING").ToString();
        return Results.Ok(response);
    })
    .WithOpenApi();

app.MapPost("/putAndGetObjectFromRedis", async (IRedisConnectionProvider redisConnectionProvider ) =>
    {
        var connection = redisConnectionProvider.Connection;
        connection.Execute("FLUSHDB");

        var person = new Person()
        {
            Id = 1,
            Name = "TestName",
            LastName = "TestLastName",
            Age = 32
        };

        var personId = connection.Set(person);
        var dataFromRedis = await connection.GetAsync<Person>(personId);
        return Results.Ok(dataFromRedis);
    })
    .WithOpenApi();

app.Run();