using ImuBackend.Domain;
using ImuBackend.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// Register the IMU sample provider
builder.Services.AddSingleton<IImuSampleProvider, JsonFileImuSampleProvider>();

// Add minimal OpenAPI for convenience
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
