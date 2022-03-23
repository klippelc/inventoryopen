namespace Inventory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMananger : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "ManagerOUPath", c => c.String(maxLength: 500, unicode: false));
            AddColumn("dbo.Users", "ManagerId", c => c.Int());
            CreateIndex("dbo.Users", "ManagerId");
            AddForeignKey("dbo.Users", "ManagerId", "dbo.Users", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "ManagerId", "dbo.Users");
            DropIndex("dbo.Users", new[] { "ManagerId" });
            DropColumn("dbo.Users", "ManagerId");
            DropColumn("dbo.Users", "ManagerOUPath");
        }
    }
}
