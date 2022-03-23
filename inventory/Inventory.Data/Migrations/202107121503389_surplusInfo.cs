namespace Inventory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class surplusInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assets", "SNFnumber", c => c.String(maxLength: 15, unicode: false));
            AddColumn("dbo.Assets", "SurplusDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Assets", "SurplusDate");
            DropColumn("dbo.Assets", "SNFnumber");
        }
    }
}
