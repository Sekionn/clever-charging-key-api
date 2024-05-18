using clever_charging_key_api.Models;
using clever_charging_key_api.Providers;
using clever_charging_key_api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<ChargingKeyContext>(opt => opt.UseInMemoryDatabase("ChargingKeyList"));

builder.Services.AddScoped<ChargingKeyContextProvider>();

builder.Services.AddScoped<ChargingKeyService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
