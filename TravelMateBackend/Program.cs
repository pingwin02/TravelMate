using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;
using TravelMateBackend.Data;
using TravelMateBackend.Repositories.Offers;
using TravelMateBackend.Services.Offers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultDbConnection"),
        ServerVersion.Parse("11.7.2-mariadb")));

builder.Services.AddScoped<IOfferRepository, OfferRepository>();
builder.Services.AddScoped<IOfferService, OfferService>();

builder.Services.AddSwaggerGen();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();  
}

app.UseHttpsRedirection(); 
app.UseRouting();           
app.UseSwagger();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TravelMate API v1");
    });
}

app.UseAuthorization();  
app.MapControllers();   

app.Run();

