using MassTransit;
using Microsoft.EntityFrameworkCore;
using TravelMate.Models.Messages;
using TravelMatePaymentService.Consumers;
using TravelMatePaymentService.Data;
using TravelMatePaymentService.Models.Settings;
using TravelMatePaymentService.Repositories;
using TravelMatePaymentService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultDbConnection"),
        ServerVersion.Parse("11.7.2-mariadb")));

builder.Services.Configure<PaymentsSettings>(
    builder.Configuration.GetSection("PaymentsSettings"));

builder.Services.AddScoped<IPaymentsRepository, PaymentsRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<CreatePaymentConsumer>();

var rabbitMqSettings = builder.Configuration.GetSection("RabbitMq");
builder.Services.AddMassTransit(busConfig =>
{
    busConfig.AddRequestClient<BookingStatusUpdateRequest>(new Uri("queue:update-booking-status-queue"));
    busConfig.AddConsumer<CreatePaymentConsumer>();
    busConfig.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMqSettings["Host"], h =>
        {
            h.Username(rabbitMqSettings["Username"]);
            h.Password(rabbitMqSettings["Password"]);
        });

        cfg.ReceiveEndpoint("payment-queue",
            e => { e.ConfigureConsumer<CreatePaymentConsumer>(context); });
    });
});
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.MapOpenApi();

app.UseHttpsRedirection();
app.UseRouting();
app.UseSwagger();

app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "TravelMatePaymentService API v1"); });

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