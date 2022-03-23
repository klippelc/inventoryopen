namespace Inventory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedLastLoginDatesColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "InventoryLastLoginDate", c => c.DateTime());
            AddColumn("dbo.Users", "ADLastLogonDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "ADLastLogonDate");
            DropColumn("dbo.Users", "InventoryLastLoginDate");
        }
    }
}
