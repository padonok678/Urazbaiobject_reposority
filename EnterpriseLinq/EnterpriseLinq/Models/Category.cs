namespace EnterpriseLinq.Models;

public class Category
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public double DiscountRate { get; private set; }

    public Category(int id, string name, double discountRate)
    {
        Id = id;
        Name = name;
        DiscountRate = discountRate;
    }

    public override string ToString() => Name;
}
