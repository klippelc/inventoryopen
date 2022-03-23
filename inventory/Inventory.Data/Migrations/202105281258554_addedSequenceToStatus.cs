namespace Inventory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedSequenceToStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AssetStatus", "Sequence", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AssetStatus", "Sequence");
        }
    }
}
