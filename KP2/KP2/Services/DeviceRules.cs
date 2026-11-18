using KP2.Models;

namespace KP2.Services;

public static class DeviceRules
{
    public static readonly Func<Device, double> NominalPower = d => d.Power;

    public static readonly Func<Device, double> DailyConsumption = d => d.CalculatePowerConsumption();

    // порог остаётся жить внутри возвращённой лямбды — это и есть замыкание
    public static Func<double, bool> CreateOverloadCheck(double thresholdWatts)
    {
        double limit = thresholdWatts;
        return value => value > limit;
    }

    public static Predicate<Device> Critical(Func<double, bool> overload, Func<Device, double> measure) =>
        d => overload(measure(d));

    public static List<Device> Where(IEnumerable<Device> source, Predicate<Device> match) =>
        source.Where(d => match(d)).ToList();
}
