namespace CorpFleet.Models;

public abstract class Vehicle : IStorable, IComparable<Vehicle>, ICloneable
{
    public int Id { get; private set; }
    public string Model { get; private set; }
    public int Power { get; private set; }
    public double BasePrice { get; private set; }

    public int CategoryId { get; private set; }
    public string CategoryName { get; private set; } = "—";
    public double TaxRate { get; private set; }

    public ServiceInfo Service { get; private set; }
    public string FilePath { get; set; }

    protected Vehicle(int id, string model, int power, double price)
    {
        if (id < 0) throw new ArgumentException("Id не может быть отрицательным", nameof(id));
        if (string.IsNullOrWhiteSpace(model)) throw new ArgumentException("Модель не может быть пустой", nameof(model));
        if (power <= 0) throw new ArgumentException("Мощность должна быть больше 0", nameof(power));
        if (price < 0) throw new ArgumentException("Стоимость не может быть отрицательной", nameof(price));

        Id = id;
        Model = model.Trim();
        Power = power;
        BasePrice = price;
        Service = new ServiceInfo();
        FilePath = Path.Combine(Path.GetTempPath(), $"vehicle_{id}.txt");
    }

    public void AssignCategory(int categoryId, string categoryName, double taxRate)
    {
        CategoryId = categoryId;
        CategoryName = categoryName;
        TaxRate = taxRate;
    }

    // abstract — реализация обязана быть в наследниках
    public abstract string GetVehicleType();

    // virtual — есть реализация по умолчанию, наследники могут переопределить (override)
    public virtual double CalculateTax() => BasePrice * TaxRate;

    // ---- System.Object ----
    public override string ToString() => $"[{GetVehicleType()}] #{Id} {Model}, {Power} л.с., {BasePrice:N0}";

    public override bool Equals(object obj) =>
        obj is Vehicle other &&
        string.Equals(Model, other.Model, StringComparison.OrdinalIgnoreCase) &&
        Power == other.Power;

    // Равные объекты обязаны давать одинаковый хэш — используем те же поля, что и в Equals
    public override int GetHashCode() => HashCode.Combine(Model.ToLowerInvariant(), Power);

    // ---- IComparable: сортировка по BasePrice ----
    public int CompareTo(Vehicle other)
    {
        if (other is null) return 1;
        return BasePrice.CompareTo(other.BasePrice);
    }

    // ---- IStorable ----
    public void Save() => File.WriteAllText(FilePath, ToString() + Environment.NewLine + Service);

    // ---- ICloneable: глубокое копирование ----
    public object Clone() => CloneCore();

    public Vehicle CloneWithId(int newId)
    {
        var copy = CloneCore();
        copy.Id = newId;
        copy.FilePath = Path.Combine(Path.GetTempPath(), $"vehicle_{newId}.txt");
        return copy;
    }

    protected abstract Vehicle CloneCore();

    protected void CopyStateFrom(Vehicle source)
    {
        CategoryId = source.CategoryId;
        CategoryName = source.CategoryName;
        TaxRate = source.TaxRate;
        FilePath = source.FilePath;
        Service = source.Service.DeepCopy(); // Deep Copy вложенного объекта
    }
}
