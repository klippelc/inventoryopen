namespace Inventory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoleDefinition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Roles", "Definition", c => c.String(maxLength: 500, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Roles", "Definition");
        }
    }
}
