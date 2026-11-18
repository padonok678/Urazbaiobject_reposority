CREATE TABLE IF NOT EXISTS Categories (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    DiscountRate REAL NOT NULL
);
CREATE TABLE IF NOT EXISTS Products (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    CategoryId INTEGER NOT NULL,
    Price REAL NOT NULL,
    Stock INTEGER NOT NULL,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);
CREATE TABLE IF NOT EXISTS Sales (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ProductId INTEGER NOT NULL,
    CustomerName TEXT NOT NULL,
    Quantity INTEGER NOT NULL,
    SaleDate TEXT NOT NULL,
    Channel TEXT NOT NULL,
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

INSERT INTO Categories (Name, DiscountRate) VALUES
 ('Электроника', 0.1),
 ('Одежда', 0.15),
 ('Продукты', 0.05),
 ('Книги', 0.2);

INSERT INTO Products (Name, CategoryId, Price, Stock) VALUES
 ('Смартфон Xiaomi 13', 1, 45000, 25),
 ('Наушники JBL Tune', 1, 6500, 60),
 ('Ноутбук Lenovo IdeaPad', 1, 72000, 12),
 ('Куртка зимняя', 2, 9800, 40),
 ('Кроссовки Nike Air', 2, 11500, 35),
 ('Футболка хлопок', 2, 1500, 120),
 ('Кофе зерновой 1 кг', 3, 1800, 200),
 ('Чай чёрный Greenfield', 3, 450, 300),
 ('Книга «Чистый код»', 4, 2200, 50),
 ('Книга «Библия C#»', 4, 3100, 30);

INSERT INTO Sales (ProductId, CustomerName, Quantity, SaleDate, Channel) VALUES
 (1, 'Иванов А.', 1, '2026-10-01', 'Online'),
 (1, 'Петрова М.', 2, '2026-10-03', 'Retail'),
 (2, 'Сидоров К.', 1, '2026-10-04', 'Online'),
 (2, 'Кузнецова О.', 3, '2026-10-05', 'Retail'),
 (3, 'Смирнов Д.', 1, '2026-10-06', 'Online'),
 (3, 'Волков Е.', 1, '2026-10-12', 'Online'),
 (4, 'Морозова Н.', 1, '2026-10-07', 'Retail'),
 (4, 'Лебедев Р.', 2, '2026-10-08', 'Online'),
 (5, 'Орлова Т.', 1, '2026-10-09', 'Retail'),
 (6, 'Фёдоров И.', 4, '2026-10-10', 'Retail'),
 (6, 'Захарова В.', 2, '2026-10-11', 'Retail'),
 (7, 'Николаев П.', 3, '2026-10-13', 'Online'),
 (7, 'Павлова С.', 5, '2026-10-14', 'Retail'),
 (8, 'Соколов Г.', 10, '2026-10-15', 'Retail'),
 (8, 'Егорова Л.', 6, '2026-10-16', 'Retail'),
 (9, 'Андреев Б.', 1, '2026-10-17', 'Online'),
 (9, 'Тихонова Ю.', 2, '2026-10-18', 'Retail'),
 (10, 'Белов Ф.', 1, '2026-10-19', 'Online'),
 (10, 'Комарова А.', 2, '2026-10-20', 'Online'),
 (1, 'Голубев Н.', 1, '2026-10-21', 'Online'),
 (2, 'Виноградова Е.', 2, '2026-10-22', 'Online'),
 (4, 'Ильин Ю.', 1, '2026-10-23', 'Online'),
 (7, 'Рыбаков Т.', 2, '2026-10-24', 'Online'),
 (5, 'Ширяев Ю.', 1, '2026-10-25', 'Online');
