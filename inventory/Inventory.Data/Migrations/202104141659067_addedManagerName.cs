namespace Inventory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedManagerName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "ManagerFirstName", c => c.String(maxLength: 50, unicode: false));
            AddColumn("dbo.Users", "ManagerLastName", c => c.String(maxLength: 50, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "ManagerLastName");
            DropColumn("dbo.Users", "ManagerFirstName");
        }
    }
}
