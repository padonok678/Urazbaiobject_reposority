using System.Globalization;
using Microsoft.Data.Sqlite;
using KP2.Models;

namespace KP2.Data;

public static class Repositories
{
    public static SqliteRepository<Room> Rooms() => new(
        "SELECT Id, Title, Floor FROM Rooms",
        "SELECT Id, Title, Floor FROM Rooms WHERE Id = $id",
        "INSERT INTO Rooms (Title, Floor) VALUES ($t, $f)",
        r => new Room(r.GetInt32(0), r.GetString(1), r.GetInt32(2)),
        (cmd, room) =>
        {
            cmd.Parameters.AddWithValue("$t", room.Title);
            cmd.Parameters.AddWithValue("$f", room.Floor);
        });

    public static SqliteRepository<Device> Devices() => new(
        "SELECT Id, RoomId, Model, Power, Type FROM Devices",
        "SELECT Id, RoomId, Model, Power, Type FROM Devices WHERE Id = $id",
        "INSERT INTO Devices (RoomId, Model, Power, Type) VALUES ($r, $m, $p, $t)",
        MapDevice,
        (cmd, device) =>
        {
            cmd.Parameters.AddWithValue("$r", device.RoomId);
            cmd.Parameters.AddWithValue("$m", device.Model);
            cmd.Parameters.AddWithValue("$p", device.Power);
            cmd.Parameters.AddWithValue("$t", device.DeviceType);
        });

    public static SqliteRepository<TelemetryLog> Logs() => new(
        "SELECT Id, DeviceId, Reading, Timestamp FROM TelemetryLogs",
        "SELECT Id, DeviceId, Reading, Timestamp FROM TelemetryLogs WHERE Id = $id",
        "INSERT INTO TelemetryLogs (DeviceId, Reading, Timestamp) VALUES ($d, $v, $t)",
        r => new TelemetryLog(r.GetInt32(0), r.GetInt32(1), r.GetDouble(2),
                              DateTime.ParseExact(r.GetString(3), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)),
        (cmd, log) =>
        {
            cmd.Parameters.AddWithValue("$d", log.DeviceId);
            cmd.Parameters.AddWithValue("$v", log.Reading);
            cmd.Parameters.AddWithValue("$t", log.Timestamp.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
        });

    // точность датчика и запас по нагрузке в БД не хранятся, считаем из Id и мощности
    private static Device MapDevice(SqliteDataReader r)
    {
        int id = r.GetInt32(0);
        int roomId = r.GetInt32(1);
        string model = r.GetString(2);
        double power = r.GetDouble(3);
        string type = r.GetString(4);

        if (type == "Sensor")
            return new SmartSensor(id, roomId, model, power, Math.Round(0.1 * (1 + id % 5), 1));
        return new SmartActuator(id, roomId, model, power, Math.Round(power * 1.25));
    }
}
