namespace EnterpriseLinq.Models;

public class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int CategoryId { get; private set; }
    public double Price { get; private set; }
    public int Stock { get; private set; }

    public Product(int id, string name, int categoryId, double price, int stock)
    {
        Id = id;
        Name = name;
        CategoryId = categoryId;
        Price = price;
        Stock = stock;
    }

    // Except / Intersect / Union сравнивают товары именно по Id
    public override bool Equals(object? obj) => obj is Product other && Id == other.Id;

    public override int GetHashCode() => Id.GetHashCode();

    public override string ToString() => $"#{Id} {Name}";
}
