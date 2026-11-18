namespace CorpFleet.Data;

public record CategoryRow(int Id, string CategoryName, double TaxRate);
public record VehicleRow(int Id, string Model, double BasePrice, int Power, int CategoryId);
public record MaintenanceLogRow(int Id, int VehicleId, string ServiceName, double Cost, string LogDate, string Channel);
