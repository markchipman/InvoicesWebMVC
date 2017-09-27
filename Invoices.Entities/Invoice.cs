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
    public decimal Ammount { get; set; }

    public Currency Currency { get; set; }

    //Navigation property
    public ClientInvoice ClientInvoice { get; set; }
  }
}
