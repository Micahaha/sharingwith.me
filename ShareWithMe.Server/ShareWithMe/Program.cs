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
    options.AddPolicy("AllowShareWithMe", builder =>
    {
        builder.WithOrigins("http://localhost:3000")
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

// Add controllers
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.AddScoped<IFileStorageService, FileStorageService>();



var app = builder.Build();

// cors policy
app.UseCors("AllowShareWithMe");


// Map controllers

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


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
