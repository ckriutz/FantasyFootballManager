using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using OpenTelemetry;
using System.Diagnostics;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var AllowEveryhting = "_allowEverything";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowEveryhting,
    policy  =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var sqlConnectionString = Environment.GetEnvironmentVariable("sqlConnectionString");
var redisConnectionString = Environment.GetEnvironmentVariable("redisConnectionString");

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.AddDbContext<FantasyDbContext>(options => options.UseSqlServer(sqlConnectionString),ServiceLifetime.Transient );
builder.Services.AddSingleton<Instrumentation>();
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName: builder.Environment.ApplicationName))
    .WithTracing(tracing => tracing.AddAspNetCoreInstrumentation().AddConsoleExporter());


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(AllowEveryhting);
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
