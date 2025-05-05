using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TravelMate.Models.Messages;
using TravelMateBookingService.Consumers;
using TravelMateBookingService.Data;
using TravelMateBookingService.Models.Settings;
using TravelMateBookingService.Repositories;
using TravelMateBookingService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultDbConnection"),
        ServerVersion.Parse("11.7.2-mariadb")));

builder.Services.Configure<BookingsSettings>(
    builder.Configuration.GetSection("BookingsSettings"));

builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<BookingStatusUpdateConsumer>();
builder.Services.AddSingleton<BookingExpirationService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<BookingExpirationService>());

var rabbitMqSettings = builder.Configuration.GetSection("RabbitMq");
builder.Services.AddMassTransit(busConfig =>
{
    busConfig.AddRequestClient<CheckSeatAvailabilityRequest>(new Uri("queue:check-seat-availability-queue"));
    busConfig.AddRequestClient<PaymentCreationRequest>(new Uri("queue:payment-queue"));
    busConfig.AddConsumer<BookingStatusUpdateConsumer>();
    busConfig.AddConsumer<CancelBookingConsumer>();
    busConfig.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMqSettings["Host"], h =>
        {
            h.Username(rabbitMqSettings["Username"]);
            h.Password(rabbitMqSettings["Password"]);
        });

        cfg.ReceiveEndpoint("update-booking-status-queue",
            e => { e.ConfigureConsumer<BookingStatusUpdateConsumer>(context); });

        cfg.ReceiveEndpoint("cancel-booking-queue",
            e => { e.ConfigureConsumer<CancelBookingConsumer>(context); });
    });
});
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "TravelMateAuthService token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.MapOpenApi();

app.UseHttpsRedirection();
app.UseRouting();
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TravelMateBookingService API v1");
    c.DocumentTitle = "TravelMate Bookings API";
});

app.UseAuthentication();
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