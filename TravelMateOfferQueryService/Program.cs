using MassTransit;
using Microsoft.EntityFrameworkCore;
using TravelMateOfferQueryService.Consumers;
using TravelMateOfferQueryService.Hubs;
using TravelMateOfferQueryService.Repositories;
using TravelMateOfferQueryService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddSingleton<DataContext>();
builder.Services.AddSignalR();
builder.Services.AddScoped<IOfferQueryRepository, OfferQueryRepository>();
builder.Services.AddScoped<IOfferQueryService, OfferQueryService>();

var rabbitMqSettings = builder.Configuration.GetSection("RabbitMq");
builder.Services.AddMassTransit(busConfig =>
{
    busConfig.AddConsumer<AddOfferEventConsumer>();
    busConfig.AddConsumer<UpdateOfferEventConsumer>();
    busConfig.AddConsumer<DeleteOfferEventConsumer>();
    busConfig.SetKebabCaseEndpointNameFormatter();
    busConfig.UsingRabbitMq((context, cfg) =>
    {
        
        cfg.Host(rabbitMqSettings["Host"], h =>
        {
            h.Username(rabbitMqSettings["Username"]);
            h.Password(rabbitMqSettings["Password"]);
        });
        cfg.ReceiveEndpoint("add-offer-queue", e =>
        {
            e.ConfigureConsumer<AddOfferEventConsumer>(context);
        });
        cfg.ReceiveEndpoint("update-offer-queue", e =>
        {
            e.ConfigureConsumer<UpdateOfferEventConsumer>(context);
        });
        cfg.ReceiveEndpoint("delete-offer-queue", e =>
        {
            e.ConfigureConsumer<DeleteOfferEventConsumer>(context);
        });
    });
});

builder.Services.AddSwaggerGen();
var app = builder.Build();

app.MapOpenApi();

app.UseHttpsRedirection();
app.UseRouting();
app.UseSwagger();
app.MapHub<OfferHub>("/api/offerHub");
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TravelMateOfferCommandService API v1");
    c.DocumentTitle = "TravelMate Offers Query API";
});

app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    // Initialize MongoDB
    var mongoContext = scope.ServiceProvider.GetRequiredService<DataContext>();
    try
    {
        await MongoDbInitializer.InitializeMongoDbAsync(mongoContext);
    }
    catch (Exception ex)
    {
        Console.WriteLine("An error occurred while initializing MongoDB: " + ex.Message);
    }
}

app.Run();