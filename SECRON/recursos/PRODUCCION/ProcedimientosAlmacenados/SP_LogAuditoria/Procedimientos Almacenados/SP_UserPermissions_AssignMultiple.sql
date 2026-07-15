CREATE OR ALTER PROCEDURE SP_UserPermissions_AssignMultiple
    @UserId INT, @PermissionsJson NVARCHAR(MAX), @GrantedBy INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@GrantedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        DELETE FROM UserPermissions WHERE UserId = @UserId;

        INSERT INTO UserPermissions (UserId, PermissionId, IsGranted, GrantedBy)
        SELECT @UserId, PermissionId, IsGranted, @GrantedBy
        FROM OPENJSON(@PermissionsJson)
        WITH (PermissionId INT '$.PermissionId', IsGranted BIT '$.IsGranted');

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO