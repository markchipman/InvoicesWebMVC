using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Invoices.Entities;

namespace WebInvoicesMVC.Models
{
  public class InvoiceCreateViewModel
  {
    public int InvoiceId { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    [Required]
    public Currency Currency { get; set; }

    public int ClientId { get; set; }    

    public int ProductId { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; } 

    public List<Product> Products { get; set; }
  }
}