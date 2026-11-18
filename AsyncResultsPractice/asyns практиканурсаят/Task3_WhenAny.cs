using System.Globalization;

namespace AsyncResultsPractice;

public static class Task3_WhenAny
{
    private static readonly CultureInfo UsCulture = CultureInfo.GetCultureInfo("en-US"); // чтобы {price:C} давал $

    public static async Task<string> FetchPriceFromExchangeAsync(string exchangeName, int delayMs, decimal price)
    {
        await Task.Delay(delayMs);
        return $"{exchangeName}: {price.ToString("C", UsCulture)}";
    }

    public static async Task RunAsync()
    {
        Console.WriteLine("=== ЗАДАНИЕ 3: Task.WhenAny ===");

        var tAlpha = FetchPriceFromExchangeAsync("Alpha", 1800, 50000m);
        var tBeta = FetchPriceFromExchangeAsync("Beta", 600, 50200m);
        var tGamma = FetchPriceFromExchangeAsync("Gamma", 2200, 49900m);

        // WhenAny возвращает Task<Task<string>>: await даёт задачу-победителя
        Task<string> winnerTask = await Task.WhenAny(tAlpha, tBeta, tGamma);
        string fastestResult = await winnerTask; // извлекаем значение из победителя
        Console.WriteLine($"Быстрейший ответ: {fastestResult}");

        // Дожидаемся остальных, чтобы не оставлять фоновые операции
        string[] all = await Task.WhenAll(tAlpha, tBeta, tGamma);
        Console.WriteLine($"Все ответы: {string.Join(" | ", all)}");
        Console.WriteLine();
    }
}
