namespace KP2.Models;

public class SmartActuator : Device
{
    public double LoadCapacity { get; private set; }

    public SmartActuator(int id, int roomId, string model, double power, double loadCapacity, DevicePassport? passport = null)
        : base(id, roomId, model, power, passport)
    {
        if (loadCapacity <= 0) throw new ArgumentException("Нагрузка должна быть больше нуля.");
        LoadCapacity = loadCapacity;
    }

    public override string DeviceType => "Actuator";

    public override string GetStatus() => base.GetStatus() + $", нагрузка до {LoadCapacity:0} Вт";

    // включённый работает 8 часов в сутки, выключенный потребляет 0,5 Вт в режиме ожидания
    public override double CalculatePowerConsumption() => _isActive ? Power * 8 / 1000 : 0.5 * 24 / 1000;

    public new string Describe() => $"Исполнительное устройство «{Model}» на {LoadCapacity:0} Вт";
}
