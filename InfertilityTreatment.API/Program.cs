using FluentValidation;
using InfertilityTreatment.API.Extensions;
using InfertilityTreatment.API.Middleware;
using InfertilityTreatment.API.Services;
using InfertilityTreatment.API.Hubs;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Business.Services;
using InfertilityTreatment.Business.Validators;
using InfertilityTreatment.Data.Context;
using InfertilityTreatment.Data.Repositories.Implementations;
using InfertilityTreatment.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Test.Services;
using Test.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCycleDtoValidator>();

builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Configure Database
builder.Services.AddDbContext<InfertilityTreatment.Data.Context.ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Test namespace Database Context - use in-memory database for testing
builder.Services.AddDbContext<Test.Data.ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("TestUsersDB"));

// Add Business Services
builder.Services.AddBusinessServices();

// Add Test namespace services
builder.Services.AddScoped<Test.Services.IUserService, Test.Services.UserService>();

// Add Payment Gateway Configuration
builder.Services.AddPaymentGateways(builder.Configuration);

// Add JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Add SignalR for real-time notifications
builder.Services.AddSignalR();

// Register SignalR notification service
builder.Services.AddScoped<SignalRNotificationService>();

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database")
    .AddCheck<ExternalServiceHealthCheck>("external-services")
    .AddCheck<SystemResourceHealthCheck>("system-resources");

// Configure Swagger with JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Infertility Treatment API",
        Version = "v1",
        Description = "API for managing infertility treatment cycles and patient care"
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.\\n\\nExample: \"Bearer 12345abcdef\""
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure CORS for React app and SignalR
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials() // Required for SignalR
              .SetIsOriginAllowed(origin => true); // Allow all origins for development
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline

// Custom middleware (order matters!)
app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Development middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Infertility Treatment API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

// CORS must be before Authentication
app.UseCors("AllowReactApp");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Map SignalR Hub
app.MapHub<NotificationHub>("/notificationHub");

// Configure event handling for SignalR notifications
using (var scope = app.Services.CreateScope())
{
    var eventService = scope.ServiceProvider.GetRequiredService<INotificationEventService>();
    var signalRService = scope.ServiceProvider.GetRequiredService<SignalRNotificationService>();
    
    // Register SignalR service as event handler
    if (eventService is NotificationEventService notificationEventService)
    {
        notificationEventService.RegisterHandler(signalRService.HandleNotificationEventAsync);
    }
}

// Initialize query optimization warmup
using (var scope = app.Services.CreateScope())
{
    var queryOptimizationService = scope.ServiceProvider.GetRequiredService<IQueryOptimizationService>();
    _ = Task.Run(async () =>
    {
        await Task.Delay(5000); // Wait 5 seconds after startup
        await queryOptimizationService.WarmupCriticalQueries();
    });
}

// Initialize Test database
using (var scope = app.Services.CreateScope())
{
    var testDbContext = scope.ServiceProvider.GetRequiredService<Test.Data.ApplicationDbContext>();
    testDbContext.Database.EnsureCreated();
}

// Health check endpoint
app.MapGet("/health", () => new
{
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Environment = app.Environment.EnvironmentName,
    Version = "1.0.0"
})
.WithName("HealthCheck")
.WithTags("Health");

app.Run();
