namespace CorpFleet.Analytics;

public class FleetReportDto
{
    public int VehicleId { get; init; }
    public string Model { get; init; }
    public string CategoryName { get; init; }
    public int Power { get; init; }
    public double BasePrice { get; init; }
    public double TaxRate { get; init; }
    public double TaxAmount { get; init; }
    public int LogsCount { get; init; }
    public double MaintenanceCost { get; init; }
    public double TotalCost => BasePrice + TaxAmount + MaintenanceCost;
}

public class CategoryStatDto
{
    public string Category { get; init; }
    public int Count { get; init; }
    public double TotalPrice { get; init; }
    public double AvgPrice { get; init; }
    public double MinPrice { get; init; }
    public double MaxPrice { get; init; }
    public double MaintenanceTotal { get; init; }
}

/// <summary>Ключ записи обслуживания для Except/Intersect/Union: равенство — по ТС и названию услуги.</summary>
public class ServiceLogKey : IEquatable<ServiceLogKey>
{
    public int VehicleId { get; }
    public string Model { get; }
    public string ServiceName { get; }

    public ServiceLogKey(int vehicleId, string model, string serviceName)
    {
        VehicleId = vehicleId;
        Model = model;
        ServiceName = serviceName;
    }

    public bool Equals(ServiceLogKey other) =>
        other is not null && VehicleId == other.VehicleId && ServiceName == other.ServiceName;

    public override bool Equals(object obj) => Equals(obj as ServiceLogKey);

    public override int GetHashCode() => HashCode.Combine(VehicleId, ServiceName);

    public override string ToString() => $"#{VehicleId} {Model}: {ServiceName}";
}
