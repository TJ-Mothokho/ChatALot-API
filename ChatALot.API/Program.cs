using ChatALot.Data.Context;
using ChatALot.Data.Repository.Implementation;
using ChatALot.Data.Repository.Interface;
using ChatALot.Data.Services.Implementation;
using ChatALot.Data.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using Serilog.AspNetCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ChatALot.API.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

//Services
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IMessageServices, MessageServices>();

//Connection String
var connectionString = builder.Configuration.GetConnectionString("LocalConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

//Logging
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

//Authentication and Authorization
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

//Signal R
builder.Services.AddSignalR();


//Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Allow frontend
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // If using authentication
    });
});
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("MyPolicy", policy =>
//    policy.AllowAnyOrigin()
//    .AllowAnyMethod()
//    .AllowAnyHeader());
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("MyPolicy");

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("/chatHub");
});

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
