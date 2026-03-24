using EpsiCodeAPI.Data;
using EpsiCodeAPI.Hubs;
using EpsiCodeAPI.Interfaces;
using EpsiCodeAPI.Jobs;
using EpsiCodeAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register In-Memory Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("EpsiCodeDb"));

builder.Services.AddScoped<IBookRepository, BookRepository>();

// Register the Book Service
builder.Services.AddHttpClient<BookService>();

// Register Order Service
builder.Services.AddScoped<IOrderService, OrderService>();

//Register Job
builder.Services.AddHostedService<BookSyncJob>();

//Register SingalR
builder.Services.AddSignalR();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMyApp", policy =>
    {
        policy.WithOrigins("https://localhost:7234")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowMyApp");

app.MapControllers();

app.MapHub<SyncHub>("/syncHub");

app.Run();
