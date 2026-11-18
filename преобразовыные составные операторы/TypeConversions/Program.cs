using Practicum.Operators;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// --- задание 1: преобразования типов ---
StorageSize size1 = 2_097_152L;
long rawBytes = (long)size1;
MegaByte mb = size1;
StorageSize size2 = (StorageSize)new MegaByte { Value = 1.5 };

Console.WriteLine($"long -> StorageSize: {size1}");
Console.WriteLine($"StorageSize -> long: {rawBytes}");
Console.WriteLine($"StorageSize -> MegaByte: {mb}");
Console.WriteLine($"1.5 МБ -> StorageSize: {size2}");

var tiny = new MegaByte { Value = 0.1 };
var tinyBytes = (StorageSize)tiny;
MegaByte back = tinyBytes;
Console.WriteLine($"0.1 МБ -> {tinyBytes} -> {back:0.##########} МБ (после округления байт значение уже не то)");

try
{
    _ = (StorageSize)new MegaByte { Value = double.MaxValue };
}
catch (OverflowException)
{
    Console.WriteLine("Слишком большое значение не влезает в long: OverflowException");
}

Console.WriteLine();

// --- задание 2: += по месту ---
var warehouse = new Inventory { TotalUnits = 100 };
Console.WriteLine($"До: {warehouse}");
warehouse += 50;
warehouse += 25;
Console.WriteLine($"После += 50, += 25: {warehouse}");

var annex = new Inventory { TotalUnits = 30 };
warehouse += annex;
Console.WriteLine($"После присоединения {annex}: {warehouse}");

Console.WriteLine();

// --- задание 3: ++ и виртуальный += ---
++warehouse;
Console.WriteLine($"++warehouse как отдельная команда: {warehouse}");

Inventory next = ++warehouse;
Console.WriteLine($"Inventory next = ++warehouse: next = {next}, warehouse = {warehouse}");
Console.WriteLine($"Это один и тот же объект: {ReferenceEquals(next, warehouse)}");

Inventory regular = new Inventory();
Inventory express = new ExpressInventory();
regular += 10;
express += 10;
Console.WriteLine($"Обычная партия, += 10: {regular}");
Console.WriteLine($"Срочная партия, += 10: {express}");

Console.WriteLine();
Console.WriteLine("Нажмите любую клавишу...");
Console.ReadKey();
