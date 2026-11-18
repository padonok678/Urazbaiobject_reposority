using System.ComponentModel;

namespace EnterpriseLinq.Models;

public class SaleReportDto
{
    [DisplayName("Клиент")] public string CustomerName { get; }
    [DisplayName("Товар")] public string ProductName { get; }
    [DisplayName("Категория")] public string CategoryName { get; }
    [DisplayName("Канал")] public string Channel { get; }
    [DisplayName("Дата")] public DateTime SaleDate { get; }
    [DisplayName("Кол-во")] public int Quantity { get; }
    [DisplayName("Цена со скидкой")] public double DiscountedPrice { get; }
    [DisplayName("Сумма сделки")] public double TotalAmount { get; }

    public SaleReportDto(string customerName, string productName, string categoryName, string channel,
                         DateTime saleDate, int quantity, double discountedPrice)
    {
        CustomerName = customerName;
        ProductName = productName;
        CategoryName = categoryName;
        Channel = channel;
        SaleDate = saleDate;
        Quantity = quantity;
        DiscountedPrice = discountedPrice;
        TotalAmount = discountedPrice * quantity;
    }
}

public class CategoryStatDto
{
    [DisplayName("Категория")] public string CategoryName { get; }
    [DisplayName("Сделок")] public int DealsCount { get; }
    [DisplayName("Выручка")] public double Revenue { get; }
    [DisplayName("Средний чек")] public double AverageCheck { get; }
    [DisplayName("Мин. покупка")] public double MinDeal { get; }
    [DisplayName("Макс. покупка")] public double MaxDeal { get; }

    public CategoryStatDto(string categoryName, int dealsCount, double revenue, double averageCheck, double minDeal, double maxDeal)
    {
        CategoryName = categoryName;
        DealsCount = dealsCount;
        Revenue = revenue;
        AverageCheck = averageCheck;
        MinDeal = minDeal;
        MaxDeal = maxDeal;
    }
}

public record CategoryWithProducts(Category Category, List<Product> Products);
