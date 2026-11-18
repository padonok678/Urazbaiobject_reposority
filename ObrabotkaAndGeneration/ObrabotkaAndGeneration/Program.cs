using ObrabotkaAndGeneration;

internal static class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("Задание 1: try-catch-finally");
        try
        {
            double res1 = MathOperations.SafeDivide(10, 2);
            double res2 = MathOperations.SafeDivide(5, 0);
            Console.WriteLine($"Получено: {res1} и {res2}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Внешний перехват: {ex.Message}");
        }
        Console.WriteLine();

        Console.WriteLine("Задание 2: каскад catch when");
        var processor = new AccountProcessor();
        processor.ProcessTransaction(1, 500);
        processor.ProcessTransaction(1, -50);
        processor.ProcessTransaction(1, -5000);
        processor.ProcessTransaction(2, 100);
        DemoWhenFilter();
        Console.WriteLine();

        Console.WriteLine("Задание 3: throw и повторный проброс");
        RegisterWithLogging("A", 25);
        RegisterWithLogging("Анна", 15);
        RegisterWithLogging("Анна", 25);
        ShowThrowDifference();

        Console.WriteLine();
        Console.WriteLine("Нажмите любую клавишу...");
        Console.ReadKey();
    }

    static void DemoWhenFilter()
    {
        int x = 0, y = 1;
        try
        {
            int result1 = x / y;
            int result2 = y / x;
            Console.WriteLine($"Результаты: {result1}, {result2}");
        }
        catch (DivideByZeroException) when (y == 0)
        {
            Console.WriteLine("  y не должен быть равен 0");
        }
        catch (DivideByZeroException ex)
        {
            Console.WriteLine($"  Фильтр не подошёл, обработано во втором catch: {ex.Message}");
        }
    }

    static void RegisterUser(string? name, int age)
    {
        if (name == null || name.Length < 2)
            throw new ArgumentException("Длина имени меньше 2 символов");
        if (age < 18)
            throw new ArgumentOutOfRangeException(nameof(age), age, "Возраст меньше 18 лет");

        Console.WriteLine($"  Пользователь {name} ({age}) зарегистрирован");
    }

    static void RegisterWithLogging(string name, int age)
    {
        Console.WriteLine($"RegisterUser(\"{name}\", {age}):");
        try
        {
            try
            {
                RegisterUser(name, age);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"  [Локальный лог] {ex.GetType().Name}: {ex.Message}");
                throw;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  [Глобальный лог] StackTrace:{Environment.NewLine}{ex.StackTrace}");
        }
    }

    static void ShowThrowDifference()
    {
        Console.WriteLine("throw; и throw ex; (есть ли RegisterUser в трассировке):");
        Console.WriteLine($"  throw;    -> {Rethrow(useThrowEx: false)}");
        Console.WriteLine($"  throw ex; -> {Rethrow(useThrowEx: true)}");
    }

    static bool Rethrow(bool useThrowEx)
    {
        try
        {
            try
            {
                RegisterUser("A", 25);
            }
            catch (ArgumentException ex)
            {
                if (useThrowEx) throw ex;
                throw;
            }
        }
        catch (Exception ex)
        {
            return ex.StackTrace?.Contains(nameof(RegisterUser)) ?? false;
        }
        return false;
    }
}
