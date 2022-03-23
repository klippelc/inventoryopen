namespace Inventory.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ProductAndInvoiceItemsUpdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InvoiceItems", "Specifications", c => c.String(maxLength: 500, unicode: false));
            DropColumn("dbo.Products", "Specifications");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "Specifications", c => c.String(maxLength: 500, unicode: false));
            DropColumn("dbo.InvoiceItems", "Specifications");
        }
    }
}
