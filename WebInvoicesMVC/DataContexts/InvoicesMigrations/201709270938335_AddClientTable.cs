namespace WebInvoicesMVC.DataContexts.InvoicesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddClientTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClientInvoices",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 255),
                        ClientInvoice_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClientInvoices", t => t.ClientInvoice_Id)
                .Index(t => t.ClientInvoice_Id);
            
            AddColumn("dbo.Invoices", "ClientInvoice_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Invoices", "ClientInvoice_Id");
            AddForeignKey("dbo.Invoices", "ClientInvoice_Id", "dbo.ClientInvoices", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Invoices", "ClientInvoice_Id", "dbo.ClientInvoices");
            DropForeignKey("dbo.Clients", "ClientInvoice_Id", "dbo.ClientInvoices");
            DropIndex("dbo.Invoices", new[] { "ClientInvoice_Id" });
            DropIndex("dbo.Clients", new[] { "ClientInvoice_Id" });
            DropColumn("dbo.Invoices", "ClientInvoice_Id");
            DropTable("dbo.Clients");
            DropTable("dbo.ClientInvoices");
        }
    }
}
