using System.Text;
using PZ7;

Console.OutputEncoding = Encoding.UTF8;

// ===== Задание 1: Money =====
Console.WriteLine("=== Задание 1: класс Money ===");
Money m1 = new Money(150, 80);
Money m2 = new Money(50, 40);
Console.WriteLine($"m1 = {m1}, m2 = {m2}");
Console.WriteLine($"m1 + m2  = {m1 + m2}   (ожидается 201 руб. 20 коп.)");
Console.WriteLine($"m1 - m2  = {m1 - m2}");
Console.WriteLine($"m1 + 100 = {m1 + 100}  (ожидается 250 руб. 80 коп.)");
Console.WriteLine($"Нормализация: Money(1, 250) = {new Money(1, 250)}  (ожидается 3 руб. 50 коп.)");
if (m1 > m2) Console.WriteLine("m1 больше m2");
if (m2 < m1) Console.WriteLine("m2 меньше m1");
Console.WriteLine($"m1 == m2: {m1 == m2}, m1 != m2: {m1 != m2}");
if (m1) Console.WriteLine("Счет m1 активен (баланс > 0)");
Money empty = new Money(0, 0);
if (!empty) Console.WriteLine("Счет empty пустой (оператор !)");

// ===== Задание 2: приведение типов =====
Console.WriteLine();
Console.WriteLine("=== Задание 2: приведение типов ===");
Money m = 12550L;                               // implicit long -> Money
Console.WriteLine($"Money m = 12550L  ->  {m}");
long allKopeks = (long)m;                        // explicit Money -> long
Console.WriteLine($"(long)m = {allKopeks}");
CurrencyRecord cr = (CurrencyRecord)m;           // explicit Money -> CurrencyRecord
Console.WriteLine($"(CurrencyRecord)m = {cr}");
Money fromCr = (Money)cr;                        // explicit CurrencyRecord -> Money
Console.WriteLine($"(Money)cr = {fromCr}");
Money rounded = (Money)new CurrencyRecord { Code = "RUB", Amount = 10.456m };
Console.WriteLine($"(Money) от 10.456 = {rounded}  (округление до копеек)");

// ===== Задание 3: AccountBalance =====
Console.WriteLine();
Console.WriteLine("=== Задание 3: AccountBalance (в .NET 9 вместо += используем Add) ===");

AccountBalance acc = new AccountBalance(1000m);
Console.WriteLine($"Начальный Id: {acc.Id}");
for (int i = 0; i < 10000; i++)
    acc.Add(10m);                                // меняем объект на месте
Console.WriteLine($"Итоговый Id: {acc.Id}  (остался прежним, объект не пересоздавался)");
Console.WriteLine(acc);

Console.WriteLine();
Console.WriteLine("Для сравнения, старый способ (статический operator +):");
AccountBalance old = new AccountBalance(1000m);
Console.WriteLine($"Начальный Id: {old.Id}");
for (int i = 0; i < 3; i++)
    old += 10m;                                  // создаёт новый объект каждый раз
Console.WriteLine($"Id после 3 шагов: {old.Id}  (каждый шаг создал новый объект)");

Console.WriteLine();
acc.Increment();
Console.WriteLine($"После Increment() (на месте): {acc}");
AccountBalance next = ++acc;                     // статический ++ вернёт новый объект
Console.WriteLine($"next = ++acc: {next}");

Console.WriteLine();
PremiumAccount prem = new PremiumAccount(500m);
prem.Add(100m);                                  // вызовется переопределённый Add
Console.WriteLine($"PremiumAccount: 500 + 100 + 5% = {prem.TotalBalance}  (ожидается 605)");

AccountBalance poly = new PremiumAccount(0m);    // полиморфизм
poly.Add(200m);
Console.WriteLine($"Полиморфизм: {poly.TotalBalance}  (ожидается 210)");
