using System.Text.Json;
using ImuBackend.Domain;

namespace ImuBackend.Infrastructure;

/// <summary>
/// Reads a single IMU sample from a JSON file on disk.
/// This is a minimal implementation intended as a starting point.
/// </summary>
public sealed class JsonFileImuSampleProvider : IImuSampleProvider
{
    private readonly string _filePath;

    public JsonFileImuSampleProvider(IConfiguration configuration, IWebHostEnvironment environment)
    {
        // Read relative path from configuration, defaulting to "imu_sample.json".
        var relativePath = configuration.GetValue<string>("ImuSample:JsonFilePath") ?? "imu_sample.json";

        // Resolve to physical path in the content root.
        _filePath = Path.Combine(environment.ContentRootPath, relativePath);
    }

    public async Task<ImuSample?> GetCurrentSampleAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
        {
            return null;
        }

        await using var stream = File.OpenRead(_filePath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var sample = await JsonSerializer.DeserializeAsync<ImuSample>(stream, options, cancellationToken);

        return sample;
    }
}
