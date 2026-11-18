namespace ObrabotkaAndGeneration;

public class AccountProcessor
{
    public void ProcessTransaction(int status, int amount)
    {
        Console.WriteLine($"Транзакция: статус {status}, сумма {amount}");
        try
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Сумма не может быть отрицательной");
            if (status == 2)
                throw new InvalidOperationException("Счёт заблокирован");

            Console.WriteLine("  Проведена успешно");
        }
        catch (ArgumentOutOfRangeException ex) when (amount < -1000)
        {
            Console.WriteLine($"  Критически недопустимая сумма! ({ex.ActualValue})");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"  Сумма вне допустимого диапазона: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Непредвиденная ошибка ({ex.GetType().Name}): {ex.Message}");
        }
    }
}
