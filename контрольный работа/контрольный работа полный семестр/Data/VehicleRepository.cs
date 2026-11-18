using CorpFleet.Models;

namespace CorpFleet.Data;

public class VehicleRepository : IRepository<Vehicle>
{
    private List<Vehicle> _items = new();

    public VehicleRepository() => Reload();

    public static bool IsCargoCategory(string categoryName) =>
        categoryName == "Грузовые" || categoryName == "Спецтехника";

    /// <summary>Читает БД и материализует строки в объекты ООП-модели.</summary>
    public void Reload()
    {
        _items = Database.GetVehicles()
            .Join(Database.GetCategories(), v => v.CategoryId, c => c.Id, (v, c) => CreateVehicle(v, c))
            .ToList();
    }

    private static Vehicle CreateVehicle(VehicleRow v, CategoryRow c)
    {
        Vehicle vehicle;
        if (IsCargoCategory(c.CategoryName))
            vehicle = new CargoVehicle(v.Id, v.Model, v.Power, v.BasePrice, Math.Round(v.Power / 25.0, 1));
        else
            vehicle = new PassengerVehicle(v.Id, v.Model, v.Power, v.BasePrice, c.CategoryName == "Автобусы" ? 40 : 5);

        vehicle.AssignCategory(c.Id, c.CategoryName, c.TaxRate);
        return vehicle;
    }

    public int NextId() => _items.Any() ? _items.Max(v => v.Id) + 1 : 1;

    public List<Vehicle> GetAll() => _items.ToList();

    public Vehicle GetById(int id) => _items.FirstOrDefault(v => v.Id == id);

    public void Add(Vehicle item)
    {
        Database.InsertVehicle(item.Id, item.Model, item.BasePrice, item.Power, item.CategoryId);
        _items.Add(item);
    }

    public bool Delete(int id)
    {
        // RemoveAll по Id, а не Remove(item): Equals у ТС сравнивает Model+Power, и клон равен оригиналу
        if (_items.All(v => v.Id != id)) return false;
        Database.DeleteVehicle(id);
        _items.RemoveAll(v => v.Id == id);
        return true;
    }
}
