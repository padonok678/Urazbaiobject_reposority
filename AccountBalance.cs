namespace PZ7;

/// <summary>
/// Задание 3: счёт, который изменяется "на месте".
/// В C# 14 для этого есть нестатические операторы += и ++.
/// В .NET 9 (C# 13) их нет, поэтому здесь они сделаны обычными методами
/// Add и Increment. Смысл тот же: объект меняется на месте, новый не создаётся.
/// </summary>
public class AccountBalance
{
    private static int _counter = 0;

    public int Id { get; }
    public decimal TotalBalance { get; set; }

    public AccountBalance(decimal balance)
    {
        Id = ++_counter;
        TotalBalance = balance;
    }

    // Аналог "public virtual void operator +=(decimal amount)" из C# 14
    public virtual void Add(decimal amount) => TotalBalance += amount;

    // Аналог "public void operator +=(AccountBalance other)" из C# 14
    public void Add(AccountBalance other) => TotalBalance += other.TotalBalance;

    // Аналог нестатического "public void operator ++()" из C# 14: бонус 1 рубль
    public void Increment() => TotalBalance += 1m;

    // Статический ++ (для записи AccountBalance next = ++acc;).
    // Он НЕ меняет аргумент, а возвращает новый объект.
    public static AccountBalance operator ++(AccountBalance a) =>
        new AccountBalance(a.TotalBalance + 1m);

    // Старый способ (до C# 14): статический +. Запись acc += 10m превращается
    // в acc = acc + 10m и создаёт НОВЫЙ объект каждый раз.
    public static AccountBalance operator +(AccountBalance a, decimal amount) =>
        new AccountBalance(a.TotalBalance + amount);

    public override string ToString() => $"Счёт №{Id}: {TotalBalance} руб.";
}
