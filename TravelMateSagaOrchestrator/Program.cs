using MassTransit;
using TravelMateSagaOrchestrator.Models.SagaStates;
using TravelMateSagaOrchestrator.Saga;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

var rabbitMqSettings = builder.Configuration.GetSection("RabbitMq");

builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<BookingSaga, BookingSagaState>()
        .InMemoryRepository();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMqSettings["Host"], h =>
        {
            h.Username(rabbitMqSettings["Username"]);
            h.Password(rabbitMqSettings["Password"]);
        });

        cfg.ConfigureEndpoints(context);
    });
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();
app.Run();