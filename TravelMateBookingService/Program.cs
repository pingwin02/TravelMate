using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using TravelMate.Models.Messages;
using TravelMateBackend.Data;

using TravelMateBookingService.Models.Settings;
using TravelMateBookingService.Repositories;
using TravelMateBookingService.Services;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultDbConnection"),
        ServerVersion.Parse("11.7.2-mariadb")));
var rabbitMqSettings = builder.Configuration.GetSection("RabbitMq");
builder.Services.AddMassTransit(busConfig =>
{

    busConfig.AddRequestClient<CheckSeatAvailabilityRequest>(new Uri("queue:check-seat-availability-queue"));
    busConfig.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMqSettings["Host"], h =>
        {
            h.Username(rabbitMqSettings["Username"]);
            h.Password(rabbitMqSettings["Password"]);
        });

    });
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Travel Mate Booking Service api", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "token z auth service",
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
            new string[] {}
        }
    });
});
builder.Services.Configure<BookingsSettings>(
    builder.Configuration.GetSection("BookingsSettings"));

builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddHostedService<BookingExpirationService>();

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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TravelMateBooking API v1");
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();