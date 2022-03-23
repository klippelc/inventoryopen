namespace Inventory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIPAddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assets", "IPAddress", c => c.String(maxLength: 50, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Assets", "IPAddress");
        }
    }
}
