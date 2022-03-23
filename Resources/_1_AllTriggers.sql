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
           @ConnectedAsset VARCHAR(255),
           @ConnectedAssetId VARCHAR(255),
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
       @ConnectedAsset = ca.Serial,
       @ConnectedAssetId = ca.Id,
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




---

GO

DROP TRIGGER IF EXISTS [dbo].[Assets_UTrig]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [Assets_UTrig]
ON [dbo].[Assets]
AFTER UPDATE
AS DECLARE @TableName VARCHAR(255),
		   @OperationName VARCHAR(255),
           @PrimaryKeyId INT,
		   @InvoiceItem VARCHAR(255),
		   @Name VARCHAR(255),
		   @Description VARCHAR(255),
		   @Drawer VARCHAR(255),
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
           @ConnectedAsset VARCHAR(255),
           @ConnectedAssetId VARCHAR(255),
           @Notes VARCHAR(255),
           @Display VARCHAR(255),
		   @IsDeleted VARCHAR(255),
		   @del_PrimaryKeyId INT,
		   @del_InvoiceItem VARCHAR(255),
           @del_Name VARCHAR(255),
		   @del_Description VARCHAR(255),
		   @del_Drawer VARCHAR(255),
           @del_AssetTag VARCHAR(255),
		   @del_Serial VARCHAR(255),
		   @del_LicenseKey VARCHAR(255),
		   @del_MacAddress VARCHAR(255),
		   @del_Status VARCHAR(255),
		   @del_DateReceived DateTime,
		   @del_Location VARCHAR(255),
		   @del_Building VARCHAR(255),
		   @del_Room VARCHAR(255),
           @del_AssignedAsset VARCHAR(255),
           @del_AssignedAssetId VARCHAR(255),
           @del_AssignedUser VARCHAR(255),
           @del_AssignedUserId VARCHAR(255),
           @del_ConnectedAsset VARCHAR(255),
           @del_ConnectedAssetId VARCHAR(255),
           @del_Notes VARCHAR(255),
           @del_Display VARCHAR(255),
		   @del_IsDeleted VARCHAR(255),
		   @ModifiedBy INT,
		   @DateModified DateTime,
		   @ModifiedByName VARCHAR(255)

SELECT @TableName = 'Assets', @OperationName = 'Update';
SELECT @PrimaryKeyId = ins.ID, 
	   @InvoiceItem = i.InvoiceItemNumber,
       @Name = ins.[Name],
       @Description = ins.[Description],
       @Drawer = ins.Drawer,
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
       @ConnectedAsset = ca.Serial,
       @ConnectedAssetId = ca.Id,
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

SELECT @del_PrimaryKeyId = del.ID, 
	   @del_InvoiceItem = i.InvoiceItemNumber,
       @del_Name = del.[Name],
       @del_Description = del.[Description],
       @del_Drawer = del.[Drawer],
       @del_AssetTag = del.AssetTag,
	   @del_Serial = del.Serial,
	   @del_LicenseKey = CASE WHEN IsNull(lt.[Name],'') = 'Hardware-Single' THEN i.LicenseKeySingle ELSE del.LicenseKeyMulti END,
	   @del_MacAddress = del.MacAddress,
	   @del_Status = s.[Name],
	   @del_DateReceived = del.DateReceived,
	   @del_Location = IsNull(loc.DisplayName,loc.[Name]),
	   @del_Building = IsNull(blg.DisplayName,blg.[Name]),
	   @del_Room = IsNull(room.DisplayName,room.[Name]),
       @del_AssignedAsset = aa.Serial,
       @del_AssignedAssetId = aa.Id,
       @del_AssignedUser = CASE WHEN IsNull(au.Id,0) <> 0 THEN IsNull(au.FirstName, '') + ' ' + IsNull(au.LastName, '') ELSE Null END,
       @del_AssignedUserId = au.Id,
       @del_ConnectedAsset = ca.Serial,
       @del_ConnectedAssetId = ca.Id,
       @del_Notes = del.Notes,
       @del_Display = CASE WHEN del.Display = 0 THEN 'False' ELSE 'True' END,
	   @del_IsDeleted = CASE WHEN del.IsDeleted = 0 THEN 'False' ELSE 'True' END
	   FROM DELETED del
	   LEFT OUTER JOIN InvoiceItems i ON del.InvoiceItemId = i.Id
	   LEFT OUTER JOIN LicenseTypes lt ON i.LicenseTypeId = lt.Id
	   LEFT OUTER JOIN AssetStatus s ON del.StatusId = s.Id
	   LEFT OUTER JOIN Locations loc ON del.LocationId = loc.Id
	   LEFT OUTER JOIN Buildings blg ON del.BuildingId = blg.Id
	   LEFT OUTER JOIN Rooms room ON del.RoomId = room.Id
	   LEFT OUTER JOIN Assets aa ON del.AssignedAssetId = aa.Id
	   LEFT OUTER JOIN Users au ON del.AssignedUserId = au.Id
	   LEFT OUTER JOIN Assets ca ON del.ConnectedAssetId = ca.Id;

IF (IsNull(@InvoiceItem,'') <> IsNull(@del_InvoiceItem,''))
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
         @del_InvoiceItem,
         @InvoiceItem,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@Name,'') <> IsNull(@del_Name,''))
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
        'Name',
         @OperationName,
         @del_Name,
         @Name,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@Description,'') <> IsNull(@del_Description,''))
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
        'Description',
         @OperationName,
         @del_Description,
         @Description,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@Drawer,'') <> IsNull(@del_Drawer,''))
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
        'Drawer',
         @OperationName,
         @del_Drawer,
         @Drawer,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@AssetTag,'') <> IsNull(@del_AssetTag,''))
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
         @del_AssetTag,
         @AssetTag,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@Serial,'') <> IsNull(@del_Serial,''))
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
         @del_Serial,
         @Serial,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@LicenseKey, '') <> IsNull(@del_LicenseKey, ''))
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

IF (IsNull(@MacAddress,'') <> IsNull(@del_MacAddress,'')) 
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
         @del_MacAddress,
         @MacAddress,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@Status,'') <> IsNull(@del_Status,''))
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
         @del_Status,
         @Status,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@DateReceived,'') <> IsNull(@del_DateReceived,''))
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
         @del_DateReceived,
         @DateReceived,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@Location,'') <> IsNull(@del_Location,''))
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
         @del_Location,
         @Location,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@Building,'') <> IsNull(@del_Building,''))
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
         @del_Building,
         @Building,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@Room,'') <> IsNull(@del_Room,''))
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
         @del_Room,
         @Room,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@AssignedAsset,'') <> IsNull(@del_AssignedAsset,''))
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
         @del_AssignedAsset,
         @AssignedAsset,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@AssignedAssetId,'') <> IsNull(@del_AssignedAssetId,''))
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
         @del_AssignedAssetId,
         @AssignedAssetId,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@AssignedUser,'') <> IsNull(@del_AssignedUser,''))
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
         @del_AssignedUser,
         @AssignedUser,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@AssignedUserId,'') <> IsNull(@del_AssignedUserId,''))
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
         @del_AssignedUserId,
         @AssignedUserId,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@ConnectedAsset,'') <> IsNull(@del_ConnectedAsset,''))
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
         @del_ConnectedAsset,
         @ConnectedAsset,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@ConnectedAssetId,'') <> IsNull(@del_ConnectedAssetId,''))
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
         @del_ConnectedAssetId,
         @ConnectedAssetId,
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

IF (IsNull(@Display,'') <> IsNull(@del_Display,''))
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
        'Display',
         @OperationName,
         @del_Display,
         @Display,
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

ALTER TABLE [dbo].[Assets] ENABLE TRIGGER [Assets_UTrig]
GO



---

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


