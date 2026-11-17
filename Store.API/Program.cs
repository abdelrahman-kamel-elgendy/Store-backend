using DotNetEnv;
using Store.API.Extensions;

Env.NoClobber().TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDatabase(builder.Configuration, builder.Environment)
    .AddApiControllers()
    .AddSwaggerDocs()
    .AddMapping()
    .AddRepositories()
    .AddApplicationServices();

var app = builder.Build();

await app.SeedDatabaseIfEnabledAsync();

app.UseSwaggerDocs();
app.UseApiErrorHandling();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();