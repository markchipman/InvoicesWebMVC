using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Invoices.Entities
{
  public class Invoice
  {
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    [Required]
    public Currency Currency { get; set; }

    public int ClientId { get; set; }

    //Navigation property
    public Client Client { get; set; }

    public List<InvoiceProduct> InvoiceProducts { get; set; }
  }
}
