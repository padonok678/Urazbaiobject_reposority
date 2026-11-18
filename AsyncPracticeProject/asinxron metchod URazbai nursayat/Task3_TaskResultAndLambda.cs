using System;
using System.Threading.Tasks;

namespace AsyncPractice;

public static class Task3_TaskResultAndLambda
{
    public static async Task<int> GetSquareAsync(int number, int delayMs)
    {
        await Task.Delay(delayMs);
        return number * number;
    }

    public static async Task RunAsync()
    {
        Console.WriteLine("=== ЗАДАНИЕ 3: Task<T> и асинхронная лямбда ===");

        int result = await GetSquareAsync(12, 1000);
        Console.WriteLine($"Квадрат числа 12: {result}");

        Func<string, int, Task<string>> formatter = async (text, delay) =>
        {
            await Task.Delay(delay);
            return $"Результат: {text.ToUpper()}";
        };

        string formatted = await formatter("async lambda", 500);
        Console.WriteLine(formatted);
        Console.WriteLine();
    }
}
