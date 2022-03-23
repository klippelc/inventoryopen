namespace Inventory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIconCSS2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AssetTypes", "IconCss", c => c.String(maxLength: 50, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AssetTypes", "IconCss");
        }
    }
}
