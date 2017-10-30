namespace WebInvoicesMVC.Models
{
  public class InvoiceProductViewModel
  {
    public int InvoiceProductId { get; set; }

    public string ProductName { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public decimal TotalAmmount { get; set; }
  }
}