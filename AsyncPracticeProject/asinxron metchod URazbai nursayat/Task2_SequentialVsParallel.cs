using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AsyncPractice;

public static class Task2_SequentialVsParallel
{
    public static async Task DownloadDataAsync(string sourceName, int delayMs)
    {
        Console.WriteLine($"  Загрузка с {sourceName} началась...");
        await Task.Delay(delayMs); // имитация сетевой загрузки
        Console.WriteLine($"  Загрузка с {sourceName} завершена.");
    }

    public static async Task RunAsync()
    {
        Console.WriteLine("=== ЗАДАНИЕ 2: Последовательно vs параллельно ===");
        var sw = new Stopwatch();

        // Сценарий А: последовательно
        Console.WriteLine("Сценарий А (последовательно):");
        sw.Start();
        await DownloadDataAsync("Сервер 1", 2000);
        await DownloadDataAsync("Сервер 2", 2000);
        await DownloadDataAsync("Сервер 3", 2000);
        sw.Stop();
        Console.WriteLine($"Время сценария А: {sw.ElapsedMilliseconds} мс (задачи шли одна за другой, ~6000 мс)");
        Console.WriteLine();

        // Сценарий Б: параллельно
        Console.WriteLine("Сценарий Б (параллельно):");
        sw.Restart();
        var task1 = DownloadDataAsync("Сервер 1", 2000);
        var task2 = DownloadDataAsync("Сервер 2", 2000);
        var task3 = DownloadDataAsync("Сервер 3", 2000);
        await task1;
        await task2;
        await task3;
        sw.Stop();
        Console.WriteLine($"Время сценария Б: {sw.ElapsedMilliseconds} мс (все три задачи запущены одновременно, ~2000 мс)");
        Console.WriteLine();
    }
}
