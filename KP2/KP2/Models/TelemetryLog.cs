namespace KP2.Models;

public class TelemetryLog
{
    public int Id { get; }
    public int DeviceId { get; }
    public double Reading { get; }
    public DateTime Timestamp { get; }

    public TelemetryLog(int id, int deviceId, double reading, DateTime timestamp)
    {
        Id = id;
        DeviceId = deviceId;
        Reading = reading;
        Timestamp = timestamp;
    }
}
