namespace PZ7;

/// <summary>Задание 3: премиум-счёт с кэшбэком 5% при пополнении.</summary>
public class PremiumAccount : AccountBalance
{
    public PremiumAccount(decimal balance) : base(balance) { }

    // Аналог "public override void operator +=(decimal amount)" из C# 14
    public override void Add(decimal amount)
    {
        decimal cashback = amount * 0.05m;
        TotalBalance += amount + cashback;
    }
}
