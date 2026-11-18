namespace PZ7;

/// <summary>Задание 2: запись о валюте (код и сумма).</summary>
public class CurrencyRecord
{
    public string Code { get; set; } = "RUB";
    public decimal Amount { get; set; }

    public override string ToString() => $"{Amount} {Code}";
}
