// Import necessary namespaces
using Microsoft.EntityFrameworkCore;
using RestApi.Data;
using RestApi.Services.CharacterService;
using RestApi.Services.TemperatureService;

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
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<ITemperatureService, TemperatureService>();

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

// Enable the authorization middleware
app.UseAuthorization();

// Map controllers for handling routes
app.MapControllers();

// Run the application
app.Run();
