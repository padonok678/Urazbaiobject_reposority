namespace CorpFleet.Models;

public class PassengerVehicle : Vehicle, IExportable
{
    public int Seats { get; private set; }

    public PassengerVehicle(int id, string model, int power, double price, int seats)
        : base(id, model, power, price)
    {
        if (seats <= 0) throw new ArgumentException("Число мест должно быть больше 0", nameof(seats));
        Seats = seats;
    }

    public override string GetVehicleType() => "Пассажирский";

    public override double CalculateTax() => base.CalculateTax() + Power * 2;

    public override string ToString() => base.ToString() + $", мест {Seats}";

    public string GetExportInfo() => $"PASSENGER;{Id};{Model};{Power};{BasePrice};{Seats}";

    protected override Vehicle CloneCore()
    {
        var copy = new PassengerVehicle(Id, Model, Power, BasePrice, Seats);
        copy.CopyStateFrom(this);
        return copy;
    }
}
