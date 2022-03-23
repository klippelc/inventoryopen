namespace Inventory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LocationAlias : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LocationAlias",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LocationId = c.Int(),
                        Name = c.String(maxLength: 300, unicode: false),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Locations", t => t.LocationId)
                .Index(t => t.LocationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LocationAlias", "LocationId", "dbo.Locations");
            DropIndex("dbo.LocationAlias", new[] { "LocationId" });
            DropTable("dbo.LocationAlias");
        }
    }
}
