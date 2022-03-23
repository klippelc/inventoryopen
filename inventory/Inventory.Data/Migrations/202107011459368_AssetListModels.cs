namespace Inventory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AssetListModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssetUserLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AssetListId = c.Int(),
                        AssetId = c.Int(),
                        UserId = c.Int(),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Assets", t => t.AssetId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .ForeignKey("dbo.AssetLists", t => t.AssetListId)
                .Index(t => t.AssetListId)
                .Index(t => t.AssetId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AssetLists",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 150, unicode: false),
                        Description = c.String(maxLength: 300, unicode: false),
                        Shared = c.Boolean(),
                        AssetTypeId = c.Int(),
                        UserId = c.Int(),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .ForeignKey("dbo.AssetTypes", t => t.AssetTypeId)
                .Index(t => t.AssetTypeId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssetUserLists", "AssetListId", "dbo.AssetLists");
            DropForeignKey("dbo.AssetLists", "AssetTypeId", "dbo.AssetTypes");
            DropForeignKey("dbo.AssetUserLists", "UserId", "dbo.Users");
            DropForeignKey("dbo.AssetLists", "UserId", "dbo.Users");
            DropForeignKey("dbo.AssetUserLists", "AssetId", "dbo.Assets");
            DropIndex("dbo.AssetLists", new[] { "UserId" });
            DropIndex("dbo.AssetLists", new[] { "AssetTypeId" });
            DropIndex("dbo.AssetUserLists", new[] { "UserId" });
            DropIndex("dbo.AssetUserLists", new[] { "AssetId" });
            DropIndex("dbo.AssetUserLists", new[] { "AssetListId" });
            DropTable("dbo.AssetLists");
            DropTable("dbo.AssetUserLists");
        }
    }
}
