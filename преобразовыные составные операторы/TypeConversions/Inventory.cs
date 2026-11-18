namespace Practicum.Operators;

public class Inventory
{
    private static int lastId;

    public int Id { get; }
    public int TotalUnits { get; set; }

    public Inventory() => Id = ++lastId;

    public virtual void operator +=(int units) => TotalUnits += units;

    public void operator +=(Inventory other) => TotalUnits += other.TotalUnits;

 
    public void operator ++() => TotalUnits++;

   
    public static Inventory operator ++(Inventory inv) => new() { TotalUnits = inv.TotalUnits + 1 };

    public override string ToString() => $"Inventory #{Id}: {TotalUnits} шт.";
}
