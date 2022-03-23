namespace Inventory.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class connectedAssets : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assets", "ConnectedAssetId", c => c.Int());
            CreateIndex("dbo.Assets", "ConnectedAssetId");
            AddForeignKey("dbo.Assets", "ConnectedAssetId", "dbo.Assets", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Assets", "ConnectedAssetId", "dbo.Assets");
            DropIndex("dbo.Assets", new[] { "ConnectedAssetId" });
            DropColumn("dbo.Assets", "ConnectedAssetId");
        }
    }
}
