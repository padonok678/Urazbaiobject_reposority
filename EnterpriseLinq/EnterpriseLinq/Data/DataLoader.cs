using System.Globalization;
using Microsoft.Data.Sqlite;
using EnterpriseLinq.Models;

namespace EnterpriseLinq.Data;

public static class DataLoader
{
    private static List<T> Query<T>(string sql, Func<SqliteDataReader, T> map)
    {
        using var connection = DatabaseInitializer.Open();
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        using var reader = command.ExecuteReader();

        var items = new List<T>();
        while (reader.Read())
            items.Add(map(reader));
        return items;
    }

    public static List<Category> LoadCategories() =>
        Query("SELECT Id, Name, DiscountRate FROM Categories",
              r => new Category(r.GetInt32(0), r.GetString(1), r.GetDouble(2)));

    public static List<Product> LoadProducts() =>
        Query("SELECT Id, Name, CategoryId, Price, Stock FROM Products",
              r => new Product(r.GetInt32(0), r.GetString(1), r.GetInt32(2), r.GetDouble(3), r.GetInt32(4)));

    public static List<Sale> LoadSales() =>
        Query("SELECT Id, ProductId, CustomerName, Quantity, SaleDate, Channel FROM Sales",
              r => new Sale(r.GetInt32(0), r.GetInt32(1), r.GetString(2), r.GetInt32(3),
                            DateTime.ParseExact(r.GetString(4), "yyyy-MM-dd", CultureInfo.InvariantCulture), r.GetString(5)));
}
