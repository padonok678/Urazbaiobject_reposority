using Microsoft.EntityFrameworkCore;

namespace WarehouseApp.Data;

/// <summary>
/// Задание 1.1: создает warehouse.db и заполняет тестовыми данными (3 категории, 8 товаров).
/// Тот же скрипт лежит в файле warehouse.sql (его можно открыть в DB Browser for SQLite).
/// </summary>
public static class DbInitializer
{
    public static void Initialize()
    {
        using var db = new WarehouseContext();

        db.Database.ExecuteSqlRaw(@"
            CREATE TABLE IF NOT EXISTS Categories (
                Id   INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL
            );");

        db.Database.ExecuteSqlRaw(@"
            CREATE TABLE IF NOT EXISTS Products (
                Id         INTEGER PRIMARY KEY AUTOINCREMENT,
                Name       TEXT NOT NULL,
                Price      REAL NOT NULL,
                Quantity   INTEGER NOT NULL,
                CategoryId INTEGER NOT NULL,
                FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
            );");

        // Заполняем только пустую БД, чтобы данные не дублировались при каждом запуске
        if (db.Categories.Any()) return;

        db.Database.ExecuteSqlRaw(@"
            INSERT INTO Categories (Id, Name) VALUES
                (1, 'Электроника'),
                (2, 'Бытовая техника'),
                (3, 'Канцелярия');");

        db.Database.ExecuteSqlRaw(@"
            INSERT INTO Products (Name, Price, Quantity, CategoryId) VALUES
                ('Ноутбук Lenovo IdeaPad', 289990.0,   5, 1),
                ('Смартфон Samsung A15',    99990.0,  12, 1),
                ('Наушники JBL Tune',       24990.0,  20, 1),
                ('Чайник Philips',          14990.0,  15, 2),
                ('Пылесос Xiaomi',          59990.0,   7, 2),
                ('Микроволновая печь LG',   42990.0,   4, 2),
                ('Бумага А4 (500 листов)',   2490.0, 100, 3),
                ('Ручка шариковая',          150.0, 300, 3);");
    }
}
