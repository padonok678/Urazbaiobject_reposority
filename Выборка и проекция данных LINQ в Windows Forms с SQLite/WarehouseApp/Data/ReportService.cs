using WarehouseApp.Models;

namespace WarehouseApp.Data;

public static class ReportService
{
    /// <summary>
    /// Задания 2.1-2.3: JOIN Products + Categories, фильтр по категории,
    /// поиск по названию и проекция (Select) в ProductReportItem.
    /// </summary>
    /// <param name="selectedCategoryId">0 = «Все категории»</param>
    /// <param name="searchText">Подстрока названия товара (может быть пустой)</param>
    public static List<ProductReportItem> GetProductReport(int selectedCategoryId, string searchText)
    {
        using var dbContext = new WarehouseContext();

        // Запрос выполняется на стороне SQLite: JOIN, WHERE и вычисление
        // TotalValue превращаются в один SQL-запрос, из БД приходят только 5 нужных колонок.
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

        // Текстовый поиск по названию (Where + Contains).
        // Делаем его по уже спроецированным данным: SQLite сравнивает кириллицу с учетом регистра,
        // а Contains с OrdinalIgnoreCase позволяет искать «ноут» и «Ноут» одинаково.
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            string text = searchText.Trim();
            result = result
                .Where(p => p.ProductName.Contains(text, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return result;
    }

    /// <summary>Список категорий для ComboBox (первый пункт - «Все категории» с Id = 0).</summary>
    public static List<Category> GetCategories()
    {
        using var dbContext = new WarehouseContext();

        var list = dbContext.Categories
            .OrderBy(c => c.Name)
            .Select(c => new Category { Id = c.Id, Name = c.Name })
            .ToList();

        list.Insert(0, new Category { Id = 0, Name = "Все категории" });
        return list;
    }
}
