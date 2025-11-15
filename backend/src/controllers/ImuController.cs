using ImuBackend.Domain;
using Microsoft.AspNetCore.Mvc;

namespace ImuBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ImuController : ControllerBase
{
    private readonly IImuSampleProvider _sampleProvider;
    private readonly ILogger<ImuController> _logger;

    public ImuController(IImuSampleProvider sampleProvider, ILogger<ImuController> logger)
    {
        _sampleProvider = sampleProvider ?? throw new ArgumentNullException(nameof(sampleProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Returns a single IMU sample read from the configured storage.
    /// </summary>
    [HttpGet("sample")]
    public async Task<ActionResult<ImuSample>> GetSampleAsync(CancellationToken cancellationToken)
    {
        var sample = await _sampleProvider.GetCurrentSampleAsync(cancellationToken);

        if (sample is null)
        {
            _logger.LogWarning("IMU sample was requested, but no sample file was found or could be read.");
            return NotFound();
        }

        return Ok(sample);
    }
}
