# 1. Титульный лист

**[Название ВУЗа / колледжа]**

**Дисциплина:** Объектно-ориентированное программирование в C#
**Тема:** Выборка и проекция данных LINQ в Windows Forms с SQLite

**Выполнил:** [ФИО студента], группа [номер]
**Проверил:** [ФИО преподавателя]

Алматы, 2026

---

# 2. Цель и техническое задание

**Цель:** освоить составление LINQ-запросов к реляционной базе данных SQLite в приложении Windows Forms, научиться выполнять проекцию данных (оператор `Select`), объединять связанные таблицы и выводить результат в `DataGridView`.

**Техническое задание.** Разработать приложение «Складской учет товаров», которое:

1. использует базу данных `warehouse.db` (SQLite) с таблицами `Categories` и `Products` (не менее 3 категорий и 6 товаров);
2. выполняет LINQ-запрос с `join`, `where` и `select`, проецируя данные в DTO-класс `ProductReportItem`;
3. фильтрует отчет по выбранной категории и ищет товары по части названия;
4. показывает результат в `DataGridView` и считает общую стоимость запаса через `Sum()`;
5. автоматически пересчитывает запрос при смене категории или вводе текста поиска.

---

# 3. Схема БД и проекция

## Таблицы SQLite

**Categories**

| Столбец | Тип | Описание |
|---|---|---|
| Id | INTEGER PRIMARY KEY | Идентификатор категории |
| Name | TEXT NOT NULL | Название категории |

**Products**

| Столбец | Тип | Описание |
|---|---|---|
| Id | INTEGER PRIMARY KEY | Идентификатор товара |
| Name | TEXT NOT NULL | Наименование товара |
| Price | REAL NOT NULL | Цена единицы |
| Quantity | INTEGER NOT NULL | Количество на складе |
| CategoryId | INTEGER FK | Внешний ключ → Categories(Id) |

Связь: одна категория → много товаров (1:N).

## DTO-класс проекции `ProductReportItem`

| Поле | Тип | Описание |
|---|---|---|
| ProductName | string | Название товара |
| CategoryName | string | Название категории (из связанной таблицы) |
| Price | decimal | Цена за единицу |
| Quantity | int | Остаток на складе |
| TotalValue | decimal | Вычисляемое поле: Price * Quantity |

Класс не соответствует ни одной таблице: он собирает данные из двух таблиц и содержит вычисляемое поле.

---

# 4. Листинги C# с комментариями

Полный код находится в папке проекта. Ниже ключевые фрагменты.

## 4.1. DTO-класс (`Models/ProductReportItem.cs`)

```csharp
public class ProductReportItem
{
    public string ProductName { get; set; } = "";
    public string CategoryName { get; set; } = "";
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal TotalValue { get; set; }   // Price * Quantity
}
```

## 4.2. LINQ-запрос с проекцией (`Data/ReportService.cs`)

```csharp
var reportQuery =
    from p in dbContext.Products
    join c in dbContext.Categories on p.CategoryId equals c.Id
    where selectedCategoryId == 0 || c.Id == selectedCategoryId   // фильтр по категории
    select new ProductReportItem                                   // проекция (Select)
    {
        ProductName  = p.Name,
        CategoryName = c.Name,
        Price        = p.Price,
        Quantity     = p.Quantity,
        TotalValue   = p.Price * p.Quantity                        // вычисляемое поле
    };

var result = reportQuery.ToList();

// поиск по названию (без учета регистра, в том числе для кириллицы)
if (!string.IsNullOrWhiteSpace(searchText))
    result = result
        .Where(p => p.ProductName.Contains(searchText.Trim(), StringComparison.OrdinalIgnoreCase))
        .ToList();
```

Примечание: JOIN, фильтр по категории и вычисление `TotalValue` выполняет SQLite одним запросом. Поиск по тексту выполняется в памяти, так как SQLite сравнивает кириллицу с учетом регистра.

## 4.3. Обработчики WinForms (`FormMain.cs`)

```csharp
private void FormMain_Load(object sender, EventArgs e)
{
    cmbCategories.DataSource = ReportService.GetCategories();   // + «Все категории»
    cmbCategories.DisplayMember = nameof(Category.Name);
    cmbCategories.ValueMember = nameof(Category.Id);
    _loading = false;
    ApplyFilter();
}

// автоматический пересчет при любом изменении фильтра
private void cmbCategories_SelectedIndexChanged(object sender, EventArgs e) => ApplyFilter();
private void txtSearch_TextChanged(object sender, EventArgs e) => ApplyFilter();
private void btnFilter_Click(object sender, EventArgs e) => ApplyFilter();

private void ApplyFilter()
{
    if (_loading) return;
    int categoryId = cmbCategories.SelectedValue is int id ? id : 0;

    var report = ReportService.GetProductReport(categoryId, txtSearch.Text);
    dgvReport.DataSource = report;
    ConfigureColumns();

    decimal total = report.Sum(r => r.TotalValue);              // LINQ Sum()
    lblTotalSum.Text = $"Общая стоимость запаса: {total:N2} ₸";
}
```

---

# 5. Скриншоты GUI WinForms

> Вставьте после запуска приложения (F5 в Visual Studio):

1. Главное окно при запуске: категория «Все категории», все 8 товаров и общая сумма.
   *[скриншот 1]*
2. Фильтрация по категории «Электроника».
   *[скриншот 2]*
3. Текстовый поиск, например «ноут» (результат: Ноутбук Lenovo IdeaPad).
   *[скриншот 3]*
4. Комбинация фильтров: категория + текст.
   *[скриншот 4]*

Ожидаемые значения для проверки (Σ Price × Quantity) приведены в таблице ниже.

| Товар | Цена | Остаток | Стоимость запаса |
|---|---|---|---|
| Ноутбук Lenovo IdeaPad | 289 990 | 5 | 1 449 950 |
| Смартфон Samsung A15 | 99 990 | 12 | 1 199 880 |
| Наушники JBL Tune | 24 990 | 20 | 499 800 |
| Чайник Philips | 14 990 | 15 | 224 850 |
| Пылесос Xiaomi | 59 990 | 7 | 419 930 |
| Микроволновая печь LG | 42 990 | 4 | 171 960 |
| Бумага А4 (500 листов) | 2 490 | 100 | 249 000 |
| Ручка шариковая | 150 | 300 | 45 000 |

Итоги: Электроника = 3 149 630; Бытовая техника = 816 740; Канцелярия = 294 000; **всего = 4 260 370**.

---

# 6. Аналитический вывод

Проекция `Select` позволяет получать из базы только те данные, которые нужны для задачи, а не все поля сущностей. В этой работе из двух таблиц собирается компактная выборка из пяти полей, включая вычисляемое `TotalValue`. Преимущества перед загрузкой «сырых» данных:

- **Меньше данных** передается из БД в приложение: не загружаются лишние столбцы (`Id`, `CategoryId` и др.).
- **Вычисления и фильтрация выполняются в SQL**: JOIN, `WHERE` и `Price * Quantity` делает SQLite, а не приложение.
- **Удобная модель для интерфейса**: DTO содержит ровно те поля, что нужны `DataGridView`, без навигационных свойств.
- **Безопасность и чистота кода**: интерфейс не зависит от структуры таблиц, а изменение схемы БД затрагивает только запрос.
- **Производительность**: чем больше таблица и шире сущность, тем заметнее выигрыш.

---

# Ответы на контрольные вопросы

**1. Что такое проекция данных в LINQ и каким оператором она реализуется?**
Проекция — преобразование элементов выборки в другую форму (другой тип или набор полей). Реализуется оператором `Select` (в синтаксисе запросов — `select`).

**2. Разница между проекцией в анонимный тип и в именованный DTO-класс?**
Анонимный тип (`new { ... }`) создается на месте, у него нет имени, его нельзя вернуть из метода как конкретный тип и удобно использовать только локально. Именованный DTO-класс можно передавать между методами и слоями, использовать в сигнатурах (`List<ProductReportItem>`), документировать и переиспользовать.

**3. Главное преимущество проекции при работе с SQLite?**
Из БД читаются только нужные столбцы, а часть вычислений делается на стороне СУБД, поэтому уменьшается объем передаваемых данных и нагрузка на приложение.

**4. Как выполнить расчет вычисляемого поля?**
Прямо в инициализаторе проекции: `TotalValue = p.Price * p.Quantity`.

**5. Чем отличается выполнение LINQ в памяти от запроса к БД через ORM?**
LINQ to Objects выполняется над уже загруженной коллекцией, делегатами в памяти приложения. В LINQ к БД через ORM (EF Core) выражение переводится в SQL, выполняется в СУБД, а в приложение приходит только результат. Запрос выполняется при материализации (`ToList()`, `Sum()` и т. п.), а не при его объявлении.
