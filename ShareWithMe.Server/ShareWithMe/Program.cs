using System.Threading.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ShareWithMe.Data;
using ShareWithMe.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowShareWithMe", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:3000" })
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

// Add controllers
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.AddScoped<IFileStorageService, FileStorageService>();


// Rate Limiting on API endpoints

// Only allows 10 requests per minute per user.

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 10,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
});


var app = builder.Build();

// Order: security headers → HTTPS → CORS (before rate limit so preflight gets CORS) → rate limit → Swagger (dev) → endpoints

app.UseHsts();
app.UseHttpsRedirection();

app.UseCors("AllowShareWithMe");

app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();


// test DB connection once at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        db.Database.OpenConnection();              // force a real open
        Console.WriteLine("Database connection successful via OpenConnection.");
        db.Database.CloseConnection();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Database connection FAILED with exception:");
        Console.WriteLine(ex.Message);
        Console.WriteLine(ex.ToString());          // full stack + inner exceptions
    }
}


var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");
