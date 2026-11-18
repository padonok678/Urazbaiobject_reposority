namespace ObrabotkaAndGeneration;

public static class MathOperations
{
    public static double SafeDivide(double a, double b)
    {
        try
        {
            if (b == 0)
                throw new DivideByZeroException("Делитель равен нулю");

            double result = a / b;
            Console.WriteLine($"Результат {a} / {b} = {result}");
            return result;
        }
        catch (DivideByZeroException ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            return double.NaN;
        }
        finally
        {
            Console.WriteLine("Операция деления завершена");
        }
    }
}
