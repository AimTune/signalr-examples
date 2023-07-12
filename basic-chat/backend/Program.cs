using backend.Hubs;
using backend.Managers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("http://127.0.0.1:3000")
                .AllowAnyHeader()
                .WithMethods("GET", "POST")
                .AllowCredentials();
        });
});

builder.Services.AddSignalR();
builder.Services.AddSingleton<UserListManager>();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors();

app.UseHttpsRedirection();
app.MapHub<MyHub>("/chat");

app.MapGet("/chatty", () =>
{
    return true;
});

app.Run();

