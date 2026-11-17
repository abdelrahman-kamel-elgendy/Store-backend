using DotNetEnv;
using Store.API.Extensions;

// Load .env file
Env.NoClobber().TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

var host = builder.Configuration["APP_HOST"] ?? "localhost";
var port = builder.Configuration["APP_PORT"] ?? "5189";
builder.WebHost.UseUrls($"http://{host}:{port}");

// register services 
builder.Services
    .AddDatabase(builder.Configuration)
    .AddApiControllers()
    .AddMapping()
    .AddRepositories()
    .AddApplicationServices();

var app = builder.Build();

// seeding 
if (await app.RunSeedingIfRequestedAsync(args))
    return;

// configure Pipeline
app.UseApiErrorHandling();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();