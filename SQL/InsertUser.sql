CREATE PROCEDURE InsertUser
    @UserName NVARCHAR(50),
    @Email NVARCHAR(100),
    @NewUserID INT OUTPUT
AS
BEGIN
    INSERT INTO Users (UserName, Email)
    VALUES (@UserName, @Email)
    
    SET @NewUserID = SCOPE_IDENTITY()
END
GO