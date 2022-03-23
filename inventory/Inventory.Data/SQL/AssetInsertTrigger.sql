USE [Inventory]
GO

DROP TRIGGER IF EXISTS [dbo].[Assets_ITrig]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [Assets_ITrig]
ON [dbo].[Assets]
AFTER INSERT
AS DECLARE @TableName VARCHAR(255),
		   @OperationName VARCHAR(255),
           @PrimaryKeyId INT,
		   @InvoiceItem VARCHAR(255),
           @AssetTag VARCHAR(255),
		   @Serial VARCHAR(255),
		   @LicenseKey VARCHAR(255),
		   @MacAddress VARCHAR(255),
		   @Status VARCHAR(255),
		   @DateReceived DateTime,
		   @Location VARCHAR(255),
		   @Building VARCHAR(255),
		   @Room VARCHAR(255),
           @AssignedAsset VARCHAR(255),
           @AssignedAssetId VARCHAR(255),
           @AssignedUser VARCHAR(255),
           @AssignedUserId VARCHAR(255),
		   @AssignedDate DateTime,
           @ConnectedAsset VARCHAR(255),
           @ConnectedAssetId VARCHAR(255),
           @LastLoginDate DateTime,
		   @LastBootDate DateTime,
           @Notes VARCHAR(255),
           @Display VARCHAR(255),
		   @IsDeleted VARCHAR(255),
		   @ModifiedBy INT,
		   @DateModified DateTime,
		   @ModifiedByName VARCHAR(255)


SELECT @TableName = 'Assets', @OperationName = 'Insert';
SELECT @PrimaryKeyId = ins.ID, 
	   @InvoiceItem = i.InvoiceItemNumber,
       @AssetTag = ins.AssetTag,
	   @Serial = ins.Serial,
	   @LicenseKey = CASE WHEN IsNull(lt.[Name],'') = 'Hardware-Single' THEN i.LicenseKeySingle ELSE ins.LicenseKeyMulti END,
	   @MacAddress = ins.MacAddress,
	   @Status = s.[Name],
	   @DateReceived = ins.DateReceived,
	   @Location = IsNull(loc.DisplayName,loc.[Name]),
	   @Building = IsNull(blg.DisplayName,blg.[Name]),
	   @Room = IsNull(room.DisplayName,room.[Name]),
       @AssignedAsset = aa.Serial,
       @AssignedAssetId = aa.Id,
       @AssignedUser = CASE WHEN IsNull(au.Id,0) <> 0 THEN IsNull(au.FirstName, '') + ' ' + IsNull(au.LastName, '') ELSE Null END,
       @AssignedUserId = au.Id,
       @AssignedDate = ins.AssignedDate,
       @ConnectedAsset = ca.Serial,
       @ConnectedAssetId = ca.Id,
       @LastLoginDate = ins.LastLoginDate,
       @LastBootDate = ins.LastBootDate,
       @Notes = ins.Notes,
       @Display = CASE WHEN ins.Display = 0 THEN 'False' ELSE 'True' END,
	   @IsDeleted = CASE WHEN ins.IsDeleted = 0 THEN 'False' ELSE 'True' END,
	   @ModifiedBy = ins.ModifiedBy, 
	   @DateModified = ins.DateModified, 
	   @ModifiedByName = IsNull(u.FirstName, '') + ' ' + IsNull(u.LastName, '')
	   FROM INSERTED ins
	   LEFT OUTER JOIN InvoiceItems i ON ins.InvoiceItemId = i.Id
	   LEFT OUTER JOIN LicenseTypes lt ON i.LicenseTypeId = lt.Id
	   LEFT OUTER JOIN AssetStatus s ON ins.StatusId = s.Id
	   LEFT OUTER JOIN Locations loc ON ins.LocationId = loc.Id
	   LEFT OUTER JOIN Buildings blg ON ins.BuildingId = blg.Id
	   LEFT OUTER JOIN Rooms room ON ins.RoomId = room.Id
	   LEFT OUTER JOIN Assets aa ON ins.AssignedAssetId = aa.Id
	   LEFT OUTER JOIN Users au ON ins.AssignedUserId = au.Id
       LEFT OUTER JOIN Assets ca ON ins.ConnectedAssetId = ca.Id
	   LEFT OUTER JOIN Users u ON ins.ModifiedBy = u.Id;

IF (@InvoiceItem IS NOT NULL) 
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
        'Invoice Item',
         @OperationName,
         Null,
         @InvoiceItem,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@AssetTag IS NOT NULL) 
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
        'Asset Tag',
         @OperationName,
         Null,
         @AssetTag,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@Serial IS NOT NULL) 
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
        'Serial',
         @OperationName,
         Null,
         @Serial,
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

IF (@MacAddress IS NOT NULL) 
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
        'Mac Address',
         @OperationName,
         Null,
         @MacAddress,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@Status IS NOT NULL) 
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
        'Status',
         @OperationName,
         Null,
         @Status,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@DateReceived IS NOT NULL) 
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
         'Date Received',
         @OperationName,
         Null,
         @DateReceived,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@Location IS NOT NULL) 
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
        'Location',
         @OperationName,
         Null,
         @Location,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@Building IS NOT NULL) 
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
        'Building',
         @OperationName,
         Null,
         @Building,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@Room IS NOT NULL) 
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
        'Room',
         @OperationName,
         Null,
         @Room,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@AssignedAsset IS NOT NULL) 
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
        'Assigned Asset',
         @OperationName,
         Null,
         @AssignedAsset,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@AssignedAssetId IS NOT NULL) 
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
        'Assigned Asset Id',
         @OperationName,
         Null,
         @AssignedAssetId,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@AssignedUser IS NOT NULL) 
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
        'Assigned User',
         @OperationName,
         Null,
         @AssignedUser,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@AssignedUserId IS NOT NULL) 
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
        'Assigned User Id',
         @OperationName,
         Null,
         @AssignedUserId,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@AssignedDate IS NOT NULL) 
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
        'Assigned Date',
         @OperationName,
         Null,
         @AssignedDate,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@ConnectedAsset IS NOT NULL) 
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
        'Connected Asset',
         @OperationName,
         Null,
         @ConnectedAsset,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@ConnectedAssetId IS NOT NULL) 
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
        'Connected Asset Id',
         @OperationName,
         Null,
         @ConnectedAssetId,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

--
IF (@LastLoginDate IS NOT NULL) 
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
        'Last Login Date',
         @OperationName,
         Null,
         @LastLoginDate,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (@LastBootDate IS NOT NULL) 
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
        'Last Boot Date',
         @OperationName,
         Null,
         @LastBootDate,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

--

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

ALTER TABLE [dbo].[Assets] ENABLE TRIGGER [Assets_ITrig]
GO


