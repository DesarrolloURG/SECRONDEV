CREATE OR ALTER PROCEDURE SP_Roles_Update
    @RoleId INT, @IsInactivation BIT,
    @RoleName VARCHAR(100) = NULL, @Description VARCHAR(255) = NULL,
    @ModifiedBy INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        IF @IsInactivation = 1
            UPDATE Roles SET IsActive = 0 WHERE RoleId = @RoleId;
        ELSE
            UPDATE Roles SET RoleName = @RoleName, Description = @Description
            WHERE RoleId = @RoleId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO