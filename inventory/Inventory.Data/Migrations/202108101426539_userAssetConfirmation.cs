namespace Inventory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userAssetConfirmation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserAssetConfirmations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(),
                        AssetId = c.Int(),
                        DateConfirmed = c.DateTime(),
                        Notes = c.String(maxLength: 500, unicode: false),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Assets", t => t.AssetId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.AssetId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserAssetConfirmations", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserAssetConfirmations", "AssetId", "dbo.Assets");
            DropIndex("dbo.UserAssetConfirmations", new[] { "AssetId" });
            DropIndex("dbo.UserAssetConfirmations", new[] { "UserId" });
            DropTable("dbo.UserAssetConfirmations");
        }
    }
}
