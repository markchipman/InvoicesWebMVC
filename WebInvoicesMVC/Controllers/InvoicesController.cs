using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Invoices.Entities;
using WebInvoicesMVC.DataContexts;
using WebInvoicesMVC.Models;

namespace WebInvoicesMVC.Controllers
{
  public class InvoicesController : Controller
  {
    private InvoicesDb db = new InvoicesDb();

    // GET: Invoices
    public ActionResult Index(string searchString)
    {
      IQueryable<Invoice> invoices;
      invoices = db.Invoices.Include(i => i.Client);
      if (!string.IsNullOrEmpty(searchString))
      {
        invoices = db.Invoices
          .Where(inv => inv.Name.Contains(searchString))
          .Include(i => i.Client);
      }

      return View(invoices.ToList());
    }

    // GET: Invoices/Details/5
    public ActionResult Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Invoice invoice = db.Invoices.Find(id);
      if (invoice == null)
      {
        return HttpNotFound();
      }
      return View(invoice);
    }

    // GET: Invoices/Create
    public ActionResult Create(int? id)
    {
      ViewBag.ClientId = new SelectList(db.Clients, "Id", "Name");
      ViewBag.Products = new SelectList(db.Products, "Id", "ProductName");
      if (id != null)
      {
        Invoice invoice = db.Invoices.Single(i => i.Id.Equals(id.Value));
        List<Product> products = invoice.InvoiceProducts.Select(ip => ip.Product).ToList();

        InvoiceCreateViewModel invoiceCreateViewModel = new InvoiceCreateViewModel
        {
          ClientId = invoice.ClientId,
          Currency = invoice.Currency,
          InvoiceId = invoice.Id,
          Name = invoice.Name,
          Products = products
        };

        return View(invoiceCreateViewModel);
      }

      return View();
    }

    // POST: Invoices/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(string submitButton, [Bind(Include = "Id,Name,Currency,ClientId,ProductId,Price,Quantity, Products")] InvoiceCreateViewModel invoiceViewModel)
    {
      if (submitButton == "Create")
      {
        if (ModelState.IsValid)
        {
          AddInvoiceProduct(invoiceViewModel);
          return RedirectToAction("Index");
        }
      }
           
      ViewBag.Clients = new SelectList(db.Clients, "Id", "Name", invoiceViewModel.ClientId);
      ViewBag.Products = new SelectList(db.Products, "Id", "ProductName", invoiceViewModel.ProductId);
      return View(invoiceViewModel);
    }

    private void UpdateInvoiceProduct(InvoiceCreateViewModel invoiceViewModel, Invoice invoice)
    {
      db.InvoiceProducts.Add(new InvoiceProduct
      {
        InvoiceId = invoice.Id,
        ProductId = invoiceViewModel.ProductId,
        Quantity = invoiceViewModel.Quantity,
        Invoice = invoice,
        Product = db.Products.SingleOrDefault(p => p.Id.Equals(invoiceViewModel.ProductId))
      });

      invoice.Name = invoiceViewModel.Name;
      invoice.ClientId = invoiceViewModel.ClientId;
      invoice.Currency = invoiceViewModel.Currency;      

      db.Entry(invoice).State = EntityState.Modified;

      db.SaveChanges();
    }

    private Invoice AddInvoiceProduct(InvoiceCreateViewModel invoiceViewModel)
    {
      Invoice invoice = new Invoice
      {
        ClientId = invoiceViewModel.ClientId,
        Currency = invoiceViewModel.Currency,
        Name = invoiceViewModel.Name
      };

      db.Invoices.Add(invoice);

      db.InvoiceProducts.Add(new InvoiceProduct
      {
        InvoiceId = invoiceViewModel.InvoiceId,
        ProductId = invoiceViewModel.ProductId,
        Quantity = invoiceViewModel.Quantity,
        Invoice = invoice,
        Product = db.Products.SingleOrDefault(p => p.Id.Equals(invoiceViewModel.ProductId))
      });
            
      db.SaveChanges();
      return invoice;
    }

    public ActionResult LoadProducts(int invoiceId)
    {
      List<Product> products = null;
      if (invoiceId != 0)
      {
        products = db.InvoiceProducts
          .Where(ip => ip.InvoiceId == invoiceId)
          .Select(ip => ip.Product).ToList();
      }
      
      return PartialView("_ProductsPartial", products);
    }

    [HttpPost]
    public PartialViewResult AddProduct(InvoiceCreateViewModel invoiceCreateViewModel)
    {
      Invoice invoice = db.Invoices
        .Include(i => i.InvoiceProducts)
        .Include(i => i.InvoiceProducts.Select(ip => ip.Product))
        .SingleOrDefault(i => i.Name.Equals(invoiceCreateViewModel.Name));

      if (invoice == null)
      {
        invoice = AddInvoiceProduct(invoiceCreateViewModel);
      }
      else
      {
        UpdateInvoiceProduct(invoiceCreateViewModel, invoice);
      }

      List<Product> products = invoice.InvoiceProducts
        .Select(ip => ip.Product).ToList();

      return PartialView("_ProductsPartial", products);
    }

    // GET: Invoices/Edit/5
    public ActionResult Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Invoice invoice = db.Invoices.Find(id);
      if (invoice == null)
      {
        return HttpNotFound();
      }
      ViewBag.ClientId = new SelectList(db.Clients, "Id", "Name", invoice.ClientId);
      return View(invoice);
    }

    // POST: Invoices/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,Name,Currency,ClientId")] Invoice invoice)
    {
      if (ModelState.IsValid)
      {
        db.Entry(invoice).State = EntityState.Modified;
        db.SaveChanges();
        return RedirectToAction("Index");
      }
      ViewBag.ClientId = new SelectList(db.Clients, "Id", "Name", invoice.ClientId);
      return View(invoice);
    }

    // GET: Invoices/Delete/5
    public ActionResult Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Invoice invoice = db.Invoices.Find(id);
      if (invoice == null)
      {
        return HttpNotFound();
      }
      return View(invoice);
    }

    // POST: Invoices/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(int id)
    {
      Invoice invoice = db.Invoices.Find(id);
      db.Invoices.Remove(invoice);
      db.SaveChanges();
      return RedirectToAction("Index");
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        db.Dispose();
      }
      base.Dispose(disposing);
    }
  }
}
