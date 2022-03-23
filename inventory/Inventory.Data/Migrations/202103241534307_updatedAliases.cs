namespace Inventory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedAliases : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.LocationAlias", "CreatedBy");
            DropColumn("dbo.LocationAlias", "ModifiedBy");
            DropColumn("dbo.LocationAlias", "DateAdded");
            DropColumn("dbo.LocationAlias", "DateModified");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LocationAlias", "DateModified", c => c.DateTime());
            AddColumn("dbo.LocationAlias", "DateAdded", c => c.DateTime());
            AddColumn("dbo.LocationAlias", "ModifiedBy", c => c.Int());
            AddColumn("dbo.LocationAlias", "CreatedBy", c => c.Int());
        }
    }
}
