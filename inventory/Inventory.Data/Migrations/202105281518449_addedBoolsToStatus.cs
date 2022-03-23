namespace Inventory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedBoolsToStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AssetStatus", "UserLicenseType", c => c.Boolean());
            AddColumn("dbo.AssetStatus", "AssetLicenseType", c => c.Boolean());
            AddColumn("dbo.AssetStatus", "SoftwareType", c => c.Boolean());
            AddColumn("dbo.AssetStatus", "HardwareType", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AssetStatus", "HardwareType");
            DropColumn("dbo.AssetStatus", "SoftwareType");
            DropColumn("dbo.AssetStatus", "AssetLicenseType");
            DropColumn("dbo.AssetStatus", "UserLicenseType");
        }
    }
}
