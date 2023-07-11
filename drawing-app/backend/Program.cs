using backend.Hubs;
using Microsoft.AspNetCore.SignalR;

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

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors();

app.UseHttpsRedirection();

app.MapHub<DrawingHub>("/drawinghub");

app.MapGet("/erase", async (IHubContext<DrawingHub, IDrawingHubClient> hubContext) =>
{
    await hubContext.Clients.All.EraseAllScreen();
    return true;
});

app.Run();