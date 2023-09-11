using Microsoft.EntityFrameworkCore;
using RestApi.Services.TemperatureService;
using Prometheus;
using RestApi.Services.MoistureService;
using RestApi.Data;

// Setup local environments
string path = @"..\.env";
if (File.Exists(path))
{
    string[] lines = File.ReadAllLines(path);
    HashSet<string> keys = new();

    foreach (var line in lines)
    {
        // Skip comments and empty lines
        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
        {
            continue;
        }

        var parts = line.Split(
            '=',
            StringSplitOptions.RemoveEmptyEntries);

        // Check if line format is correct
        if (parts.Length != 2)
        {
            Console.WriteLine($"Skipping invalid line: {line}");
            continue;
        }

        string key = parts[0].Trim();
        string value = parts[1].Trim();

        // Check for empty keys
        if (string.IsNullOrEmpty(key))
        {
            Console.WriteLine($"Skipping invalid key in line: {line}");
            continue;
        }

        // Check for duplicate keys
        if (keys.Contains(key))
        {
            Console.WriteLine($"Duplicate key {key} found. Skipping...");
            continue;
        }
        keys.Add(key);

        // Set the environment variable
        Environment.SetEnvironmentVariable(key, value);
    }
}
else
{
    Console.WriteLine("No environment file found!");
    return;
}

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
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

string host = Environment.GetEnvironmentVariable("POSTGRES_HOST")!;
string username = Environment.GetEnvironmentVariable("POSTGRES_USER")!;
string password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")!;

connectionString = connectionString
                        .Replace("{HOST}", host)
                        .Replace("{USERNAME}", username)
                        .Replace("{PASSWORD}", password);

builder.Services.AddDbContext<DataContext>(options => options.UseNpgsql(connectionString));
// builder.Services.AddDbContext<DataContext>(options =>
//     options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the controllers present in the assembly for handling HTTP requests
builder.Services.AddControllers();

// Add support for API exploration and Swagger (for API documentation)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add AutoMapper to automatically handle object-object mapping
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Register custom services for dependency injection
builder.Services.AddScoped<ITemperatureService, TemperatureService>();
builder.Services.AddScoped<IMoistureService, MoistureService>();

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
