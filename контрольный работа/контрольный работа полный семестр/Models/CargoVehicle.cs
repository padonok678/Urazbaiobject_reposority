namespace CorpFleet.Models;

public class CargoVehicle : Vehicle, IExportable
{
    public double MaxLoadTon { get; private set; }

    public CargoVehicle(int id, string model, int power, double price, double maxLoadTon)
        : base(id, model, power, price)
    {
        if (maxLoadTon <= 0) throw new ArgumentException("Грузоподъёмность должна быть больше 0", nameof(maxLoadTon));
        MaxLoadTon = maxLoadTon;
    }

    public override string GetVehicleType() => "Грузовой";

    public override double CalculateTax() => base.CalculateTax() + MaxLoadTon * 100;

    public override string ToString() => base.ToString() + $", груз {MaxLoadTon} т";

    // Явная реализация: метод виден только через ссылку типа IExportable
    string IExportable.GetExportInfo() => $"CARGO;{Id};{Model};{Power};{BasePrice};{MaxLoadTon}";

    protected override Vehicle CloneCore()
    {
        var copy = new CargoVehicle(Id, Model, Power, BasePrice, MaxLoadTon);
        copy.CopyStateFrom(this);
        return copy;
    }
}
