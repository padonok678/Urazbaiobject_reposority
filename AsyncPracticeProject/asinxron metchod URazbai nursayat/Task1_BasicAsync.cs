using System;
using System.Threading.Tasks;

namespace AsyncPractice;

public static class Task1_BasicAsync
{
    public static async Task PrintMessageAsync(string message, int delayMs)
    {
        Console.WriteLine($"[Async Start] Начало задачи: {message}");
        await Task.Delay(delayMs); // неблокирующая пауза
        Console.WriteLine($"[Async End] Завершена задача: {message}");
    }

    public static async Task RunAsync()
    {
        Console.WriteLine("=== ЗАДАНИЕ 1: Базовый асинхронный метод ===");

        await PrintMessageAsync("Загрузка конфигурации", 2000);
        Console.WriteLine("[Main] Продолжение работы основного потока");

        // Тяжёлая вычислительная операция в фоновом потоке (без Thread.Sleep)
        long sum = await Task.Run(() =>
        {
            long total = 0;
            for (int i = 0; i < 500_000_000; i++)
                total += i % 7;
            return total;
        });
        Console.WriteLine($"[Task.Run] Расчёт завершён. Результат: {sum}");
        Console.WriteLine();
    }
}
