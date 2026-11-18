namespace CorpFleet.Models;

/// <summary>Вложенный сервисный объект — нужен, чтобы показать глубокое копирование (Deep Copy).</summary>
public class ServiceInfo
{
    public string ServiceName { get; set; } = "Плановое ТО";
    public List<string> Notes { get; private set; } = new();

    public ServiceInfo DeepCopy() => new ServiceInfo
    {
        ServiceName = ServiceName,
        Notes = new List<string>(Notes) // новый список, а не ссылка на старый
    };

    public override string ToString() => $"{ServiceName}; заметок: {Notes.Count}";
}
