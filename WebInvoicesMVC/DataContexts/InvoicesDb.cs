using System.Data.Entity;
using Invoices.Entities;

namespace WebInvoicesMVC.DataContexts
{
  public class InvoicesDb : DbContext
  {
    public InvoicesDb()
    : base("DefaultConnection")
    {      
    }

    public DbSet<Invoice> Invoices { get; set; }

    public DbSet<Client> Clients { get; set; }

    public DbSet<ClientInvoice> ClientInvoices { get; set; }
  }
}