namespace Inventory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIconCSS3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LicenseTypes", "IconCss", c => c.String(maxLength: 50, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LicenseTypes", "IconCss");
        }
    }
}
