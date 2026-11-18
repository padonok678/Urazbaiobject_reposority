namespace KP2.Models;

public abstract class Device : IExportable, IComparable<Device>, ICloneable
{
    protected bool _isActive;

    public int Id { get; private set; }
    public int RoomId { get; private set; }
    public string Model { get; private set; }
    public double Power { get; private set; }
    public DevicePassport Passport { get; private set; }

    public bool IsActive => _isActive;
    public abstract string DeviceType { get; }

    public event EventHandler<DeviceEventArgs>? StatusChanged;

    protected Device(int id, int roomId, string model, double power, DevicePassport? passport = null)
    {
        if (id < 0) throw new ArgumentException("Id не может быть отрицательным.");
        if (string.IsNullOrWhiteSpace(model)) throw new ArgumentException("Укажите модель устройства.");
        if (power <= 0) throw new ArgumentException("Мощность должна быть больше нуля.");

        Id = id;
        RoomId = roomId;
        Model = model.Trim();
        Power = power;
        Passport = passport ?? new DevicePassport($"SN-{id:D5}", DateTime.Today, 0);
    }

    public virtual string GetStatus() => $"{(_isActive ? "включён" : "выключен")}";

    public abstract double CalculatePowerConsumption();

    public string Describe() => $"Устройство «{Model}»";

    public void Activate() => ChangeState(true);

    public void Deactivate() => ChangeState(false);

    private void ChangeState(bool active)
    {
        if (_isActive == active) return;
        _isActive = active;
        StatusChanged?.Invoke(this, new DeviceEventArgs($"{Model}: {(active ? "включён" : "выключен")}", DateTime.Now));
    }

    internal void RestoreState(bool active) => _isActive = active;

    public override string ToString() => $"[{DeviceType}] #{Id} {Model}, {Power:0.#} Вт";

    public override bool Equals(object? obj) => obj is Device other && Id == other.Id && Model == other.Model;

    public override int GetHashCode() => HashCode.Combine(Id, Model);

    public int CompareTo(Device? other) => other is null ? 1 : Power.CompareTo(other.Power);

    string IExportable.GetExportData() => $"{DeviceType};{Id};{Model};{Power};{Passport.SerialNumber}";

    public object Clone()
    {
        var copy = (Device)MemberwiseClone();
        copy.Passport = Passport.DeepCopy();
        copy.StatusChanged = null;
        return copy;
    }
}
