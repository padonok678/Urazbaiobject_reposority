namespace AsyncResultsPractice;

public record Person(string Name, int Age);

/// <summary>Кэш: ValueTask&lt;T&gt; возвращает значение синхронно, если оно уже есть.</summary>
public class DataService
{
    private readonly Dictionary<int, string> _cache = new();

    public ValueTask<string> GetDataAsync(int id)
    {
        if (_cache.ContainsKey(id))
            return new ValueTask<string>(_cache[id]); // синхронно, без аллокации Task

        return new ValueTask<string>(LoadAsync(id));  // асинхронная загрузка
    }

    private async Task<string> LoadAsync(int id)
    {
        await Task.Delay(1000); // имитация загрузки
        string data = $"Данные #{id}";
        _cache[id] = data;
        return data;
    }
}

public class BankAccount
{
    public event EventHandler<string>? Alert;
    public int Balance { get; private set; }

    public void Put(int sum)
    {
        Balance += sum;
        if (sum >= 1000)
            Alert?.Invoke(this, $"Крупное пополнение: {sum}. Баланс: {Balance}");
    }
}

public static class Task1_ReturnTypes
{
    // ---- Шаг 1.1: Task<T> ----
    public static async Task<Person> GetPersonAsync(string name, int age)
    {
        await Task.Delay(500); // имитация чтения из БД
        return new Person(name, age);
    }

    // ---- Пример из задания: ValueTask<int> ----
    public static ValueTask<int> CalculateBonusAsync(int score)
    {
        if (score < 50) return new ValueTask<int>(0);
        return CalculateBonusFromDbAsync(score);
    }

    private static async ValueTask<int> CalculateBonusFromDbAsync(int score)
    {
        await Task.Delay(300);
        return score * 2;
    }

    // ---- Шаг 1.4: async void — только обработчик события ----
    private static async void OnAlertReceived(object? sender, string msg)
    {
        try
        {
            await Task.Delay(300);
            Console.WriteLine($"  [Alert] {msg}");
        }
        catch (Exception ex)
        {
            // из async void исключения наружу не выйдут — ловим внутри
            Console.WriteLine($"  [Alert error] {ex.Message}");
        }
    }

    public static async Task RunAsync()
    {
        Console.WriteLine("=== ЗАДАНИЕ 1: Возвращаемые типы ===");

        // 1.1 Task<T>
        Person person = await GetPersonAsync("Анна", 25);
        Console.WriteLine($"Task<T>: {person}");

        // 1.2 / 1.3 ValueTask<T> + AsTask()
        var service = new DataService();

        Console.WriteLine("ValueTask: первый вызов (кэша нет, ~1 сек)...");
        string first = await service.GetDataAsync(1);
        Console.WriteLine($"  Получено: {first}");

        ValueTask<string> cached = service.GetDataAsync(1); // уже в кэше → синхронно
        Console.WriteLine($"  Второй вызов завершён синхронно: {cached.IsCompletedSuccessfully}");
        Console.WriteLine($"  Получено: {await cached}");

        ValueTask<string> valueTask = service.GetDataAsync(2);
        Task<string> asTask = valueTask.AsTask(); // преобразование в Task<string>
        Console.WriteLine($"AsTask(): {await asTask}");

        Console.WriteLine($"Бонус (score 30): {await CalculateBonusAsync(30)}, бонус (score 80): {await CalculateBonusAsync(80)}");

        // 1.4 async void в событии
        var account = new BankAccount();
        account.Alert += OnAlertReceived;
        Console.WriteLine("Генерация события:");
        account.Put(500);   // события нет
        account.Put(2000);  // событие
        await Task.Delay(600); // даём async void обработчику завершиться (await на него невозможен)
        Console.WriteLine();
    }
}
