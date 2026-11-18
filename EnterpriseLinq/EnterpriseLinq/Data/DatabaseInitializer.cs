using Microsoft.Data.Sqlite;

namespace EnterpriseLinq.Data;

public static class DatabaseInitializer
{
    public static string DbPath => Path.Combine(AppContext.BaseDirectory, "enterprise.db");

    public static SqliteConnection Open()
    {
        var connection = new SqliteConnection(new SqliteConnectionStringBuilder
        {
            DataSource = DbPath,
            ForeignKeys = true
        }.ToString());
        connection.Open();
        return connection;
    }

    // Если файла нет или он пустой, создаёт таблицы и стартовые данные
    public static void Initialize()
    {
        using var connection = Open();

        using (var create = connection.CreateCommand())
        {
            create.CommandText = DbScripts.Schema;
            create.ExecuteNonQuery();
        }

        long categories;
        using (var check = connection.CreateCommand())
        {
            check.CommandText = "SELECT COUNT(*) FROM Categories";
            categories = (long)check.ExecuteScalar()!;
        }

        if (categories > 0) return;

        using var seed = connection.CreateCommand();
        seed.CommandText = DbScripts.Seed;
        seed.ExecuteNonQuery();
    }
}
