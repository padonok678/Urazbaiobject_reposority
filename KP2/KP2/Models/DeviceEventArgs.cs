namespace KP2.Models;

public class DeviceEventArgs : EventArgs
{
    public string Message { get; }
    public DateTime Timestamp { get; }

    public DeviceEventArgs(string message, DateTime timestamp)
    {
        Message = message;
        Timestamp = timestamp;
    }
}
