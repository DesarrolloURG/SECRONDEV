CREATE OR ALTER PROCEDURE SP_UserPermissions_Insert
    @UserId INT, @PermissionId INT, @IsGranted BIT, @GrantedBy INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@GrantedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO UserPermissions (UserId, PermissionId, IsGranted, GrantedBy)
        VALUES (@UserId, @PermissionId, @IsGranted, @GrantedBy);
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO