using Store.Infrastructure;
using DotNetEnv;

Env.NoClobber().TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

if (string.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("StoreDb")))
{
    throw new InvalidOperationException(
        "Required configuration 'ConnectionStrings:StoreDb' is missing. Set the ConnectionStrings__StoreDb environment variable (for example in the repository-root .env file).");
}

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.InfrastructureConfig(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
