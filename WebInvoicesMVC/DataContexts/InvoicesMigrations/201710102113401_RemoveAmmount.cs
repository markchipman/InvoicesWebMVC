namespace WebInvoicesMVC.DataContexts.InvoicesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveAmmount : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Invoices", "Ammount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Invoices", "Ammount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
