using Microsoft.Data.Sqlite;

namespace CorpFleet.Data;

/// <summary>Чтение/запись SQLite. Результаты материализуются в List&lt;T&gt;.</summary>
public static class Database
{
    public static string DbPath => Path.Combine(AppContext.BaseDirectory, "corp_fleet.db");

    private static SqliteConnection Open()
    {
        var conn = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = DbPath }.ToString());
        conn.Open();
        return conn;
    }

    private static List<T> Query<T>(string sql, Func<SqliteDataReader, T> map)
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        using var reader = cmd.ExecuteReader();
        var list = new List<T>();
        while (reader.Read()) list.Add(map(reader));
        return list;
    }

    private static void Execute(string sql, params (string Name, object Value)[] parameters)
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        parameters.ToList().ForEach(p => cmd.Parameters.AddWithValue(p.Name, p.Value));
        cmd.ExecuteNonQuery();
    }

    public static List<CategoryRow> GetCategories() =>
        Query("SELECT Id, CategoryName, TaxRate FROM Categories",
              r => new CategoryRow(r.GetInt32(0), r.GetString(1), r.GetDouble(2)));

    public static List<VehicleRow> GetVehicles() =>
        Query("SELECT Id, Model, BasePrice, Power, CategoryId FROM Vehicles",
              r => new VehicleRow(r.GetInt32(0), r.GetString(1), r.GetDouble(2), r.GetInt32(3), r.GetInt32(4)));

    public static List<MaintenanceLogRow> GetLogs() =>
        Query("SELECT Id, VehicleId, ServiceName, Cost, LogDate, Channel FROM MaintenanceLogs",
              r => new MaintenanceLogRow(r.GetInt32(0), r.GetInt32(1), r.GetString(2), r.GetDouble(3), r.GetString(4), r.GetString(5)));

    public static void InsertVehicle(int id, string model, double price, int power, int categoryId) =>
        Execute("INSERT INTO Vehicles (Id, Model, BasePrice, Power, CategoryId) VALUES ($id, $m, $p, $pw, $c)",
                ("$id", id), ("$m", model), ("$p", price), ("$pw", power), ("$c", categoryId));

    public static void DeleteVehicle(int id)
    {
        Execute("DELETE FROM MaintenanceLogs WHERE VehicleId = $id", ("$id", id));
        Execute("DELETE FROM Vehicles WHERE Id = $id", ("$id", id));
    }

    public static void InsertLog(int vehicleId, string service, double cost, string date, string channel) =>
        Execute("INSERT INTO MaintenanceLogs (VehicleId, ServiceName, Cost, LogDate, Channel) VALUES ($v, $s, $c, $d, $ch)",
                ("$v", vehicleId), ("$s", service), ("$c", cost), ("$d", date), ("$ch", channel));
}
