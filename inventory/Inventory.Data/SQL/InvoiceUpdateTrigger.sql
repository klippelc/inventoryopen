USE [Inventory]
GO

DROP TRIGGER IF EXISTS [dbo].[Invoices_UTrig]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [Invoices_UTrig]
ON [dbo].[Invoices]
AFTER UPDATE
AS DECLARE @TableName VARCHAR(255),
		   @OperationName VARCHAR(255),
           @PrimaryKeyId INT,
		   @PONumber VARCHAR(255),
		   @Supplier VARCHAR(255),
		   @PurchaseDate DateTime,
		   @TotalPrice DECIMAL (18, 2),
		   @IsDeleted VARCHAR(255),
		   @del_PrimaryKeyId INT,
           @del_PONumber VARCHAR(255),
           @del_InvoiceNumber VARCHAR(255),
           @del_Supplier VARCHAR(255),
           @del_PurchaseDate DateTime,
           @del_TotalPrice DECIMAL (18, 2),
		   @del_IsDeleted VARCHAR(255),
		   @ModifiedBy INT,
		   @DateModified DateTime,
		   @ModifiedByName VARCHAR(255)

SELECT @TableName = 'Invoices', @OperationName = 'Update';
SELECT @PrimaryKeyId = ins.ID, 
	   @PONumber = ins.PONumber,
	   @Supplier = IsNull(s.DisplayName, IsNull(s.[Name], ins.SupplierId)),
	   @PurchaseDate = ins.PurchaseDate,
	   @TotalPrice = ins.TotalPrice,
	   @IsDeleted = CASE WHEN ins.IsDeleted = 0 THEN 'False' ELSE 'True' END,
	   @ModifiedBy = IsNull(ins.ModifiedBy, 0), 
	   @DateModified = ins.DateModified,
	   @ModifiedByName = IsNull(u.FirstName, '') + ' ' + IsNull(u.LastName, '')
	   FROM INSERTED ins
	   LEFT OUTER JOIN Suppliers s ON ins.SupplierId = s.Id
	   LEFT OUTER JOIN Users u ON ins.ModifiedBy = u.Id;

SELECT @del_PrimaryKeyId = del.ID, 
	   @del_PONumber = del.PONumber,
	   @del_Supplier = IsNull(s.DisplayName, IsNull(s.[Name], del.SupplierId)),
	   @del_PurchaseDate = del.PurchaseDate,
	   @del_TotalPrice = del.TotalPrice,
	   @del_IsDeleted = CASE WHEN del.IsDeleted = 0 THEN 'False' ELSE 'True' END
	   FROM DELETED del
	   LEFT OUTER JOIN Suppliers s ON del.SupplierId = s.Id

IF (IsNull(@PONumber,'') <> IsNull(@del_PONumber,''))
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
        'PONumber',
         @OperationName,
         @del_PONumber,
         @PONumber,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@Supplier,'') <> IsNull(@del_Supplier,''))
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
         @del_Supplier,
         @Supplier,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@PurchaseDate,'') <> IsNull(@del_PurchaseDate,''))
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
        'Purchase Date',
         @OperationName,
         @del_PurchaseDate,
         @PurchaseDate,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@TotalPrice,'') <> IsNull(@del_TotalPrice,''))
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
        'Total Price',
         @OperationName,
         @del_TotalPrice,
         @TotalPrice,
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

ALTER TABLE [dbo].[Invoices] ENABLE TRIGGER [Invoices_UTrig]
GO


