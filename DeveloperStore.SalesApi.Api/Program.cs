using DeveloperStore.SalesApi.Api.Middleware;
using DeveloperStore.SalesApi.Application;
using DeveloperStore.SalesApi.Infrastructure;
using DeveloperStore.SalesApi.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseApiExceptionHandler();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await databaseInitializer.InitializeAsync();
}

app.Run();

public partial class Program;
