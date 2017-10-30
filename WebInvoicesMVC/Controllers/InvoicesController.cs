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

    // GET: Invoices/Create
    public ActionResult Create(int? id)
    {
      ViewBag.ClientId = new SelectList(db.Clients, "Id", "Name");
      ViewBag.Products = new SelectList(db.Products, "Id", "ProductName");
      if (id != null)
      {
        Invoice invoice = db.Invoices.Single(i => i.Id.Equals(id.Value));

        InvoiceCreateViewModel invoiceCreateViewModel = new InvoiceCreateViewModel
        {
          ClientId = invoice.ClientId,
          Currency = invoice.Currency,
          InvoiceId = invoice.Id,
          Name = invoice.Name
        };

        return View(invoiceCreateViewModel);
      }

      return View();
    }

    [HttpPost]
    public JsonResult GetPrice(string productId)
    {
      var product = db.Products
        .SingleOrDefault(p => p.Id.ToString().Equals(productId));

      return Json(product.Price);
    }

    // POST: Invoices/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create([Bind(Include = "Id,Name,Currency,ClientId,ProductId,Price,Quantity")] InvoiceCreateViewModel invoiceViewModel)
    {
      if (ModelState.IsValid)
      {
        AddOrUpdateInvoice(invoiceViewModel);
        return RedirectToAction("Index");
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

    [HttpPost]
    public int? UpdateInvoiceProduct(int invoiceProductId, int quantity)
    {
      var invoiceProduct = db.InvoiceProducts.FirstOrDefault(p => p.Id == invoiceProductId);
      if (invoiceProduct != null)
      {
        invoiceProduct.Quantity = quantity;
        db.Entry(invoiceProduct).State = EntityState.Modified;
        db.SaveChanges();
        return quantity;
      }

      return null;
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
      List<InvoiceProductViewModel> products = null;
      if (invoiceId != 0)
      {
        products = GetInvoiceProducts(invoiceId);
      }

      return PartialView("_ProductsPartial", products);
    }

    private List<InvoiceProductViewModel> GetInvoiceProducts(int invoiceId)
    {
      return (
        from p in db.Products
        from ip in p.InvoiceProducts
        where ip.InvoiceId.Equals(invoiceId)
        select new InvoiceProductViewModel
        {
          InvoiceProductId = ip.Id,
          Price = p.Price,
          ProductName = p.ProductName,
          Quantity = ip.Quantity,
          TotalAmmount = ip.Quantity * p.Price
        }).ToList();


      //return db.InvoiceProducts
      //  .Include(ip => ip.Product)
      //  .Where(ip => ip.InvoiceId == invoiceId)
      //  .SelectMany(
      //    ip => ip.Products,
      //    (ip, p) => new InvoiceProductViewModel
      //    {
      //      Price = p.Price,
      //      ProductName = p.ProductName,
      //      Quantity = ip.Quantity,
      //      TotalAmmount = ip.Quantity * p.Price
      //    })
      //  .ToList();
    }

    [HttpPost]
    public PartialViewResult AddProduct(InvoiceCreateViewModel invoiceCreateViewModel)
    {
      var invoice = AddOrUpdateInvoice(invoiceCreateViewModel);

      var products = GetInvoiceProducts(invoice.Id);
      return PartialView("_ProductsPartial", products);
    }

    private Invoice AddOrUpdateInvoice(InvoiceCreateViewModel invoiceCreateViewModel)
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
      return invoice;
    }

    // GET: Invoices/Edit/5
    public ActionResult Edit(int? id)
    {
      ViewBag.ClientId = new SelectList(db.Clients, "Id", "Name");
      ViewBag.Products = new SelectList(db.Products, "Id", "ProductName");
      var firstProduct = db.Products.FirstOrDefault();
      if (firstProduct != null)
      {
        ViewBag.FirstProductPrice = firstProduct.Price;
      }
      else
      {
        ViewBag.FirstProductPrice = 0;
      }
      

      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      Invoice invoice = db.Invoices.Find(id);
      if (invoice == null)
      {
        return HttpNotFound();
      }

      InvoiceCreateViewModel invoiceCreateViewModel = new InvoiceCreateViewModel
      {
        ClientId = invoice.ClientId,
        Currency = invoice.Currency,
        InvoiceId = invoice.Id,
        Name = invoice.Name,
        ProductId = 0
      };

      return View(invoiceCreateViewModel);
    }

    // POST: Invoices/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit([Bind(Include = "Id,Name,Currency,ClientId,ProductId,Price,Quantity")] InvoiceCreateViewModel invoiceViewModel)
    {
      if (ModelState.IsValid)
      {
        AddOrUpdateInvoice(invoiceViewModel);
        return RedirectToAction("Index");
      }
      ViewBag.FirstProductPrice = db.Products.FirstOrDefault();
      ViewBag.ClientId = new SelectList(db.Clients, "Id", "Name", invoiceViewModel.ClientId);
      ViewBag.Products = new SelectList(db.Products, "Id", "ProductName", invoiceViewModel.ProductId);
      return View(invoiceViewModel);
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
