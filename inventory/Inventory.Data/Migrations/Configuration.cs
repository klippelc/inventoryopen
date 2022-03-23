namespace Inventory.Data.Migrations
{
    using Inventory.Data.Services;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<InventoryDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Inventory.Data.Services.InventoryDbContext";
        }

        protected override void Seed(InventoryDbContext context)
        {
            context.SaveChanges();
        }
    }
}