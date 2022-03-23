namespace Inventory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedGroupToStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AssetStatus", "Group", c => c.String(maxLength: 150, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AssetStatus", "Group");
        }
    }
}
