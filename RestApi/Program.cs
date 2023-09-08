// Import necessary namespaces
using Microsoft.EntityFrameworkCore;
using RestApi.Data;
using RestApi.Services.TemperatureService;
using Prometheus;

// Suppress some default metrics to make the output cleaner, so the exemplars are easier to see.
// Metrics.SuppressDefaultMetrics(new SuppressDefaultMetricOptions
// {
//     SuppressEventCounters = true,
//     SuppressMeters = true,
//     SuppressProcessMetrics = true
// });

// Initialize a new WebApplication builder
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add the DbContext (DataContext) to the dependency injection container
// Use Npgsql as the database provider and read connection string from configuration
builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the controllers present in the assembly for handling HTTP requests
builder.Services.AddControllers();

// Add support for API exploration and Swagger (for API documentation)
// You can learn more about Swagger/OpenAPI configuration at the given URL
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add AutoMapper to automatically handle object-object mapping
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Register custom services for dependency injection
// Here, implementations for ICharacterService and ITemperatureService are provided
builder.Services.AddScoped<ITemperatureService, TemperatureService>();


// Start the metrics exporter as a background service.
// Open http://localhost:1234/metrics to see the metrics.
//
// Metrics published:
// * built-in process metrics giving basic information about the .NET runtime (enabled by default)
// * metrics from .NET Event Counters (enabled by default, updated every 10 seconds)
// * metrics from .NET Meters (enabled by default)
// * metrics about requests handled by the web app (configured below)
builder.Services.AddMetricServer(options =>
{
    options.Port = 1234;
});

// Build the application
var app = builder.Build();


// Configure the HTTP request pipeline

// If the app is in development mode, enable Swagger for API documentation
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Capture metrics about all received HTTP requests.
app.UseHttpMetrics();

// Setup routing middleware, very important
app.UseRouting();

// Enable the authorization middleware
app.UseAuthorization();

// Map controllers for handling routes
app.MapControllers();

// Run the application
app.Run();
