using DotNetEnv;
using Store.API.Extensions;

Env.NoClobber().TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDatabase(builder.Configuration, builder.Environment)
    .AddApiControllers()
    .AddMapping()
    .AddRepositories()
    .AddApplicationServices();

var app = builder.Build();

if (await app.RunSeedingIfRequestedAsync(args))
    return;

app.UseApiErrorHandling();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();