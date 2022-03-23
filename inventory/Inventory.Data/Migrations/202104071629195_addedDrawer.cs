namespace Inventory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedDrawer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assets", "Drawer", c => c.String(maxLength: 150, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Assets", "Drawer");
        }
    }
}
