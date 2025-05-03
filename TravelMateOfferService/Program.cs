using Microsoft.EntityFrameworkCore;
using TravelMateOfferService.Data;
using TravelMateOfferService.Repositories;
using TravelMateOfferService.Services;

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

app.MapOpenApi();

app.UseHttpsRedirection();
app.UseRouting();
app.UseSwagger();

app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "TravelMateOfferService API v1"); });

app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    try
    {
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine("An error occurred while migrating the database: " + ex.Message);
    }
}

app.Run();