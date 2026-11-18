using System.Diagnostics;

namespace ExceptionsPractice;

internal static class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("Задание 1: try-catch-finally");
        SafeDivide(20, 4);
        SafeDivide(7, 0);
        Console.WriteLine();

        Console.WriteLine("Задание 2: фильтры when");
        ProcessTransaction(0, 0);
        ProcessTransaction(-5, 3);
        ProcessTransaction(10, 2);
        Console.WriteLine();

        Console.WriteLine("Задание 3: TryParse вместо try-catch");
        foreach (string input in new[] { "abc", "15" })
        {
            Console.WriteLine($"Ввод \"{input}\":");
            Console.Write("  try-catch: ");
            CalculateSquareTryCatch(input);
            Console.Write("  TryParse:  ");
            CalculateSquareTryParse(input);
        }
        CompareSpeed(2000);

        Console.WriteLine();
        Console.WriteLine("Нажмите любую клавишу...");
        Console.ReadKey();
    }

    static int Divide(int a, int b) => a / b;

    static void SafeDivide(int x, int y)
    {
        try
        {
            int result = Divide(x, y);
            Console.WriteLine($"Результат {x} / {y} = {result}");
        }
        catch (DivideByZeroException ex)
        {
            Console.WriteLine("Ошибка деления на 0: " + ex.Message);
        }
        finally
        {
            Console.WriteLine("Операция деления завершена (блок finally)");
        }
    }

    static void ProcessTransaction(int errorCode, int attempts)
    {
        Console.WriteLine($"ProcessTransaction({errorCode}, {attempts}):");
        try
        {
            int share = Divide(1000, 0);
            Console.WriteLine(share);
        }
        catch (DivideByZeroException) when (attempts == 0)
        {
            Console.WriteLine("  Блок 1: попытки исчерпаны, операция прервана");
        }
        catch (DivideByZeroException ex) when (errorCode < 0)
        {
            Console.WriteLine($"  Блок 2: отрицательный код системы {errorCode} ({ex.Message})");
        }
        catch (DivideByZeroException ex)
        {
            Console.WriteLine($"  Блок 3: деление на ноль ({ex.Message})");
        }
    }

    static void CalculateSquareTryCatch(string input)
    {
        try
        {
            int value = int.Parse(input);
            Console.WriteLine($"квадрат {value * value}");
        }
        catch (FormatException)
        {
            Console.WriteLine("некорректный формат числа");
        }
    }

    // При throw .NET собирает стек вызовов и раскручивает его, это на порядки дороже обычной проверки.
    // TryParse сообщает о плохом вводе через bool, и пользовательский ввод ошибочен достаточно часто.
    static void CalculateSquareTryParse(string input)
    {
        if (int.TryParse(input, out int value))
            Console.WriteLine($"квадрат {value * value}");
        else
            Console.WriteLine("некорректный ввод (без исключения)");
    }

    static void CompareSpeed(int iterations)
    {
        var timer = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            try { _ = int.Parse("abc"); }
            catch (FormatException) { }
        }
        timer.Stop();
        double withException = timer.Elapsed.TotalMilliseconds;

        timer.Restart();
        for (int i = 0; i < iterations; i++)
            _ = int.TryParse("abc", out _);
        timer.Stop();
        double withTryParse = timer.Elapsed.TotalMilliseconds;

        Console.WriteLine($"{iterations} разборов строки \"abc\": try-catch {withException:F1} мс, TryParse {withTryParse:F3} мс");
    }
}
