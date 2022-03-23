namespace Inventory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedNewLoginDates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assets", "LastLoginDate", c => c.DateTime());
            AddColumn("dbo.Assets", "LastBootDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Assets", "LastBootDate");
            DropColumn("dbo.Assets", "LastLoginDate");
        }
    }
}
