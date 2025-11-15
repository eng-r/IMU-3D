namespace ImuBackend.Domain;

/// <summary>
/// Represents a single orientation sample from an IMU.
/// Angles are expressed in radians.
/// </summary>
public sealed class ImuSample
{
    public DateTime Timestamp { get; init; }

    public double Yaw { get; init; }

    public double Pitch { get; init; }

    public double Roll { get; init; }
}
