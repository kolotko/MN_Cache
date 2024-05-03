
using Microsoft.AspNetCore.OutputCaching;
using OutputCache.Abstractions;
using OutputCache.Dto;
using OutputCache.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("NoCache", builder => builder
        .With(c => !(c.HttpContext.Request.Headers["nocache"] == "true"))
        .Cache());
});
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

app.UseOutputCache();
app.UseHttpsRedirection();

app.MapPost("/customers", async (CreateCustomerRequestDto createCustomerRequestDto, ICustomerRepository customerRepository, 
        IOutputCacheStore outputCacheStore, CancellationToken cancellationToken) =>
    {
        var customer = customerRepository.Create(createCustomerRequestDto);
        if (customer is not null)
        {
            await outputCacheStore.EvictByTagAsync("customers",cancellationToken);
            return Results.Created($"customers/{customer.Id}", customer);
        }

        return Results.BadRequest();
    })
    .WithOpenApi();

app.MapGet("/customers/{id:int}",  (int id, ICustomerRepository customerRepository) =>
    {
        var customer = customerRepository.GetById(id);
        if (customer is not null)
        {
            return Results.Ok(customer);
        }

        return Results.NotFound();
    })
    .WithOpenApi();

app.MapGet("/customers", (string? name, ICustomerRepository customerRepository) =>
    {
        var customers = customerRepository.GetCustomers(name);
        if (customers.Count > 0)
        {
            return Results.Ok(customers);
        }

        return Results.NotFound();
    })
    .CacheOutput(x => x.Tag("customers"))
    .WithOpenApi();

int count = 0;
app.MapGet("/lock", async (context) =>
    {
        await Task.Delay(2000);
        await context.Response.WriteAsync(count++.ToString());
    })
    .CacheOutput(p => p.SetLocking(false));

app.Run();