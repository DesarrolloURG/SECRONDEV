CREATE OR ALTER PROCEDURE SP_RolePermissions_AssignMultiple
    @RoleId INT, @PermissionIdsJson NVARCHAR(MAX), @CreatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        DELETE FROM RolePermissions WHERE RoleId = @RoleId;

        INSERT INTO RolePermissions (RoleId, PermissionId, IsGranted, CreatedBy)
        SELECT @RoleId, Id, 1, @CreatedBy
        FROM OPENJSON(@PermissionIdsJson) WITH (Id INT '$');

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO