using Microsoft.Data.Sqlite;

namespace KP2.Data;

public static class Db
{
    public static string DbPath => Path.Combine(AppContext.BaseDirectory, "smart_home.db");

    public static SqliteConnection Open()
    {
        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = DbPath,
            Mode = SqliteOpenMode.ReadWrite,
            ForeignKeys = true
        };
        var connection = new SqliteConnection(builder.ToString());
        connection.Open();
        return connection;
    }
}
