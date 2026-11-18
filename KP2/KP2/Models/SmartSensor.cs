namespace KP2.Models;

public class SmartSensor : Device
{
    public double Accuracy { get; private set; }

    public SmartSensor(int id, int roomId, string model, double power, double accuracy, DevicePassport? passport = null)
        : base(id, roomId, model, power, passport)
    {
        if (accuracy <= 0) throw new ArgumentException("Точность должна быть больше нуля.");
        Accuracy = accuracy;
    }

    public override string DeviceType => "Sensor";

    public override string GetStatus() => base.GetStatus() + $", точность ±{Accuracy}";

    // датчик работает круглосуточно, кВт·ч в сутки
    public override double CalculatePowerConsumption() => Power * 24 / 1000;
}
