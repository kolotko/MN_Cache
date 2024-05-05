using RedisIntegration.BackgroundTasks;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IConnectionMultiplexer>(x =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));
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

app.Run();