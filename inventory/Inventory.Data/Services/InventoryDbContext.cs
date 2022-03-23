using Inventory.Data.Common;
using Inventory.Data.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.Data.Services
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext() : base("dbConnection")
        {
        }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetCategory> AssetCategories { get; set; }
        public DbSet<AssetStatus> AssetStatuses { get; set; }
        public DbSet<AssetType> AssetTypes { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<LicenseType> LicenseTypes { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<AmenityType> AmenityTypes { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<RoomAmenity> RoomAmenities { get; set; }
        public DbSet<BuildingAmenity> BuildingAmenities { get; set; }
        public DbSet<LocationAmenity> LocationAmenities { get; set; }
        public DbSet<LocationAlias> LocationAliases { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<AssetList> AssetLists { get; set; }
        public DbSet<AssetUserList> AssetUserLists { get; set; }
        public DbSet<UserAssetConfirmation> UserAssetConfirmations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Sets Entity Framework to map every string property to a varchar instead of a nvarchar.
            modelBuilder.Properties()
           .Where(x =>
               x.PropertyType.FullName.Equals("System.String")
               && !x.GetCustomAttributes(false).OfType<ColumnAttribute>()
               .Any(q => q.TypeName != null && q.TypeName.Equals("varchar", StringComparison.InvariantCultureIgnoreCase)))
               .Configure(c => c.HasColumnType("varchar"));

            modelBuilder.Entity<Location>().Property(x => x.Latitude).HasPrecision(12, 7);
            modelBuilder.Entity<Location>().Property(x => x.Longitude).HasPrecision(12, 7);

            modelBuilder.Entity<Building>().Property(x => x.Latitude).HasPrecision(12, 7);
            modelBuilder.Entity<Building>().Property(x => x.Longitude).HasPrecision(12, 7);

            modelBuilder.Entity<Room>().Property(x => x.Latitude).HasPrecision(12, 7);
            modelBuilder.Entity<Room>().Property(x => x.Longitude).HasPrecision(12, 7);

            modelBuilder.Entity<Asset>()
                        .HasOptional(m => m.ConnectedAsset)
                        .WithMany(t => t.ConnectedAssets)
                        .HasForeignKey(m => m.ConnectedAssetId)
                        .WillCascadeOnDelete(false);
        }

        public override async Task<int> SaveChangesAsync()
        {
            AddTimestamps();
            return await base.SaveChangesAsync();
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries().Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((BaseEntity)entity.Entity).DateAdded = DateTime.Now;
                }

                ((BaseEntity)entity.Entity).DateModified = DateTime.Now; 
            }
        }
    }
}