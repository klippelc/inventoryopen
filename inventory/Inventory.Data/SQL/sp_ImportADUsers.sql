USE [Inventory]
GO

/****** Object:  StoredProcedure [dbo].[sp_ImportADUsers]    Script Date: 3/9/2020 11:46:30 AM ******/
DROP PROCEDURE [dbo].[sp_ImportADUsers]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Chris Klippel
-- Create date: 03/05/2020
-- Description:	Import Active Directory Users (be sure linked server AD is setup first)
-- =============================================
CREATE PROCEDURE [dbo].[sp_ImportADUsers]

AS

DECLARE @SYMBOLS AS VARCHAR(128);
DECLARE @query VARCHAR(MAX);
DECLARE @ADfields VARCHAR(MAX);

BEGIN

	---
	SET @SYMBOLS_2 = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
	SET @ADfields_2 = 'sAMAccountName, givenname, sn';
	---

	SET @SYMBOLS = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789'; -- AD Search character prefixes used to partition search. (1000 page limit). Below, AD fields to retrieve:
	SET @ADfields = 'createTimeStamp, sAMAccountName, title, telephoneNumber, userPrincipalName, userAccountControl, employeeID, givenname, sn, extensionAttribute9, manager, lastLogon';
	SET @query = 'SELECT * INTO ##tmpAD FROM (';

	-- Get each character and for each character construct and AD query:
	WITH nmTbl AS (SELECT TOP (LEN(@SYMBOLS)) Idx = ROW_NUMBER() OVER (ORDER BY [object_id]) FROM sys.all_objects ORDER BY Idx)
	SELECT @query = @query + 'SELECT * FROM OPENQUERY(AD, ''SELECT ' + @ADfields + ' FROM ''''LDAP://OU=PKS,OU=Organization,DC=bc,DC=broward,DC=cty''''   
	WHERE employeeID = ''''*'''' AND objectCategory = ''''Person'''' AND (sAMAccountName = ''''' + SUBSTRING(@SYMBOLS, nmTbl.Idx, 1) + '*'''') '')
	UNION
	' FROM nmTbl;
 
	-- Finish generating query string:
	SELECT @query = LEFT(@query, LEN(@query) - CHARINDEX(REVERSE('UNION'), REVERSE(@query)) - 4) + ') AS qry'
 
	-- Remove temp table if existing before running:
	IF OBJECT_ID('tempdb.dbo.##tmpAD', 'U') IS NOT NULL
		DROP TABLE ##tmpAD; 
	EXECUTE(@query)
 
--- INSERT
    INSERT INTO [Inventory].[dbo].[Users] (EmployeeId, Email, Phone, Title, UserName, FirstName, LastName, Division, Park, ManagerOUPath, Active, ADLastLogonDate, AdminCreated, CreatedBy, ModifiedBy, DateAdded, DateModified)
	 	 SELECT Ltrim(Rtrim(t.EmployeeId)) as EmployeeId,
			    Lower(Ltrim(Rtrim(t.userPrincipalName))) as Email, 
			    Ltrim(Rtrim(t.telephoneNumber)) as Phone, 
			    Ltrim(Rtrim(t.title)) as Title, 
			    Lower(Ltrim(Rtrim(t.sAMAccountName))) as UserName, 
			    Upper(LEFT(Ltrim(Rtrim(t.givenname)), 1)) + lower(RIGHT(Ltrim(Rtrim(t.givenname)), len(t.givenname)-1)) as FirstName,
			    Upper(LEFT(Ltrim(Rtrim(t.sn)), 1)) + lower(RIGHT(Ltrim(Rtrim(t.sn)), len(t.sn)-1)) as LastName,
				'Parks' as Division,    
				Ltrim(Rtrim(t.extensionAttribute9)) as Park,
				Ltrim(Rtrim(t.manager)) as ManagerOUPath,
			    CASE 
			 	 WHEN t.userAccountControl = '514' THEN 0
				 ELSE 1
			    END as Active,
				TRY_CONVERT(datetime,((TRY_CONVERT(numeric(38,6), t.lastlogon)) / 864000000000.0 - 109207)) as ADLastLogonDate,
			    0 as AdminCreated,
				0 as CreatedBy,
				0 as ModfiedBy,
				t.createTimeStamp as DateAdded,
			    t.createTimeStamp as DateModified
		   FROM ##tmpAD t 
LEFT OUTER JOIN [Inventory].[dbo].[Users] u WITH (NOLOCK)
		     ON t.EmployeeId = u.EmployeeId
		  WHERE u.EmployeeId IS NULL
	   ORDER BY username
--------------------------------------

--- UPDATE Active Where Users Removed From OU
	      UPDATE [Inventory].[dbo].[Users] 
		     SET Active = 0,
			     DateModified = GetDate(),
				 ModifiedBy = 0
   		    FROM ##tmpAD t 
RIGHT OUTER JOIN [Inventory].[dbo].[Users] u WITH (NOLOCK)
		      ON t.EmployeeId = u.EmployeeId
		   WHERE t.EmployeeId IS NULL 
		     AND u.AdminCreated = 0
			 AND u.Active = 1
--------------------------------------

--- UPDATE
	  UPDATE [Inventory].[dbo].[Users] 
		 SET Email = Lower(Ltrim(Rtrim(t.userPrincipalName))),
			 Phone = Ltrim(Rtrim(t.telephoneNumber)),
			 Title = Ltrim(Rtrim(t.title)),
			 UserName = Lower(Ltrim(Rtrim(t.sAMAccountName))),
			 FirstName = Upper(LEFT(Ltrim(Rtrim(t.givenname)), 1)) + lower(RIGHT(Ltrim(Rtrim(t.givenname)), len(t.givenname)-1)),
			 LastName = Upper(LEFT(Ltrim(Rtrim(t.sn)), 1)) + lower(RIGHT(Ltrim(Rtrim(t.sn)), len(t.sn)-1)),
			 Division = 'Parks',
			 Park = Ltrim(Rtrim(t.extensionAttribute9)),
			 Active = CASE WHEN t.userAccountControl = '514' THEN '0' ELSE '1' END,
			 ADLastLogonDate = TRY_CONVERT(datetime,((TRY_CONVERT(numeric(38,6), t.lastlogon)) / 864000000000.0 - 109207)),
		     DateModified = GetDate(),
			 ModifiedBy = 0
		FROM ##tmpAD t 
  INNER JOIN [Inventory].[dbo].[Users] u WITH (NOLOCK)
		  ON RTrim(LTrim(IsNull(t.EmployeeId,''))) = RTrim(LTrim(IsNull(u.EmployeeId,''))) 
	   WHERE u.AdminCreated = 0
-----------------------------------

--- Add the UserAssetView Role
	INSERT INTO UserRoles (UserId, RoleId, IsDeleted, CreatedBy, ModifiedBy, DateAdded, DateModified)
			(SELECT ur.Id as UserId,
					(Select Id from Roles Where [Name] = 'UserAssetView') as RoleId,
					0 as IsDeleted,
					0 as CreatedBy,
					0 as ModifiedBy,
					GetDate() as DateAdded,
					GetDate() as DateModified

			   FROM Users ur
	Left Outer Join UserRoles usr
				 On ur.Id = usr.UserId
				And usr.RoleId = (Select Id from Roles Where [Name] = 'UserAssetView')
				And usr.IsDeleted = 0
			  Where usr.Id Is Null
				And ur.Active = 1
		   Group by ur.Id)

-----------------------------------

--- Update Manager for each user record where managerOUPath is different between Users table and tmpAD
		  SELECT u1.Id, 
		         Ltrim(Rtrim(t1.manager)) as ManagerOUPath 
		    INTO ##tempUsers
   		    FROM ##tmpAD t1 
      INNER JOIN [Inventory].[dbo].[Users] u1 WITH (NOLOCK)
		      ON RTrim(LTrim(IsNull(t1.EmployeeId,''))) = RTrim(LTrim(IsNull(u1.EmployeeId,'')))
		   WHERE RTrim(LTrim(IsNull(t1.manager,''))) != RTrim(LTrim(IsNull(u1.ManagerOUPath,'')))
		   	  OR u1.ManagerId is null
			 AND u1.ManagerFirstName is null
			 AND u1.ManagerLastName is null
			 AND RTrim(LTrim(IsNull(u1.ManagerOUPath,''))) <> ''
		  
	WHILE EXISTS (SELECT Top 1 Id FROM ##tempUsers)
	BEGIN
		SET @UserId = (Select TOP 1 Id From ##tempUsers)
		SET @UserManagerOUPath = (Select ManagerOUPath From ##tempUsers Where Id = @UserId)
		---
		SET @query_2 = 'SELECT * INTO ##tmpAD2 FROM (';

		WITH nmTbl2 AS (SELECT TOP (LEN(@SYMBOLS_2)) Idx = ROW_NUMBER() OVER (ORDER BY [object_id]) FROM sys.all_objects ORDER BY Idx)
		SELECT @query_2 = @query_2 + 'SELECT * FROM OPENQUERY(AD, ''SELECT ' + @ADfields_2  + ' FROM ''''LDAP://'+@UserManagerOUPath+'''''
		WHERE employeeID = ''''*'''' AND objectCategory = ''''Person'''' AND (sAMAccountName = ''''' + SUBSTRING(@SYMBOLS_2, nmTbl2.Idx, 1) + '*'''') '')
		UNION
		' FROM nmTbl2;
 
		SELECT @query_2 = LEFT(@query_2, LEN(@query_2) - CHARINDEX(REVERSE('UNION'), REVERSE(@query_2)) - 4) + ') AS qry'
 
		IF OBJECT_ID('tempdb.dbo.##tmpAD2', 'U') IS NOT NULL
			DROP TABLE ##tmpAD2;
		EXECUTE(@query_2)

		IF OBJECT_ID('tempdb.dbo.##tmpAD2', 'U') IS NOT NULL
		BEGIN
				SET @ManagerUserName = null;
				SET @ManagerUserName = (SELECT Lower(Ltrim(Rtrim(IsNull(sAMAccountName,'')))) FROM ##tmpAD2);
				SET @ManagerFirstName = (SELECT Upper(LEFT(Ltrim(Rtrim(givenname)), 1)) + lower(RIGHT(Ltrim(Rtrim(givenname)), len(givenname)-1)) FROM ##tmpAD2);
				SET @ManagerLastName = (SELECT Upper(LEFT(Ltrim(Rtrim(sn)), 1)) + lower(RIGHT(Ltrim(Rtrim(sn)), len(sn)-1)) FROM ##tmpAD2);
				SET @ManagerId = (SELECT Id FROM [Inventory].[dbo].[Users] Where Lower(Ltrim(Rtrim(IsNull(UserName,'')))) =  @ManagerUserName);

			IF (@ManagerUserName IS NOT NULL)
			BEGIN
			UPDATE [Inventory].[dbo].[Users] 
					SET ManagerId = @ManagerId,
					    ManagerFirstName = @ManagerFirstName,
						ManagerLastName = @ManagerLastName,
						DateModified = GetDate(),
						ModifiedBy = 0
   		     WHERE Id = @UserId
			END
		END

		DELETE FROM ##tempUsers Where Id = @UserId
		SET @ManagerUserName = null;
		SET @ManagerFirstName = null;
		SET @ManagerLastName = null;
		SET @ManagerId = null;
	END

--- UPDATE ManagerPath. Update (ManagerId, Manager First/Last Name IF managerPath is null)
	  UPDATE [Inventory].[dbo].[Users] 
		 SET ManagerOUPath = Ltrim(Rtrim(t2.manager)),
		     ManagerId = IIF (RTrim(LTrim(IsNull(t2.Manager,''))) = '', null, u2.ManagerId),
		     ManagerFirstName = IIF (RTrim(LTrim(IsNull(t2.Manager,''))) = '', null, u2.ManagerFirstName),
		     ManagerLastName = IIF (RTrim(LTrim(IsNull(t2.Manager,''))) = '', null, u2.ManagerLastName)
		FROM ##tmpAD t2
  INNER JOIN [Inventory].[dbo].[Users] u2 WITH (NOLOCK)
		  ON RTrim(LTrim(IsNull(t2.EmployeeId,''))) = RTrim(LTrim(IsNull(u2.EmployeeId,'')))
	   WHERE u2.AdminCreated = 0
	     AND RTrim(LTrim(IsNull(t2.manager,''))) != RTrim(LTrim(IsNull(u2.ManagerOUPath,'')))
---

	DROP TABLE IF EXISTS [##tempUsers]
	DROP TABLE IF EXISTS [##tmpAD2]
	DROP TABLE IF EXISTS [##tmpAD]

END
GO


