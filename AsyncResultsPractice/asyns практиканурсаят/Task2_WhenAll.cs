using System.Diagnostics;

namespace AsyncResultsPractice;

public static class Task2_WhenAll
{
    public static async Task<int> ComputeSquareAsync(int n, int delayMs)
    {
        await Task.Delay(delayMs);
        Console.WriteLine($"  [Thread {Environment.CurrentManagedThreadId}] Квадрат {n} вычислен");
        return n * n;
    }

    public static async Task ExecuteSequentialAsync()
    {
        Console.WriteLine("Последовательно:");
        var sw = Stopwatch.StartNew();

        int r1 = await ComputeSquareAsync(4, 1000);
        int r2 = await ComputeSquareAsync(5, 1000);
        int r3 = await ComputeSquareAsync(6, 1000);

        sw.Stop();
        Console.WriteLine($"  Результаты: {r1}, {r2}, {r3}. Время: {sw.ElapsedMilliseconds} мс (~3000)");
    }

    public static async Task ExecuteWhenAllAsync()
    {
        Console.WriteLine("Параллельно (Task.WhenAll):");
        var sw = Stopwatch.StartNew();

        var task1 = ComputeSquareAsync(4, 1000);
        var task2 = ComputeSquareAsync(5, 1000);
        var task3 = ComputeSquareAsync(6, 1000);

        int[] results = await Task.WhenAll(task1, task2, task3);

        sw.Stop();
        Console.WriteLine($"  Результаты (массив): {string.Join(", ", results)}. Время: {sw.ElapsedMilliseconds} мс (~1000)");

        // Шаг 2.4: после await Task.WhenAll все задачи завершены, поэтому .Result здесь
        // не блокирует поток (чтение готового значения).
        Console.WriteLine($"  Через свойства Result: {task1.Result}, {task2.Result}, {task3.Result}");
    }

    public static async Task RunAsync()
    {
        Console.WriteLine("=== ЗАДАНИЕ 2: Task.WhenAll ===");
        await ExecuteSequentialAsync();
        await ExecuteWhenAllAsync();
        Console.WriteLine();
    }
}
