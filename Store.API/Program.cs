using DotNetEnv;
using Store.API.Extensions;
using Store.Core.Options;

// Load .env file
Env.NoClobber().TraversePath().Load();

Directory.CreateDirectory(Path.Combine(
    Directory.GetCurrentDirectory(),
    Environment.GetEnvironmentVariable("IMAGES__ROOTPATH") ?? "wwwroot"));

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
    .AddApplicationServices()
    .AddImageStorage(builder.Configuration)
    .AddCorsConfiguration(builder.Configuration);

var app = builder.Build();

// seeding 
if (await app.RunSeedingIfRequestedAsync(args))
    return;

// configure Pipeline
app.UseSecurityHeaders();
app.UseApiErrorHandling();
app.UseCors("CORSPolicy");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();