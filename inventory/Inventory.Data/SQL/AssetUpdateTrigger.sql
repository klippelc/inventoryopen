USE [Inventory]
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
		   @IPAddress VARCHAR(255),
		   @MacAddress VARCHAR(255),
		   @Status VARCHAR(255),
		   @SurplusDate DateTime,
		   @SNFnumber VARCHAR(255),
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
		   @del_PrimaryKeyId INT,
		   @del_InvoiceItem VARCHAR(255),
           @del_Name VARCHAR(255),
		   @del_Description VARCHAR(255),
		   @del_Drawer VARCHAR(255),
           @del_AssetTag VARCHAR(255),
		   @del_Serial VARCHAR(255),
		   @del_LicenseKey VARCHAR(255),
		   @del_IPAddress VARCHAR(255),
		   @del_MacAddress VARCHAR(255),
		   @del_Status VARCHAR(255),
           @del_SurplusDate DateTime,
		   @del_SNFnumber VARCHAR(255),
		   @del_DateReceived DateTime,
		   @del_Location VARCHAR(255),
		   @del_Building VARCHAR(255),
		   @del_Room VARCHAR(255),
           @del_AssignedAsset VARCHAR(255),
           @del_AssignedAssetId VARCHAR(255),
           @del_AssignedUser VARCHAR(255),
           @del_AssignedUserId VARCHAR(255),
		   @del_AssignedDate DateTime,
           @del_ConnectedAsset VARCHAR(255),
           @del_ConnectedAssetId VARCHAR(255),
           @del_LastLoginDate DateTime,
		   @del_LastBootDate DateTime,
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
	   @IPAddress = ins.IPAddress,
	   @MacAddress = ins.MacAddress,
	   @Status = s.[Name],
	   @SurplusDate = ins.SurplusDate,
	   @SNFnumber = ins.SNFnumber,
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
       @IPAddress = ins.IPAddress,
       @Drawer = ins.Drawer,
       @SNFnumber = ins.SNFnumber,
       @SurplusDate = ins.SurplusDate,
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

SELECT @del_PrimaryKeyId = del.ID, 
	   @del_InvoiceItem = i.InvoiceItemNumber,
       @del_Name = del.[Name],
       @del_Description = del.[Description],
       @del_Drawer = del.[Drawer],
       @del_AssetTag = del.AssetTag,
	   @del_Serial = del.Serial,
	   @del_LicenseKey = CASE WHEN IsNull(lt.[Name],'') = 'Hardware-Single' THEN i.LicenseKeySingle ELSE del.LicenseKeyMulti END,
	   @del_IPAddress = del.IPAddress,
	   @del_MacAddress = del.MacAddress,
	   @del_Status = s.[Name],
       @del_SurplusDate = del.SurplusDate,
	   @del_SNFnumber = del.SNFnumber,
	   @del_DateReceived = del.DateReceived,
	   @del_Location = IsNull(loc.DisplayName,loc.[Name]),
	   @del_Building = IsNull(blg.DisplayName,blg.[Name]),
	   @del_Room = IsNull(room.DisplayName,room.[Name]),
       @del_AssignedAsset = aa.Serial,
       @del_AssignedAssetId = aa.Id,
       @del_AssignedUser = CASE WHEN IsNull(au.Id,0) <> 0 THEN IsNull(au.FirstName, '') + ' ' + IsNull(au.LastName, '') ELSE Null END,
       @del_AssignedUserId = au.Id,
       @del_AssignedDate = del.AssignedDate,
       @del_ConnectedAsset = ca.Serial,
       @del_ConnectedAssetId = ca.Id,
       @del_IPAddress = del.IPAddress,
       @del_Drawer = del.Drawer,
       @del_SNFnumber = del.SNFnumber,
       @del_SurplusDate = del.SurplusDate,
       @del_LastLoginDate = del.LastLoginDate,
       @del_LastBootDate = del.LastBootDate,
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

IF (IsNull(@IPAddress,'') <> IsNull(@del_IPAddress,'')) 
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
        'IP Address',
         @OperationName,
         @del_IPAddress,
         @IPAddress,
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

IF (IsNull(@SurplusDate,'') <> IsNull(@del_SurplusDate,''))
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
        'SurplusDate',
         @OperationName,
         @del_SurplusDate,
         @SurplusDate,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@SNFnumber,'') <> IsNull(@del_SNFnumber,''))
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
        'Surplus Number',
         @OperationName,
         @del_SNFnumber,
         @SNFnumber,
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

IF (IsNull(@AssignedDate,'') <> IsNull(@del_AssignedDate,''))
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
         @del_AssignedDate,
         @AssignedDate,
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

--

IF (IsNull(@LastLoginDate,'') <> IsNull(@del_LastLoginDate,''))
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
         @del_LastLoginDate,
         @LastLoginDate,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

IF (IsNull(@LastBootDate,'') <> IsNull(@del_LastBootDate,''))
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
         @del_LastBootDate,
         @LastBootDate,
		 @ModifiedBy,
         @DateModified,
         @ModifiedByName )
END

--

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


