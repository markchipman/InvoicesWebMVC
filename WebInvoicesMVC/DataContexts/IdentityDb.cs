using Microsoft.AspNet.Identity.EntityFramework;
using WebInvoicesMVC.Models;

namespace WebInvoicesMVC.DataContexts
{
  public class IdentityDb : IdentityDbContext<ApplicationUser>
  {
    public IdentityDb()
      : base("DefaultConnection", throwIfV1Schema: false)
    {
    }

    public static IdentityDb Create()
    {
      return new IdentityDb();
    }
  }
}