using CorpFleet.Data;
using CorpFleet.Models;

namespace CorpFleet.Events;

public class FleetManager
{
    public VehicleRepository Repository { get; }

    public event Action<object, VehicleEventArgs> StatusChanged;

    public FleetManager(VehicleRepository repository) => Repository = repository;

    private void Raise(string message, double price) =>
        StatusChanged?.Invoke(this, new VehicleEventArgs(message, price));

    public void AddVehicle(Vehicle v)
    {
        Repository.Add(v);
        Raise($"Добавлено ТС: {v.Model}", v.BasePrice);
    }

    public Vehicle CloneVehicle(Vehicle original)
    {
        var copy = original.CloneWithId(Repository.NextId());
        copy.Service.Notes.Add($"Копия объекта #{original.Id}");
        Repository.Add(copy);
        Raise($"Клонировано ТС: {original.Model} (#{original.Id} → #{copy.Id})", copy.BasePrice);
        return copy;
    }

    public bool RemoveVehicle(Vehicle v)
    {
        bool ok = Repository.Delete(v.Id);
        if (ok) Raise($"Удалено ТС: {v.Model}", -v.BasePrice);
        return ok;
    }

    public void RegisterMaintenance(Vehicle v, string service, double cost, string channel)
    {
        Database.InsertLog(v.Id, service, cost, DateTime.Now.ToString("yyyy-MM-dd"), channel);
        Raise($"Обслуживание ({channel}): {v.Model} — {service}", cost);
    }

    // ---- Predicate / Func ----
    public List<Vehicle> FindAll(Predicate<Vehicle> filter) =>
        Repository.GetAll().Where(v => filter(v)).ToList();

    // Замыкание: лямбда захватывает параметр surcharge
    public Func<Vehicle, double> CreateTaxCalculator(double surcharge) =>
        v => v.CalculateTax() + surcharge;

    public string DemoClosures()
    {
        double minPower = 150;
        Predicate<Vehicle> powerful = v => v.Power >= minPower; // захватывает ПЕРЕМЕННУЮ minPower
        int before = FindAll(powerful).Count;

        minPower = 300; // меняем переменную — лямбда увидит новое значение
        int after = FindAll(powerful).Count;

        Func<Vehicle, double> taxWithFee = CreateTaxCalculator(500);
        double totalTax = Repository.GetAll().Sum(taxWithFee);

        return $"Замыкания:\r\n" +
               $" • Predicate (Power >= 150): {before} ТС\r\n" +
               $" • после minPower = 300 (та же лямбда): {after} ТС\r\n" +
               $" • Func налог + надбавка 500, суммарно: {totalTax:N0}";
    }
}
