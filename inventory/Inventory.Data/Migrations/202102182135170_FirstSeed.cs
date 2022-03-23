namespace Inventory.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class FirstSeed : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Amenities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 150, unicode: false),
                        Sequence = c.Int(nullable: false),
                        TypeId = c.Int(),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AmenityTypes", t => t.TypeId)
                .Index(t => t.TypeId);
            
            CreateTable(
                "dbo.BuildingAmenities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BuildingId = c.Int(nullable: false),
                        AmenityId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Amenities", t => t.AmenityId, cascadeDelete: true)
                .ForeignKey("dbo.Buildings", t => t.BuildingId, cascadeDelete: true)
                .Index(t => t.BuildingId)
                .Index(t => t.AmenityId);
            
            CreateTable(
                "dbo.Buildings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PropertyId = c.String(maxLength: 8000, unicode: false),
                        Name = c.String(maxLength: 150, unicode: false),
                        DisplayName = c.String(maxLength: 300, unicode: false),
                        Code = c.String(maxLength: 10, unicode: false),
                        Description = c.String(maxLength: 300, unicode: false),
                        Phone = c.String(maxLength: 30, unicode: false),
                        LocationId = c.Int(),
                        Latitude = c.Decimal(precision: 12, scale: 7),
                        Longitude = c.Decimal(precision: 12, scale: 7),
                        Geography = c.Geography(),
                        AddressLine1 = c.String(maxLength: 255, unicode: false),
                        AddressLine2 = c.String(maxLength: 255, unicode: false),
                        CityId = c.Int(),
                        StateId = c.Int(),
                        PostalCode = c.String(maxLength: 20, unicode: false),
                        Active = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Notes = c.String(maxLength: 500, unicode: false),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Locations", t => t.LocationId)
                .ForeignKey("dbo.Cities", t => t.CityId)
                .ForeignKey("dbo.States", t => t.StateId)
                .Index(t => t.LocationId)
                .Index(t => t.CityId)
                .Index(t => t.StateId);
            
            CreateTable(
                "dbo.Assets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InvoiceItemId = c.Int(),
                        AssetTag = c.String(maxLength: 8000, unicode: false),
                        Name = c.String(maxLength: 150, unicode: false),
                        Description = c.String(maxLength: 300, unicode: false),
                        Serial = c.String(maxLength: 150, unicode: false),
                        LicenseKeyMulti = c.String(maxLength: 150, unicode: false),
                        MacAddress = c.String(maxLength: 255, unicode: false),
                        StatusId = c.Int(),
                        DateReceived = c.DateTime(),
                        LocationId = c.Int(),
                        BuildingId = c.Int(),
                        RoomId = c.Int(),
                        AssignedAssetId = c.Int(),
                        AssignedUserId = c.Int(),
                        Notes = c.String(maxLength: 500, unicode: false),
                        Display = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Assets", t => t.AssignedAssetId)
                .ForeignKey("dbo.Users", t => t.AssignedUserId)
                .ForeignKey("dbo.Locations", t => t.LocationId)
                .ForeignKey("dbo.InvoiceItems", t => t.InvoiceItemId)
                .ForeignKey("dbo.Buildings", t => t.BuildingId)
                .ForeignKey("dbo.Rooms", t => t.RoomId)
                .ForeignKey("dbo.AssetStatus", t => t.StatusId)
                .Index(t => t.InvoiceItemId)
                .Index(t => t.StatusId)
                .Index(t => t.LocationId)
                .Index(t => t.BuildingId)
                .Index(t => t.RoomId)
                .Index(t => t.AssignedAssetId)
                .Index(t => t.AssignedUserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmployeeId = c.String(maxLength: 50, unicode: false),
                        Email = c.String(maxLength: 500, unicode: false),
                        Phone = c.String(maxLength: 20, unicode: false),
                        MobilePhone = c.String(maxLength: 20, unicode: false),
                        Title = c.String(maxLength: 50, unicode: false),
                        UserName = c.String(maxLength: 50, unicode: false),
                        FirstName = c.String(maxLength: 50, unicode: false),
                        LastName = c.String(maxLength: 50, unicode: false),
                        Division = c.String(maxLength: 50, unicode: false),
                        Park = c.String(maxLength: 50, unicode: false),
                        Active = c.Boolean(nullable: false),
                        AdminCreated = c.Boolean(nullable: false),
                        Notes = c.String(maxLength: 500, unicode: false),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PropertyId = c.String(maxLength: 8000, unicode: false),
                        Name = c.String(maxLength: 300, unicode: false),
                        DisplayName = c.String(maxLength: 300, unicode: false),
                        Code = c.String(maxLength: 10, unicode: false),
                        Description = c.String(maxLength: 300, unicode: false),
                        Phone = c.String(maxLength: 30, unicode: false),
                        Latitude = c.Decimal(precision: 12, scale: 7),
                        Longitude = c.Decimal(precision: 12, scale: 7),
                        Geography = c.Geography(),
                        AddressLine1 = c.String(maxLength: 255, unicode: false),
                        AddressLine2 = c.String(maxLength: 255, unicode: false),
                        CityId = c.Int(),
                        StateId = c.Int(),
                        PostalCode = c.String(maxLength: 20, unicode: false),
                        LeadManagerId = c.Int(),
                        Active = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Notes = c.String(maxLength: 500, unicode: false),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cities", t => t.CityId)
                .ForeignKey("dbo.States", t => t.StateId)
                .ForeignKey("dbo.Users", t => t.LeadManagerId)
                .Index(t => t.CityId)
                .Index(t => t.StateId)
                .Index(t => t.LeadManagerId);
            
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 150, unicode: false),
                        StateId = c.Int(),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.States", t => t.StateId)
                .Index(t => t.StateId);
            
            CreateTable(
                "dbo.States",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 10, unicode: false),
                        Name = c.String(maxLength: 150, unicode: false),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Suppliers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 150, unicode: false),
                        DisplayName = c.String(maxLength: 150, unicode: false),
                        Description = c.String(maxLength: 300, unicode: false),
                        Url = c.String(maxLength: 255, unicode: false),
                        Phone = c.String(maxLength: 20, unicode: false),
                        Email = c.String(maxLength: 150, unicode: false),
                        ContactName = c.String(maxLength: 150, unicode: false),
                        AddressLine1 = c.String(maxLength: 255, unicode: false),
                        AddressLine2 = c.String(maxLength: 255, unicode: false),
                        City = c.String(maxLength: 100, unicode: false),
                        StateId = c.Int(),
                        PostalCode = c.String(maxLength: 20, unicode: false),
                        Notes = c.String(maxLength: 500, unicode: false),
                        Active = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.States", t => t.StateId)
                .Index(t => t.StateId);
            
            CreateTable(
                "dbo.Invoices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PONumber = c.String(maxLength: 50, unicode: false),
                        SupplierId = c.Int(),
                        PurchaseDate = c.DateTime(),
                        TotalPrice = c.Decimal(precision: 18, scale: 2),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Suppliers", t => t.SupplierId)
                .Index(t => t.SupplierId);
            
            CreateTable(
                "dbo.InvoiceItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InvoiceId = c.Int(),
                        InvoiceItemNumber = c.Int(),
                        AssetTypeId = c.Int(),
                        AssetCategoryId = c.Int(),
                        ManuId = c.Int(),
                        ProductId = c.Int(),
                        LicenseTypeId = c.Int(),
                        LicenseKeySingle = c.String(maxLength: 8000, unicode: false),
                        UnitPrice = c.Decimal(precision: 18, scale: 2),
                        ExpirationDate = c.DateTime(),
                        Notes = c.String(maxLength: 500, unicode: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssetTypes", t => t.AssetTypeId)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .ForeignKey("dbo.Manufacturers", t => t.ManuId)
                .ForeignKey("dbo.AssetCategories", t => t.AssetCategoryId)
                .ForeignKey("dbo.Invoices", t => t.InvoiceId)
                .ForeignKey("dbo.LicenseTypes", t => t.LicenseTypeId)
                .Index(t => t.InvoiceId)
                .Index(t => t.AssetTypeId)
                .Index(t => t.AssetCategoryId)
                .Index(t => t.ManuId)
                .Index(t => t.ProductId)
                .Index(t => t.LicenseTypeId);
            
            CreateTable(
                "dbo.AssetCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 150, unicode: false),
                        Description = c.String(maxLength: 300, unicode: false),
                        AssetTypeId = c.Int(),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssetTypes", t => t.AssetTypeId)
                .Index(t => t.AssetTypeId);
            
            CreateTable(
                "dbo.AssetTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 150, unicode: false),
                        Description = c.String(maxLength: 300, unicode: false),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 150, unicode: false),
                        DisplayName = c.String(maxLength: 150, unicode: false),
                        Description = c.String(maxLength: 300, unicode: false),
                        ManuId = c.Int(),
                        AssetTypeId = c.Int(),
                        AssetCategoryId = c.Int(),
                        Active = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Notes = c.String(maxLength: 500, unicode: false),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssetCategories", t => t.AssetCategoryId)
                .ForeignKey("dbo.AssetTypes", t => t.AssetTypeId)
                .ForeignKey("dbo.Manufacturers", t => t.ManuId)
                .Index(t => t.ManuId)
                .Index(t => t.AssetTypeId)
                .Index(t => t.AssetCategoryId);
            
            CreateTable(
                "dbo.Manufacturers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 150, unicode: false),
                        DisplayName = c.String(maxLength: 150, unicode: false),
                        Description = c.String(maxLength: 300, unicode: false),
                        SupportUrl = c.String(maxLength: 150, unicode: false),
                        SupportPhone = c.String(maxLength: 30, unicode: false),
                        SupportEmail = c.String(maxLength: 200, unicode: false),
                        Notes = c.String(maxLength: 500, unicode: false),
                        Active = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LicenseTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 150, unicode: false),
                        Sequence = c.Int(),
                        Description = c.String(maxLength: 300, unicode: false),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LocationAmenities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LocationId = c.Int(nullable: false),
                        AmenityId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Amenities", t => t.AmenityId, cascadeDelete: true)
                .ForeignKey("dbo.Locations", t => t.LocationId, cascadeDelete: true)
                .Index(t => t.LocationId)
                .Index(t => t.AmenityId);
            
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 150, unicode: false),
                        Description = c.String(maxLength: 300, unicode: false),
                        Active = c.Boolean(nullable: false),
                        Sequence = c.Int(nullable: false),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Rooms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PropertyId = c.String(maxLength: 8000, unicode: false),
                        Name = c.String(maxLength: 150, unicode: false),
                        DisplayName = c.String(maxLength: 300, unicode: false),
                        Description = c.String(maxLength: 300, unicode: false),
                        RoomTypeId = c.Int(),
                        BuildingId = c.Int(),
                        Latitude = c.Decimal(precision: 12, scale: 7),
                        Longitude = c.Decimal(precision: 12, scale: 7),
                        Geography = c.Geography(),
                        Capacity = c.Int(),
                        Active = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Notes = c.String(maxLength: 500, unicode: false),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Buildings", t => t.BuildingId)
                .ForeignKey("dbo.RoomTypes", t => t.RoomTypeId)
                .Index(t => t.RoomTypeId)
                .Index(t => t.BuildingId);
            
            CreateTable(
                "dbo.RoomAmenities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoomId = c.Int(nullable: false),
                        AmenityId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Amenities", t => t.AmenityId, cascadeDelete: true)
                .ForeignKey("dbo.Rooms", t => t.RoomId, cascadeDelete: true)
                .Index(t => t.RoomId)
                .Index(t => t.AmenityId);
            
            CreateTable(
                "dbo.RoomTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 150, unicode: false),
                        Sequence = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Notes = c.String(maxLength: 500, unicode: false),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AssetStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 150, unicode: false),
                        Description = c.String(maxLength: 300, unicode: false),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AmenityTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 150, unicode: false),
                        Description = c.String(maxLength: 300, unicode: false),
                        Active = c.Boolean(nullable: false),
                        CreatedBy = c.Int(),
                        ModifiedBy = c.Int(),
                        DateAdded = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AuditLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PrimaryKeyId = c.Int(nullable: false),
                        TableName = c.String(maxLength: 255, unicode: false),
                        ColumnName = c.String(maxLength: 255, unicode: false),
                        OperationName = c.String(maxLength: 255, unicode: false),
                        OldValue = c.String(unicode: false),
                        NewValue = c.String(unicode: false),
                        ModifiedBy = c.Int(),
                        DateModified = c.DateTime(),
                        ModifiedByName = c.String(maxLength: 255, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Amenities", "TypeId", "dbo.AmenityTypes");
            DropForeignKey("dbo.BuildingAmenities", "BuildingId", "dbo.Buildings");
            DropForeignKey("dbo.Assets", "StatusId", "dbo.AssetStatus");
            DropForeignKey("dbo.Rooms", "RoomTypeId", "dbo.RoomTypes");
            DropForeignKey("dbo.RoomAmenities", "RoomId", "dbo.Rooms");
            DropForeignKey("dbo.RoomAmenities", "AmenityId", "dbo.Amenities");
            DropForeignKey("dbo.Rooms", "BuildingId", "dbo.Buildings");
            DropForeignKey("dbo.Assets", "RoomId", "dbo.Rooms");
            DropForeignKey("dbo.Assets", "BuildingId", "dbo.Buildings");
            DropForeignKey("dbo.UserRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.LocationAmenities", "LocationId", "dbo.Locations");
            DropForeignKey("dbo.LocationAmenities", "AmenityId", "dbo.Amenities");
            DropForeignKey("dbo.Locations", "LeadManagerId", "dbo.Users");
            DropForeignKey("dbo.Suppliers", "StateId", "dbo.States");
            DropForeignKey("dbo.Invoices", "SupplierId", "dbo.Suppliers");
            DropForeignKey("dbo.InvoiceItems", "LicenseTypeId", "dbo.LicenseTypes");
            DropForeignKey("dbo.InvoiceItems", "InvoiceId", "dbo.Invoices");
            DropForeignKey("dbo.Assets", "InvoiceItemId", "dbo.InvoiceItems");
            DropForeignKey("dbo.InvoiceItems", "AssetCategoryId", "dbo.AssetCategories");
            DropForeignKey("dbo.Products", "ManuId", "dbo.Manufacturers");
            DropForeignKey("dbo.InvoiceItems", "ManuId", "dbo.Manufacturers");
            DropForeignKey("dbo.InvoiceItems", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "AssetTypeId", "dbo.AssetTypes");
            DropForeignKey("dbo.Products", "AssetCategoryId", "dbo.AssetCategories");
            DropForeignKey("dbo.InvoiceItems", "AssetTypeId", "dbo.AssetTypes");
            DropForeignKey("dbo.AssetCategories", "AssetTypeId", "dbo.AssetTypes");
            DropForeignKey("dbo.Locations", "StateId", "dbo.States");
            DropForeignKey("dbo.Cities", "StateId", "dbo.States");
            DropForeignKey("dbo.Buildings", "StateId", "dbo.States");
            DropForeignKey("dbo.Locations", "CityId", "dbo.Cities");
            DropForeignKey("dbo.Buildings", "CityId", "dbo.Cities");
            DropForeignKey("dbo.Buildings", "LocationId", "dbo.Locations");
            DropForeignKey("dbo.Assets", "LocationId", "dbo.Locations");
            DropForeignKey("dbo.Assets", "AssignedUserId", "dbo.Users");
            DropForeignKey("dbo.Assets", "AssignedAssetId", "dbo.Assets");
            DropForeignKey("dbo.BuildingAmenities", "AmenityId", "dbo.Amenities");
            DropIndex("dbo.RoomAmenities", new[] { "AmenityId" });
            DropIndex("dbo.RoomAmenities", new[] { "RoomId" });
            DropIndex("dbo.Rooms", new[] { "BuildingId" });
            DropIndex("dbo.Rooms", new[] { "RoomTypeId" });
            DropIndex("dbo.UserRoles", new[] { "RoleId" });
            DropIndex("dbo.UserRoles", new[] { "UserId" });
            DropIndex("dbo.LocationAmenities", new[] { "AmenityId" });
            DropIndex("dbo.LocationAmenities", new[] { "LocationId" });
            DropIndex("dbo.Products", new[] { "AssetCategoryId" });
            DropIndex("dbo.Products", new[] { "AssetTypeId" });
            DropIndex("dbo.Products", new[] { "ManuId" });
            DropIndex("dbo.AssetCategories", new[] { "AssetTypeId" });
            DropIndex("dbo.InvoiceItems", new[] { "LicenseTypeId" });
            DropIndex("dbo.InvoiceItems", new[] { "ProductId" });
            DropIndex("dbo.InvoiceItems", new[] { "ManuId" });
            DropIndex("dbo.InvoiceItems", new[] { "AssetCategoryId" });
            DropIndex("dbo.InvoiceItems", new[] { "AssetTypeId" });
            DropIndex("dbo.InvoiceItems", new[] { "InvoiceId" });
            DropIndex("dbo.Invoices", new[] { "SupplierId" });
            DropIndex("dbo.Suppliers", new[] { "StateId" });
            DropIndex("dbo.Cities", new[] { "StateId" });
            DropIndex("dbo.Locations", new[] { "LeadManagerId" });
            DropIndex("dbo.Locations", new[] { "StateId" });
            DropIndex("dbo.Locations", new[] { "CityId" });
            DropIndex("dbo.Assets", new[] { "AssignedUserId" });
            DropIndex("dbo.Assets", new[] { "AssignedAssetId" });
            DropIndex("dbo.Assets", new[] { "RoomId" });
            DropIndex("dbo.Assets", new[] { "BuildingId" });
            DropIndex("dbo.Assets", new[] { "LocationId" });
            DropIndex("dbo.Assets", new[] { "StatusId" });
            DropIndex("dbo.Assets", new[] { "InvoiceItemId" });
            DropIndex("dbo.Buildings", new[] { "StateId" });
            DropIndex("dbo.Buildings", new[] { "CityId" });
            DropIndex("dbo.Buildings", new[] { "LocationId" });
            DropIndex("dbo.BuildingAmenities", new[] { "AmenityId" });
            DropIndex("dbo.BuildingAmenities", new[] { "BuildingId" });
            DropIndex("dbo.Amenities", new[] { "TypeId" });
            DropTable("dbo.AuditLogs");
            DropTable("dbo.AmenityTypes");
            DropTable("dbo.AssetStatus");
            DropTable("dbo.RoomTypes");
            DropTable("dbo.RoomAmenities");
            DropTable("dbo.Rooms");
            DropTable("dbo.Roles");
            DropTable("dbo.UserRoles");
            DropTable("dbo.LocationAmenities");
            DropTable("dbo.LicenseTypes");
            DropTable("dbo.Manufacturers");
            DropTable("dbo.Products");
            DropTable("dbo.AssetTypes");
            DropTable("dbo.AssetCategories");
            DropTable("dbo.InvoiceItems");
            DropTable("dbo.Invoices");
            DropTable("dbo.Suppliers");
            DropTable("dbo.States");
            DropTable("dbo.Cities");
            DropTable("dbo.Locations");
            DropTable("dbo.Users");
            DropTable("dbo.Assets");
            DropTable("dbo.Buildings");
            DropTable("dbo.BuildingAmenities");
            DropTable("dbo.Amenities");
        }
    }
}
