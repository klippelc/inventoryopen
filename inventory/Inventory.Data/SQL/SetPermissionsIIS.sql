CREATE LOGIN [IIS APPPOOL\ParksInventory] FROM WINDOWS
GO

ALTER  SERVER ROLE [dbcreator] ADD MEMBER [IIS APPPOOL\ParksInventory]
GO