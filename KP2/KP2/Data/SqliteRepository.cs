using Microsoft.Data.Sqlite;
using KP2.Models;

namespace KP2.Data;

public class SqliteRepository<T> : IRepository<T> where T : class
{
    private readonly string _selectAll;
    private readonly string _selectById;
    private readonly string _insert;
    private readonly Func<SqliteDataReader, T> _map;
    private readonly Action<SqliteCommand, T> _bind;

    public SqliteRepository(string selectAll, string selectById, string insert,
                            Func<SqliteDataReader, T> map, Action<SqliteCommand, T> bind)
    {
        _selectAll = selectAll;
        _selectById = selectById;
        _insert = insert;
        _map = map;
        _bind = bind;
    }

    public List<T> GetAll()
    {
        using var connection = Db.Open();
        using var command = connection.CreateCommand();
        command.CommandText = _selectAll;
        using var reader = command.ExecuteReader();

        var items = new List<T>();
        while (reader.Read())
            items.Add(_map(reader));
        return items;
    }

    // Microsoft.Data.Sqlite работает синхронно, поэтому чтение уходит в пул потоков
    public Task<List<T>> GetAllAsync() => Task.Run(() => GetAll());

    public T? GetById(int id)
    {
        using var connection = Db.Open();
        using var command = connection.CreateCommand();
        command.CommandText = _selectById;
        command.Parameters.AddWithValue("$id", id);
        using var reader = command.ExecuteReader();
        return reader.Read() ? _map(reader) : null;
    }

    public void Add(T entity)
    {
        using var connection = Db.Open();
        using var command = connection.CreateCommand();
        command.CommandText = _insert;
        _bind(command, entity);
        command.ExecuteNonQuery();
    }
}
