namespace KP2.Models;

public interface IRepository<T> where T : class
{
    T? GetById(int id);
    List<T> GetAll();
    void Add(T entity);
}

public interface IExportable
{
    string GetExportData();
}
