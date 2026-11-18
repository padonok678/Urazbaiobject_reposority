namespace WarehouseApp.Models;

/// <summary>Таблица Categories.</summary>
public class Category
{
    public int Id { get; set; }                       // INTEGER PRIMARY KEY
    public string Name { get; set; } = "";            // TEXT NOT NULL
    public List<Product> Products { get; set; } = new();
}

/// <summary>Таблица Products.</summary>
public class Product
{
    public int Id { get; set; }                       // INTEGER PRIMARY KEY
    public string Name { get; set; } = "";            // TEXT NOT NULL
    public decimal Price { get; set; }                // REAL NOT NULL
    public int Quantity { get; set; }                 // INTEGER NOT NULL
    public int CategoryId { get; set; }               // INTEGER, FK -> Categories(Id)
    public Category Category { get; set; }
}
