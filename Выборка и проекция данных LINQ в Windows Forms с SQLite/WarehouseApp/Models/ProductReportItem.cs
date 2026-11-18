namespace WarehouseApp.Models;

/// <summary>
/// DTO-класс проекции: только те поля, которые нужны отчету.
/// Именно он попадает в DataGridView, а не сущности Product/Category.
/// </summary>
public class ProductReportItem
{
    public string ProductName { get; set; } = "";   // Название товара
    public string CategoryName { get; set; } = "";  // Название связанной категории
    public decimal Price { get; set; }              // Цена за единицу
    public int Quantity { get; set; }               // Остаток на складе
    public decimal TotalValue { get; set; }         // Вычисляемое поле: Price * Quantity
}
