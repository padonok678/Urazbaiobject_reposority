using System;

namespace CSharp_Exception_Practicum_NET9;

internal static class StackUnwindDemo
{
    public static void Run()
    {
        try
        {
            Console.WriteLine("Main: начало");
            ServiceLevel();
            Console.WriteLine("Main: конец");
        }
        catch (DivideByZeroException ex)
        {
            Console.WriteLine($"[Catch в Main]: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("[Finally в Main]");
        }
    }

    private static void ServiceLevel()
    {
        try
        {
            DataLevel();
        }
        catch (IndexOutOfRangeException ex)
        {
            // Обработчик не подходит для DivideByZeroException.
            Console.WriteLine($"[Catch в ServiceLevel]: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("[Finally в ServiceLevel]");
        }
    }

    private static void DataLevel()
    {
        try
        {
            int x = 10;
            int y = 0;
            int z = x / y;
            Console.WriteLine(z);
        }
        finally
        {
            Console.WriteLine("[Finally в DataLevel]");
        }
    }
}

public class InsufficientFundsException : Exception
{
    public decimal Balance { get; }
    public decimal TargetAmount { get; }

    public InsufficientFundsException(string message, decimal balance, decimal amount)
        : base(message)
    {
        Balance = balance;
        TargetAmount = amount;
    }
}

public class InvalidTransactionException : ArgumentException
{
    public decimal Amount { get; }

    public InvalidTransactionException(string message, decimal amount)
        : base(message)
    {
        Amount = amount;
    }
}

public class BankAccount
{
    public decimal Balance { get; private set; }

    public BankAccount(decimal initialBalance)
    {
        if (initialBalance < 0)
            throw new ArgumentOutOfRangeException(nameof(initialBalance),
                "Начальный баланс не может быть отрицательным.");

        Balance = initialBalance;
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
            throw new InvalidTransactionException(
                "Сумма должна быть больше 0!", amount);

        if (amount > Balance)
            throw new InsufficientFundsException(
                "Недостаточно средств на счете!", Balance, amount);

        Balance -= amount;
    }
}

public class UserValidationException : ArgumentException
{
    public string FieldName { get; }
    public object? AttemptedValue { get; }

    public UserValidationException(string message, string fieldName, object? value)
        : base(message)
    {
        FieldName = fieldName;
        AttemptedValue = value;
    }
}

public static class UserService
{
    public static void ProcessRegistration(string name, int age)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new UserValidationException(
                "Имя не может быть пустым!", nameof(name), name);

        if (age < 18)
            throw new UserValidationException(
                "Возраст должен быть от 18 лет!", nameof(age), age);

        Console.WriteLine($"Пользователь {name} зарегистрирован.");
    }
}

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("========== ЗАДАНИЕ 1: STACK UNWINDING ==========");
        StackUnwindDemo.Run();

        Console.WriteLine();
        Console.WriteLine("========== ЗАДАНИЕ 2: CUSTOM EXCEPTIONS ==========");
        var account = new BankAccount(100m);

        try
        {
            account.Withdraw(-20m);
        }
        catch (InvalidTransactionException ex)
        {
            Console.WriteLine($"[InvalidTransactionException]: {ex.Message}");
            Console.WriteLine($"Некорректная сумма: {ex.Amount}");
        }

        try
        {
            account.Withdraw(150m);
        }
        catch (InsufficientFundsException ex)
        {
            Console.WriteLine($"[InsufficientFundsException]: {ex.Message}");
            Console.WriteLine($"Баланс: {ex.Balance}");
            Console.WriteLine($"Запрошенная сумма: {ex.TargetAmount}");
        }

        try
        {
            account.Withdraw(30m);
            Console.WriteLine($"Снятие выполнено. Остаток: {account.Balance}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("========== ЗАДАНИЕ 3: USER VALIDATION ==========");
        try
        {
            UserService.ProcessRegistration("Алия", 16);
        }
        catch (UserValidationException ex)
        {
            Console.WriteLine($"Ошибка регистрации: {ex.Message}");
            Console.WriteLine($"Ошибочное поле: {ex.FieldName}");
            Console.WriteLine($"Переданное значение: {ex.AttemptedValue}");
        }

        Console.WriteLine();
        Console.WriteLine("Нажмите Enter для завершения...");
        Console.ReadLine();
    }
}
