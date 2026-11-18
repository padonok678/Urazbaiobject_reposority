namespace EnterpriseLinq.Models;

public class Sale
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public string CustomerName { get; private set; }
    public int Quantity { get; private set; }
    public DateTime SaleDate { get; private set; }
    public string Channel { get; private set; }

    public Sale(int id, int productId, string customerName, int quantity, DateTime saleDate, string channel)
    {
        if (string.IsNullOrWhiteSpace(customerName)) throw new ArgumentException("Укажите имя клиента.");
        if (quantity <= 0) throw new ArgumentException("Количество должно быть больше нуля.");

        Id = id;
        ProductId = productId;
        CustomerName = customerName.Trim();
        Quantity = quantity;
        SaleDate = saleDate;
        Channel = channel;
    }
}
