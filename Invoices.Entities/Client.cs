using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Invoices.Entities
{
  public class Client
  {
    [Required]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    [DisplayName("Client Name")]
    public string Name { get; set; }

    public List<Invoice> Invoice { get; set; }
  }
}
