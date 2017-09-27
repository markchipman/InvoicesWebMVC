using System.ComponentModel.DataAnnotations;
using System.Runtime;

namespace Invoices.Entities
{
  public class Client
  {
    [Required]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    //Navigation property
    public ClientInvoice ClientInvoice { get; set; }
  }
}
