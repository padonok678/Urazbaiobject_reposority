using EnterpriseLinq.Models;

namespace EnterpriseLinq.Services;

public class ReportService
{
    private readonly List<Category> _categories;
    private readonly List<Product> _products;
    private readonly List<Sale> _sales;

    public ReportService(List<Category> categories, List<Product> products, List<Sale> sales)
    {
        _categories = categories;
        _products = products;
        _sales = sales;
    }

    public IReadOnlyList<Category> Categories => _categories;
    public IReadOnlyList<Product> Products => _products;

    public Product? FindProduct(int id) => _products.FirstOrDefault(p => p.Id == id);

    public Category? FindCategory(int id) => _categories.FirstOrDefault(c => c.Id == id);

    // ---- 2.1 DTO: Join Sales + Products + Categories (отложенный запрос) ----
    public IEnumerable<SaleReportDto> BuildReportDeferred() =>
        _sales
            .Join(_products, s => s.ProductId, p => p.Id, (s, p) => new { Sale = s, Product = p })
            .Join(_categories, x => x.Product.CategoryId, c => c.Id, (x, c) => new SaleReportDto(
                x.Sale.CustomerName,
                x.Product.Name,
                c.Name,
                x.Sale.Channel,
                x.Sale.SaleDate,
                x.Sale.Quantity,
                x.Product.Price * (1 - c.DiscountRate)));

    // GroupJoin: категория и список её товаров (у категории без товаров список будет пустым)
    public List<CategoryWithProducts> GetCategoriesWithProducts() =>
        _categories
            .GroupJoin(_products, c => c.Id, p => p.CategoryId, (c, ps) => new CategoryWithProducts(c, ps.ToList()))
            .ToList();

    // ---- 2.2 Фильтрация, сортировка, пагинация ----
    public IEnumerable<SaleReportDto> Filter(string? categoryName, string? channel)
    {
        var query = BuildReportDeferred();

        if (!string.IsNullOrEmpty(categoryName))
            query = query.Where(s => s.CategoryName == categoryName);
        if (!string.IsNullOrEmpty(channel))
            query = query.Where(s => s.Channel == channel);

        return query.OrderBy(s => s.CategoryName).ThenByDescending(s => s.TotalAmount);
    }

    public List<SaleReportDto> GetPage(int pageNumber, int pageSize) =>
        Filter(null, null)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

    public int GetPageCount(int pageSize) =>
        Math.Max(1, (int)Math.Ceiling(BuildReportDeferred().Count() / (double)pageSize));

    // ---- 2.3 Группировка и агрегация ----
    public List<CategoryStatDto> GetCategoryStats() =>
        BuildReportDeferred()
            .GroupBy(s => s.CategoryName)
            .Select(g => new CategoryStatDto(
                g.Key,
                g.Count(),
                g.Sum(s => s.TotalAmount),
                g.Average(s => s.TotalAmount),
                g.Min(s => s.TotalAmount),
                g.Max(s => s.TotalAmount)))
            .OrderBy(s => s.CategoryName)
            .ToList();

    public string BuildAudit()
    {
        var stats = GetCategoryStats();
        if (stats.Count == 0) return "Продаж нет.";

        string audit = stats.Aggregate(
            "ФИНАНСОВЫЙ АУДИТ",
            (text, s) => text + $"\r\n • {s.CategoryName}: {s.DealsCount} сделок, выручка {s.Revenue:N0}, средний чек {s.AverageCheck:N0}");

        var best = stats.OrderByDescending(s => s.Revenue).First();
        return audit +
               $"\r\n\r\nИтого сделок: {stats.Sum(s => s.DealsCount)}" +
               $"\r\nОбщая выручка: {stats.Sum(s => s.Revenue):N0}" +
               $"\r\nЛучшая категория: {best.CategoryName} ({best.Revenue:N0})";
    }

    // ---- 2.4 Множества: товары по каналам продаж ----
    private IEnumerable<Product> ProductsSoldIn(string channel) =>
        _sales
            .Where(s => s.Channel == channel)
            .Join(_products, s => s.ProductId, p => p.Id, (s, p) => p);

    public List<Product> OnlineOnly() => ProductsSoldIn("Online").Except(ProductsSoldIn("Retail")).ToList();

    public List<Product> CommonProducts() => ProductsSoldIn("Online").Intersect(ProductsSoldIn("Retail")).ToList();

    public List<Product> AllSoldProducts() => ProductsSoldIn("Online").Union(ProductsSoldIn("Retail")).ToList();

    // ---- 2.5 Отложенное и немедленное выполнение ----
    public Sale AddSale(string customer, int productId, int quantity, string channel)
    {
        var product = FindProduct(productId)
            ?? throw new ArgumentException($"Товар с Id {productId} не найден.");

        int nextId = _sales.Count == 0 ? 1 : _sales.Max(s => s.Id) + 1;
        var sale = new Sale(nextId, product.Id, customer, quantity, DateTime.Today, channel);
        _sales.Add(sale);
        return sale;
    }

    public string DemoExecution()
    {
        // работаем с копией списка продаж, чтобы не портить данные приложения
        var sandbox = new ReportService(_categories, _products, _sales.ToList());

        IEnumerable<SaleReportDto> deferred = sandbox.BuildReportDeferred();
        List<SaleReportDto> snapshot = sandbox.BuildReportDeferred().ToList();
        int before = snapshot.Count;

        sandbox.AddSale("Демо Клиент", 1, 2, "Online");

        var found = new List<string>();
        foreach (var dto in deferred.Where(d => d.CustomerName == "Демо Клиент"))
            found.Add($"{dto.CustomerName} — {dto.ProductName}, {dto.TotalAmount:N0}");

        return "Отложенное и немедленное выполнение:\r\n" +
               $" • продаж до добавления: {before}\r\n" +
               $" • после добавления новой продажи IEnumerable при переборе даёт {deferred.Count()}, нашёл: {string.Join("; ", found)}\r\n" +
               $" • список ToList() остался прежним: {snapshot.Count}";
    }
}
