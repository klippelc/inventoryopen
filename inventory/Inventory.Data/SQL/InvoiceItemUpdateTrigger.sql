USE [Inventory]
GO

DROP TRIGGER IF EXISTS [dbo].[InvoiceItems_UTrig]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [InvoiceItems_UTrig]
ON [dbo].[InvoiceItems]
AFTER UPDATE
AS DECLARE @TableName VARCHAR(255),
		   @OperationName VARCHAR(255),
           @PrimaryKeyId INT,
		   @Invoice VARCHAR(255),
		   @InvoiceItemNumber INT,
		   @AssetType VARCHAR(255),
		   @AssetCategory VARCHAR(255),
           @Manu VARCHAR(255),
		   @Product VARCHAR(255),
		   @LicenseType VARCHAR(255),
		   @LicenseKey VARCHAR(255),
		   @UnitPrice DECIMAL (18, 2),
		   @ExpirationDate DateTime,
           @Specifications VARCHAR(500),
           @Notes VARCHAR(500),
		   @IsDeleted VARCHAR(255),
		   @del_PrimaryKeyId INT,
		   @del_Invoice VARCHAR(255),
		   @del_InvoiceItemNumber INT,
		   @del_AssetType VARCHAR(255),
		   @del_AssetCategory VARCHAR(255),
           @del_Manu VARCHAR(255),
		   @del_Product VARCHAR(255),
		   @del_LicenseType VARCHAR(255),
		   @del_LicenseKey VARCHAR(255),
		   @del_UnitPrice DECIMAL (18, 2),
		   @del_ExpirationDate DateTime,
           @del_Specifications VARCHAR(500),
           @del_Notes VARCHAR(500),
		   @del_IsDeleted VARCHAR(255),
		   @ModifiedBy INT,
		   @DateModified DateTime,
		   @ModifiedByName VARCHAR(255)

SELECT @TableName = 'InvoiceItems', @OperationName = 'Update';
SELECT @PrimaryKeyId = ins.Id, 
	   @Invoice = ins.InvoiceId,
	   @InvoiceItemNumber = ins.InvoiceItemNumber,
	   @AssetType = t.[Name],
	   @AssetCategory = c.[Name],
       @Manu = IsNull(m.DisplayName,m.[Name]),
	   @Product = IsNull(p.DisplayName,p.[Name]),
	   @LicenseType = lt.[Name],
	   @LicenseKey = ins.LicenseKeySingle,
	   @UnitPrice = ins.UnitPrice,
	   @ExpirationDate = ins.ExpirationDate,
       @Specifications = ins.Specifications,
       @Notes = ins.Notes,
	   @IsDeleted = CASE WHEN ins.IsDeleted = 0 THEN 'False' ELSE 'True' END,
	   @ModifiedBy = ins.ModifiedBy,
	   @DateModified = ins.DateModified, 
	   @ModifiedByName = IsNull(u.FirstName, '') + ' ' + IsNull(u.LastName, '')
	   FROM INSERTED ins
	   LEFT OUTER JOIN AssetTypes t ON ins.AssetTypeId = t.Id
	   LEFT OUTER JOIN AssetCategories c ON ins.AssetCategoryId = c.Id
	   LEFT OUTER JOIN Manufacturers m ON ins.ManuId = m.Id
	   LEFT OUTER JOIN Products p ON ins.ProductId = p.Id
	   LEFT OUTER JOIN LicenseTypes lt ON ins.LicenseTypeId = lt.Id
	   LEFT OUTER JOIN Users u ON ins.ModifiedBy = u.Id;

SELECT @del_PrimaryKeyId = del.ID, 
	   @del_Invoice = del.InvoiceId,
	   @del_InvoiceItemNumber = del.InvoiceItemNumber,
	   @del_AssetType = t.[Name],
	   @del_AssetCategory = c.[Name],
       @del_Manu = IsNull(m.DisplayName,m.[Name]),
	   @del_Product = IsNull(p.DisplayName,p.[Name]),
	   @del_LicenseType = lt.[Name],
	   @del_LicenseKey = del.LicenseKeySingle,
	   @del_UnitPrice = del.UnitPrice,
	   @del_ExpirationDate = del.ExpirationDate,
       @del_Specifications = del.Specifications,
       @del_Notes = del.Notes,
	   @del_IsDeleted = CASE WHEN del.IsDeleted = 0 THEN 'False' ELSE 'True' END
	   FROM DELETED del
	   LEFT OUTER JOIN AssetTypes t ON del.AssetTypeId = t.Id
	   LEFT OUTER JOIN AssetCategories c ON del.AssetCategoryId = c.Id
	   LEFT OUTER JOIN Manufacturers m ON del.ManuId = m.Id
	   LEFT OUTER JOIN Products p ON del.ProductId = p.Id
	   LEFT OUTER JOIN LicenseTypes lt ON del.LicenseTypeId = lt.Id
	   LEFT OUTER JOIN Users u ON del.ModifiedBy = u.Id;

IF (IsNull(@Invoice,'') <> IsNull(@del_Invoice,''))
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[AuditLogs](
        PrimaryKeyId, 
        TableName,
        ColumnName,
        OperationName,
        OldValue,
        NewValue,
		ModifiedBy,
        DateModified, 
        ModifiedByName
    )
    VALUES (
         @PrimaryKeyId,
         @TableName,
         'Invoice',
         @OperationName,
         @del_Invoice,
         @Invoice,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END


IF (IsNull(@InvoiceItemNumber,'') <> IsNull(@del_InvoiceItemNumber,''))
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[AuditLogs](
        PrimaryKeyId, 
        TableName,
        ColumnName,
        OperationName,
        OldValue,
        NewValue,
		ModifiedBy,
        DateModified, 
        ModifiedByName
    )
    VALUES (
         @PrimaryKeyId,
         @TableName,
         'Invoice Item Number',
         @OperationName,
         @del_InvoiceItemNumber,
         @InvoiceItemNumber ,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@AssetType,'') <> IsNull(@del_AssetType,''))
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[AuditLogs](
        PrimaryKeyId, 
        TableName,
        ColumnName,
        OperationName,
        OldValue,
        NewValue,
		ModifiedBy,
        DateModified, 
        ModifiedByName
    )
    VALUES (
         @PrimaryKeyId,
         @TableName,
         'Asset Type',
         @OperationName,
         @del_AssetType,
         @AssetType ,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@AssetCategory,'') <> IsNull(@del_AssetCategory,''))
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[AuditLogs](
        PrimaryKeyId, 
        TableName,
        ColumnName,
        OperationName,
        OldValue,
        NewValue,
		ModifiedBy,
        DateModified, 
        ModifiedByName
    )
    VALUES (
         @PrimaryKeyId,
         @TableName,
         'Asset Category',
         @OperationName,
         @del_AssetCategory,
         @AssetCategory,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@Manu,'') <> IsNull(@del_Manu,''))
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[AuditLogs](
        PrimaryKeyId, 
        TableName,
        ColumnName,
        OperationName,
        OldValue,
        NewValue,
		ModifiedBy,
        DateModified, 
        ModifiedByName
    )
    VALUES (
         @PrimaryKeyId,
         @TableName,
         'Manufacturer',
         @OperationName,
         @del_Manu,
         @Manu,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@Product,'') <> IsNull(@del_Product,''))
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[AuditLogs](
        PrimaryKeyId, 
        TableName,
        ColumnName,
        OperationName,
        OldValue,
        NewValue,
		ModifiedBy,
        DateModified, 
        ModifiedByName
    )
    VALUES (
         @PrimaryKeyId,
         @TableName,
         'Product',
         @OperationName,
         @del_Product,
         @Product ,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@LicenseType,'') <> IsNull(@del_LicenseType,''))
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[AuditLogs](
        PrimaryKeyId, 
        TableName,
        ColumnName,
        OperationName,
        OldValue,
        NewValue,
		ModifiedBy,
        DateModified, 
        ModifiedByName
    )
    VALUES (
         @PrimaryKeyId,
         @TableName,
         'License Type',
         @OperationName,
         @del_LicenseType,
         @LicenseType,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@LicenseKey,'') <> IsNull(@del_LicenseKey,''))
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[AuditLogs](
        PrimaryKeyId, 
        TableName,
        ColumnName,
        OperationName,
        OldValue,
        NewValue,
		ModifiedBy,
        DateModified, 
        ModifiedByName
    )
    VALUES (
         @PrimaryKeyId,
         @TableName,
         'License Key',
         @OperationName,
         @del_LicenseKey,
         @LicenseKey,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@UnitPrice,0) <> IsNull(@del_UnitPrice,0))
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[AuditLogs](
        PrimaryKeyId, 
        TableName,
        ColumnName,
        OperationName,
        OldValue,
        NewValue,
		ModifiedBy,
        DateModified, 
        ModifiedByName
    )
    VALUES (
         @PrimaryKeyId,
         @TableName,
         'Unit Price',
         @OperationName,
         @del_UnitPrice,
         @UnitPrice,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@ExpirationDate,'') <> IsNull(@del_ExpirationDate,''))
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[AuditLogs](
        PrimaryKeyId, 
        TableName,
        ColumnName,
        OperationName,
        OldValue,
        NewValue,
		ModifiedBy,
        DateModified, 
        ModifiedByName
    )
    VALUES (
         @PrimaryKeyId,
         @TableName,
         'ExpirationDate',
         @OperationName,
         @del_ExpirationDate,
         @ExpirationDate,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@Specifications,'') <> IsNull(@del_Specifications,''))
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[AuditLogs](
        PrimaryKeyId, 
        TableName,
        ColumnName,
        OperationName,
        OldValue,
        NewValue,
		ModifiedBy,
        DateModified, 
        ModifiedByName
    )
    VALUES (
         @PrimaryKeyId,
         @TableName,
         'Specifications',
         @OperationName,
         @del_Specifications,
         @Specifications,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@Notes,'') <> IsNull(@del_Notes,''))
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[AuditLogs](
        PrimaryKeyId, 
        TableName,
        ColumnName,
        OperationName,
        OldValue,
        NewValue,
		ModifiedBy,
        DateModified, 
        ModifiedByName
    )
    VALUES (
         @PrimaryKeyId,
         @TableName,
         'Notes',
         @OperationName,
         @del_Notes,
         @Notes,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@IsDeleted,'') <> IsNull(@del_IsDeleted,'') AND @IsDeleted = 'False')
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[AuditLogs](
        PrimaryKeyId, 
        TableName,
        ColumnName,
        OperationName,
        OldValue,
        NewValue,
		ModifiedBy,
        DateModified, 
        ModifiedByName
    )
    VALUES (
         @PrimaryKeyId,
         @TableName,
         'IsDeleted',
         @OperationName,
         @del_IsDeleted,
         @IsDeleted,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@IsDeleted,'') <> IsNull(@del_IsDeleted,'') AND @IsDeleted = 'True')
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[AuditLogs](
        PrimaryKeyId, 
        TableName,
        ColumnName,
        OperationName,
        OldValue,
        NewValue,
		ModifiedBy,
        DateModified, 
        ModifiedByName
    )
    VALUES (
         @PrimaryKeyId,
         @TableName,
         'IsDeleted',
         'Delete',
         @del_IsDeleted,
         @IsDeleted,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

GO

ALTER TABLE [dbo].[InvoiceItems] ENABLE TRIGGER [InvoiceItems_UTrig]
GO


