CREATE OR ALTER PROCEDURE SP_Permissions_Insert
    @PermissionCode VARCHAR(50), @PermissionName VARCHAR(150), @Description VARCHAR(255) = NULL,
    @ModuleName VARCHAR(100) = NULL, @ActionType VARCHAR(50) = NULL, @IsActive BIT, @CreatedBy INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO Permissions (PermissionCode, PermissionName, Description, ModuleName, ActionType, IsActive)
        VALUES (@PermissionCode, @PermissionName, @Description, @ModuleName, @ActionType, @IsActive);
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO