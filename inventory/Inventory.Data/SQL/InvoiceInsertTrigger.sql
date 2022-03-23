USE [Inventory]
GO

DROP TRIGGER IF EXISTS [dbo].[Invoices_ITrig]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [Invoices_ITrig]
ON [dbo].[Invoices]
AFTER INSERT
AS DECLARE @TableName VARCHAR(255),
		   @OperationName VARCHAR(255),
           @PrimaryKeyId INT,
		   @PONumber VARCHAR(255),
		   @Supplier VARCHAR(255),
		   @PurchaseDate DateTime,
		   @TotalPrice DECIMAL (18, 2),
		   @IsDeleted VARCHAR(255),
		   @ModifiedBy INT,
		   @DateModified DateTime,
		   @ModifiedByName VARCHAR(255)

SELECT @TableName = 'Invoices', @OperationName = 'Insert';

SELECT @PrimaryKeyId = ins.ID, 
	   @PONumber = ins.PONumber,
	   @Supplier = IsNull(s.DisplayName, IsNull(s.[Name], ins.SupplierId)),
	   @PurchaseDate = ins.PurchaseDate,
	   @TotalPrice = ins.TotalPrice,
	   @IsDeleted = CASE WHEN ins.IsDeleted = 0 THEN 'False' ELSE 'True' END,
	   @ModifiedBy = ins.ModifiedBy, 
	   @DateModified = IsNull(ins.DateModified, Null), 
	   @ModifiedByName = IsNull(u.FirstName, '') + ' ' + IsNull(u.LastName, '')
	   FROM INSERTED ins
	   LEFT OUTER JOIN Suppliers s ON ins.SupplierId = s.Id
	   LEFT OUTER JOIN Users u ON ins.ModifiedBy = u.Id;

IF (@PONumber IS NOT NULL) 
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
         Null,
         @PONumber,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@Supplier IS NOT NULL) 
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
        'Supplier',
         @OperationName,
         Null,
         @Supplier,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@PurchaseDate IS NOT NULL) 
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
         Null,
         @PurchaseDate,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@TotalPrice IS NOT NULL) 
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
         Null,
         @TotalPrice,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

GO

ALTER TABLE [dbo].[Invoices] ENABLE TRIGGER [Invoices_ITrig]
GO


