namespace Practicum.Operators;

public class ExpressInventory : Inventory
{
    // срочная партия приходит с двойным весом
    public override void operator +=(int units) => TotalUnits += units * 2;
}
