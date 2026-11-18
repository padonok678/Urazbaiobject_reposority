namespace KP2.Models;

public class DevicePassport
{
    public string SerialNumber { get; private set; }
    public DateTime CalibrationDate { get; private set; }
    public double Offset { get; private set; }

    public DevicePassport(string serialNumber, DateTime calibrationDate, double offset)
    {
        SerialNumber = serialNumber;
        CalibrationDate = calibrationDate;
        Offset = offset;
    }

    public void Recalibrate(DateTime date, double offset)
    {
        CalibrationDate = date;
        Offset = offset;
    }

    public DevicePassport DeepCopy() => new(SerialNumber, CalibrationDate, Offset);

    public override string ToString() => $"{SerialNumber}, калибровка {CalibrationDate:dd.MM.yyyy}, поправка {Offset:0.##}";
}
