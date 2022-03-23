namespace Inventory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addAssetStatusTypeTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssetStatusTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AssetStatusId = c.Int(),
                        AssetTypeId = c.Int(),
                        LicenseTypeId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssetStatus", t => t.AssetStatusId)
                .ForeignKey("dbo.AssetTypes", t => t.AssetTypeId)
                .ForeignKey("dbo.LicenseTypes", t => t.LicenseTypeId)
                .Index(t => t.AssetStatusId)
                .Index(t => t.AssetTypeId)
                .Index(t => t.LicenseTypeId);
            
            DropColumn("dbo.AssetStatus", "UserLicenseType");
            DropColumn("dbo.AssetStatus", "AssetLicenseType");
            DropColumn("dbo.AssetStatus", "SoftwareType");
            DropColumn("dbo.AssetStatus", "HardwareType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AssetStatus", "HardwareType", c => c.Boolean());
            AddColumn("dbo.AssetStatus", "SoftwareType", c => c.Boolean());
            AddColumn("dbo.AssetStatus", "AssetLicenseType", c => c.Boolean());
            AddColumn("dbo.AssetStatus", "UserLicenseType", c => c.Boolean());
            DropForeignKey("dbo.AssetStatusTypes", "LicenseTypeId", "dbo.LicenseTypes");
            DropForeignKey("dbo.AssetStatusTypes", "AssetTypeId", "dbo.AssetTypes");
            DropForeignKey("dbo.AssetStatusTypes", "AssetStatusId", "dbo.AssetStatus");
            DropIndex("dbo.AssetStatusTypes", new[] { "LicenseTypeId" });
            DropIndex("dbo.AssetStatusTypes", new[] { "AssetTypeId" });
            DropIndex("dbo.AssetStatusTypes", new[] { "AssetStatusId" });
            DropTable("dbo.AssetStatusTypes");
        }
    }
}
