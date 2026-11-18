using System.ComponentModel;

namespace KP2.Analytics;

public class DeviceReportDto
{
    [DisplayName("Id")] public int DeviceId { get; }
    [DisplayName("Модель")] public string Model { get; }
    [DisplayName("Тип")] public string DeviceType { get; }
    [DisplayName("Мощность, Вт")] public double Power { get; }
    [DisplayName("Комната")] public string RoomTitle { get; }
    [DisplayName("Этаж")] public int Floor { get; }
    [DisplayName("Замеров")] public int LogsCount { get; }
    [DisplayName("Среднее показание")] public double AvgReading { get; }

    public DeviceReportDto(int deviceId, string model, string deviceType, double power,
                           string roomTitle, int floor, int logsCount, double avgReading)
    {
        DeviceId = deviceId;
        Model = model;
        DeviceType = deviceType;
        Power = power;
        RoomTitle = roomTitle;
        Floor = floor;
        LogsCount = logsCount;
        AvgReading = avgReading;
    }
}

public class RoomStatDto
{
    [DisplayName("Комната")] public string RoomTitle { get; }
    [DisplayName("Устройств")] public int DeviceCount { get; }
    [DisplayName("Σ мощность, Вт")] public double TotalPower { get; }
    [DisplayName("Средняя, Вт")] public double AveragePower { get; }
    [DisplayName("Максимум, Вт")] public double MaxPower { get; }

    public RoomStatDto(string roomTitle, int deviceCount, double totalPower, double averagePower, double maxPower)
    {
        RoomTitle = roomTitle;
        DeviceCount = deviceCount;
        TotalPower = totalPower;
        AveragePower = averagePower;
        MaxPower = maxPower;
    }
}
