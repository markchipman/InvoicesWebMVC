using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Drawing.Printing;
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
    public ActionResult Create(string submitButton, [Bind(Include = "Id,Name,Currency,ClientId,ProductName,Price,Quantity, Products")] InvoiceCreateViewModel invoiceViewModel)
    {
      if (submitButton == "Create")
      {
        if (ModelState.IsValid)
        {
          AddInvoiceProduct(invoiceViewModel);
          return RedirectToAction("Index");
        }
      }
      else if (submitButton == "AddProduct")
      {
        //Find if invoice is added to db. If not then add:
        Invoice invoice = db.Invoices.SingleOrDefault(i => i.Name.Equals(invoiceViewModel.Name));
        if (invoice == null)
        {
          invoice = AddInvoiceProduct(invoiceViewModel);         
        }
        else
        {
          UpdateInvoiceProduct(invoiceViewModel, invoice);              
        }

        invoiceViewModel.Price = 0;
        invoiceViewModel.ProductName = String.Empty;
        invoiceViewModel.Quantity = 0;

        Invoice inv = db.Invoices
          .Single(i => i.Id == invoice.Id);

        IEnumerable<InvoiceProduct> invProd = db.InvoiceProducts
          .Where(ip => ip.InvoiceId.Equals(invoice.Id));

        //IEnumerable<Product> prod = from p in db.Products
        //  where invProd.Contains(p.Id)
        //  select p;


        IEnumerable<Product> query1 = invoice.InvoiceProducts
          .Select(ip => ip.Product);          

        invoiceViewModel.Products = query1.ToList();

      }
      
      ViewBag.ClientId = new SelectList(db.Clients, "Id", "Name", invoiceViewModel.ClientId);
      return View(invoiceViewModel);
    }

    private void UpdateInvoiceProduct(InvoiceCreateViewModel invoiceViewModel, Invoice invoice)
    {
      db.InvoiceProducts.Add(new InvoiceProduct
      {
        InvoiceId = invoice.Id,
        ProductId = db.Products.Single(p => p.ProductName.Equals(invoiceViewModel.ProductName)).Id,
        Quantity = invoiceViewModel.Quantity,
      });

      invoice.Name = invoiceViewModel.Name;
      invoice.ClientId = invoiceViewModel.ClientId;
      invoice.Currency = invoiceViewModel.Currency;      

      db.Entry(invoice).State = EntityState.Modified;

      db.SaveChanges();
    }

    private Invoice AddInvoiceProduct(InvoiceCreateViewModel invoiceViewModel)
    {
      db.InvoiceProducts.Add(new InvoiceProduct
      {
        InvoiceId = invoiceViewModel.InvoiceId,
        ProductId = db.Products.Single(p => p.ProductName.Equals(invoiceViewModel.ProductName)).Id,
        Quantity = invoiceViewModel.Quantity,
      });

      Invoice invoice = new Invoice
      {
        ClientId = invoiceViewModel.ClientId,
        Currency = invoiceViewModel.Currency,
        Name = invoiceViewModel.Name
      };

      db.Invoices.Add(invoice);
      db.SaveChanges();

      return invoice;
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
