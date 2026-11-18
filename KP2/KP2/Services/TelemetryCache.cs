using KP2.Data;
using KP2.Models;

namespace KP2.Services;

public class TelemetryCache
{
    private readonly SqliteRepository<TelemetryLog> _logs;
    private readonly Dictionary<int, string> _cache = new();

    public TelemetryCache(SqliteRepository<TelemetryLog> logs) => _logs = logs;

    public ValueTask<string> GetLatestAsync(int deviceId)
    {
        if (_cache.TryGetValue(deviceId, out var cached))
            return new ValueTask<string>(cached);

        return new ValueTask<string>(LoadAsync(deviceId));
    }

    private async Task<string> LoadAsync(int deviceId)
    {
        await Task.Delay(300 + deviceId % 5 * 150); // задержка опроса «по сети»

        var logs = await _logs.GetAllAsync();
        var last = logs
            .Where(l => l.DeviceId == deviceId)
            .OrderByDescending(l => l.Timestamp)
            .FirstOrDefault();

        string text = last == null ? "нет данных" : $"{last.Reading:0.##} ({last.Timestamp:dd.MM HH:mm})";
        _cache[deviceId] = text;
        return text;
    }

    public void Clear() => _cache.Clear();
}
