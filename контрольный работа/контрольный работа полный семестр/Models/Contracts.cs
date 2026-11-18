namespace CorpFleet.Models;

/// <summary>Обобщённый репозиторий. where T : class — T обязан быть ссылочным типом.</summary>
public interface IRepository<T> where T : class
{
    List<T> GetAll();
    T GetById(int id);
    void Add(T item);
    bool Delete(int id);
}

public interface IStorable
{
    string FilePath { get; set; }
    void Save();
}

public interface IExportable
{
    string GetExportInfo();
}
