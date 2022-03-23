USE [Inventory]
GO

DROP TRIGGER IF EXISTS [dbo].[InvoiceItems_ITrig]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [InvoiceItems_ITrig]
ON [dbo].[InvoiceItems]
AFTER INSERT
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
		   @ModifiedBy INT,
		   @DateModified DateTime,
		   @ModifiedByName VARCHAR(255)

SELECT @TableName = 'InvoiceItems', @OperationName = 'Insert';
SELECT @PrimaryKeyId = ins.ID, 
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
	   @ModifiedBy = IsNull(ins.ModifiedBy, 0), 
	   @DateModified = ins.DateModified, 
	   @ModifiedByName = IsNull(u.FirstName, '') + ' ' + IsNull(u.LastName, '')
	   FROM INSERTED ins
	   LEFT OUTER JOIN AssetTypes t ON ins.AssetTypeId = t.Id
	   LEFT OUTER JOIN AssetCategories c ON ins.AssetCategoryId = c.Id
	   LEFT OUTER JOIN Manufacturers m ON ins.ManuId = m.Id
	   LEFT OUTER JOIN Products p ON ins.ProductId = p.Id
	   LEFT OUTER JOIN LicenseTypes lt ON ins.LicenseTypeId = lt.Id
	   LEFT OUTER JOIN Users u ON ins.ModifiedBy = u.Id;

IF (@Invoice IS NOT NULL) 
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
         Null,
         @Invoice,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END


IF (@InvoiceItemNumber IS NOT NULL) 
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
         Null,
         @InvoiceItemNumber ,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@AssetType IS NOT NULL) 
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
         Null,
         @AssetType,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@AssetCategory IS NOT NULL) 
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
         Null,
         @AssetCategory,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@Manu IS NOT NULL) 
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
         Null,
         @Manu,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@Product IS NOT NULL) 
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
         Null,
         @Product ,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@LicenseType IS NOT NULL) 
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
         Null,
         @LicenseType,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@LicenseKey IS NOT NULL) 
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
         Null,
         @LicenseKey,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@UnitPrice IS NOT NULL) 
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
         Null,
         @UnitPrice,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@ExpirationDate IS NOT NULL) 
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
         Null,
         @ExpirationDate,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@Specifications IS NOT NULL) 
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
         Null,
         @Specifications,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@Notes IS NOT NULL) 
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
         Null,
         @Notes,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

GO

ALTER TABLE [dbo].[InvoiceItems] ENABLE TRIGGER [InvoiceItems_ITrig]
GO


