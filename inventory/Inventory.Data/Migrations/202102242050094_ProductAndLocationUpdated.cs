namespace Inventory.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ProductAndLocationUpdated : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Locations", "SubnetAddress", c => c.String(maxLength: 50, unicode: false));
            AddColumn("dbo.Products", "Specifications", c => c.String(maxLength: 500, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "Specifications");
            DropColumn("dbo.Locations", "SubnetAddress");
        }
    }
}
