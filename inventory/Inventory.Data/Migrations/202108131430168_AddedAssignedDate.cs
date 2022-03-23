namespace Inventory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedAssignedDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assets", "AssignedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Assets", "AssignedDate");
        }
    }
}
