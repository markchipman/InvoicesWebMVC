using Invoices.Entities;

namespace WebInvoicesMVC.DataContexts.InvoicesMigrations
{
  using System.Data.Entity.Migrations;

  internal sealed class Configuration : DbMigrationsConfiguration<WebInvoicesMVC.DataContexts.InvoicesDb>
  {
    public Configuration()
    {
      AutomaticMigrationsEnabled = false;
      MigrationsDirectory = @"DataContexts\InvoicesMigrations";
      ContextKey = "WebInvoicesMVC.DataContexts.InvoicesDb";
    }

    protected override void Seed(WebInvoicesMVC.DataContexts.InvoicesDb context)
    {
      //  This method will be called after migrating to the latest version.

      //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
      //  to avoid creating duplicate seed data. E.g.

      context.Clients.AddOrUpdate(
        new Client
        {
          Id = 1,
          Name = "Client1"
        },
        new Client
        {
          Id = 2,
          Name = "Client2"
        },
        new Client
        {
          Id = 3,
          Name = "Client3"
        });

      context.Invoices.AddOrUpdate(
        new Invoice
        {
          Id = 1,
          Name = "Inv1",
          Currency = Currency.PLN,         
          ClientId = 1
        },
        new Invoice
        {
          Id = 2,
          Name = "Inv2",
          Currency = Currency.GBP,          
          ClientId = 2
        },
        new Invoice
        {
          Id = 3,
          Name = "Inv3",
          Currency = Currency.USD,          
          ClientId = 3
        },
        new Invoice
        {
          Id = 4,
          Name = "Inv4",
          Currency = Currency.USD,
          ClientId = 3
        }
      );

      context.Products.AddOrUpdate(
        new Product
        {
          Id = 1,
          ProductName = "Printer",
          Price = 200
        },
        new Product
        {
          Id = 2,
          ProductName = "Sink",
          Price = 444
        },
        new Product
        {
          Id = 3,
          ProductName = "TV",
          Price = 1000
        });

      context.InvoiceProducts.AddOrUpdate(
        new InvoiceProduct
        {
          Id = 1,
          InvoiceId = 1,
          ProductId = 1,
          Quantity = 1,
        },
        new InvoiceProduct
        {
          Id = 2,
          InvoiceId = 1,
          ProductId = 2,
          Quantity = 3
        },
        new InvoiceProduct
        {
          Id = 3,
          InvoiceId = 1,
          ProductId = 3,
          Quantity = 6
        },
        new InvoiceProduct
        {
          Id = 4,
          InvoiceId = 2,
          ProductId = 1,
          Quantity = 10
        },
        new InvoiceProduct
        {
          Id = 5,
          InvoiceId = 3,
          ProductId = 2,
          Quantity = 30,
        },
        new InvoiceProduct
        {
          Id = 6,
          InvoiceId = 4,
          ProductId = 1,
          Quantity = 1
        });

    }
  }
}
