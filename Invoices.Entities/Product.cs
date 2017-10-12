using System.Collections.Generic;

namespace Invoices.Entities
{
  public class Product
  {
    public int Id { get; set; }

    public string ProductName { get; set; }

    public decimal Price { get; set; }

    public List<InvoiceProduct> InvoiceProducts { get; set; }    
  }
}
