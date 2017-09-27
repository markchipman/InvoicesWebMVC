using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Invoices.Entities
{
  public enum Currency
  {
    [Display(Name="Złoty")]
    PLN,
    VND,
    GBP,
    USD
  }
}
