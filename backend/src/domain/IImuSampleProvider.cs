namespace ImuBackend.Domain;

/// <summary>
/// Abstraction for a component that can provide the current IMU sample.
/// </summary>
public interface IImuSampleProvider
{
    Task<ImuSample?> GetCurrentSampleAsync(CancellationToken cancellationToken = default);
}
