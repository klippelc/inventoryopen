namespace Inventory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedColorCSSToStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AssetStatus", "ColorCss", c => c.String(maxLength: 50, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AssetStatus", "ColorCss");
        }
    }
}
