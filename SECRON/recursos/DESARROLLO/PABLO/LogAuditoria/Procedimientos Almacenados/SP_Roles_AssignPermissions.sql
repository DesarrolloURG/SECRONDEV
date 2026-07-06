CREATE OR ALTER PROCEDURE SP_Roles_AssignPermissions
    @RoleId INT, @PermissionIdsJson NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;

    BEGIN TRANSACTION
    BEGIN TRY
        DELETE FROM RolePermissions WHERE RoleId = @RoleId;

        INSERT INTO RolePermissions (RoleId, PermissionId)
        SELECT @RoleId, Id FROM OPENJSON(@PermissionIdsJson) WITH (Id INT '$');

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO