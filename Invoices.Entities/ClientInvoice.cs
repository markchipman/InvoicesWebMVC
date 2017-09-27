using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Invoices.Entities
{
  public class ClientInvoice
  {
    [Required]
    public string Id { get; set; }

    public IList<Client> Clients { get; set; }

    public IList<Invoice> Invoices { get; set; }
  }
}
