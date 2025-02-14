using ChatALot.Data.Context;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using Serilog.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("LocalConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.MSSqlServer(
    connectionString: connectionString,
    sinkOptions: new Serilog.Sinks.MSSqlServer.MSSqlServerSinkOptions
        {
            TableName = "Logs",
            AutoCreateSqlTable = true,
        }
    )
    .Enrich.WithProperty("Application", "User Activity API")
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
