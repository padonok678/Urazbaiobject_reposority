using KP2.Models;

namespace KP2.Analytics;

public static class ReportAnalytics
{
    // Join (устройство + комната), затем GroupJoin с журналом телеметрии
    public static List<DeviceReportDto> BuildDto(
        IEnumerable<Device> devices, IEnumerable<Room> rooms, IEnumerable<TelemetryLog> logs) =>
        devices
            .Join(rooms, d => d.RoomId, r => r.Id, (d, r) => new { Device = d, Room = r })
            .GroupJoin(logs, x => x.Device.Id, l => l.DeviceId, (x, ls) => new DeviceReportDto(
                x.Device.Id, x.Device.Model, x.Device.DeviceType, x.Device.Power,
                x.Room.Title, x.Room.Floor,
                ls.Count(), ls.Any() ? ls.Average(l => l.Reading) : 0))
            .ToList();

    public static IEnumerable<DeviceReportDto> Filter(
        IEnumerable<DeviceReportDto> source, string? deviceType, string? text, double minPower)
    {
        var query = source.Where(d => d.Power >= minPower);

        if (!string.IsNullOrEmpty(deviceType))
            query = query.Where(d => d.DeviceType == deviceType);

        if (!string.IsNullOrWhiteSpace(text))
            query = query.Where(d => d.Model.Contains(text, StringComparison.OrdinalIgnoreCase)
                                  || d.RoomTitle.Contains(text, StringComparison.OrdinalIgnoreCase));

        return query.OrderBy(d => d.Floor).ThenByDescending(d => d.Power);
    }

    public static List<T> Page<T>(IEnumerable<T> source, int page, int pageSize) =>
        source.Skip((page - 1) * pageSize).Take(pageSize).ToList();

    public static List<RoomStatDto> RoomStats(IEnumerable<DeviceReportDto> dtos) =>
        dtos.GroupBy(d => d.RoomTitle)
            .Select(g => new RoomStatDto(g.Key, g.Count(), g.Sum(d => d.Power), g.Average(d => d.Power), g.Max(d => d.Power)))
            .OrderBy(s => s.RoomTitle)
            .ToList();

    public static string BuildSummary(IEnumerable<DeviceReportDto> source)
    {
        var dtos = source.ToList();
        if (dtos.Count == 0) return "Нет данных по выбранным фильтрам.";

        string byRoom = RoomStats(dtos).Aggregate(
            "Сводка по комнатам:",
            (text, s) => text + $"\r\n • {s.RoomTitle}: {s.DeviceCount} устр., Σ {s.TotalPower:N1} Вт, в среднем {s.AveragePower:N1} Вт");

        bool anyHeavy = dtos.Any(d => d.Power > 1500);
        bool allPowered = dtos.All(d => d.Power > 0);
        var firstActuator = dtos.FirstOrDefault(d => d.DeviceType == "Actuator");
        string firstName = firstActuator == null ? "нет" : $"{firstActuator.Model} ({firstActuator.RoomTitle})";

        return byRoom +
               $"\r\n\r\nВсего: {dtos.Count} устройств, Σ {dtos.Sum(d => d.Power):N1} Вт" +
               $"\r\nAny: есть прибор мощнее 1500 Вт: {anyHeavy}" +
               $"\r\nAll: у всех мощность больше нуля: {allPowered}" +
               $"\r\nFirstOrDefault: первый исполнитель: {firstName}";
    }

    public static List<string> ModelsInRoom(IEnumerable<DeviceReportDto> dtos, string roomTitle) =>
        dtos.Where(d => d.RoomTitle == roomTitle).Select(d => d.Model).Distinct().ToList();

    public static List<string> CommonModels(IEnumerable<DeviceReportDto> dtos, string roomA, string roomB) =>
        ModelsInRoom(dtos, roomA).Intersect(ModelsInRoom(dtos, roomB)).ToList();

    public static List<string> OnlyInFirst(IEnumerable<DeviceReportDto> dtos, string roomA, string roomB) =>
        ModelsInRoom(dtos, roomA).Except(ModelsInRoom(dtos, roomB)).ToList();

    public static List<string> AllModels(IEnumerable<DeviceReportDto> dtos, string roomA, string roomB) =>
        ModelsInRoom(dtos, roomA).Union(ModelsInRoom(dtos, roomB)).ToList();

    public static string DeferredDemo(IEnumerable<DeviceReportDto> dtos)
    {
        var source = dtos.ToList();

        IEnumerable<DeviceReportDto> deferred = source.Where(d => d.Power > 1000);
        List<DeviceReportDto> snapshot = source.Where(d => d.Power > 1000).ToList();
        int before = snapshot.Count;

        source.Add(new DeviceReportDto(0, "Новый обогреватель", "Actuator", 2000, "—", 0, 0, 0));

        return "Отложенное и немедленное выполнение (мощнее 1000 Вт):\r\n" +
               $" • до добавления прибора в источник: {before}\r\n" +
               $" • отложенный IEnumerable после добавления: {deferred.Count()} (запрос выполнился заново)\r\n" +
               $" • ToList() после добавления: {snapshot.Count} (результат уже сохранён)";
    }
}
