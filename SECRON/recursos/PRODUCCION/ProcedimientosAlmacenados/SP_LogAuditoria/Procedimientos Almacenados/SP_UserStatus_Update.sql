-- @IsInactivation = 1 => solo IsActive=0 (InactivarEstadoUsuario)
-- @IsInactivation = 0 => update normal (ActualizarEstadoUsuario)
CREATE OR ALTER PROCEDURE SP_UserStatus_Update
    @StatusId INT, @IsInactivation BIT,
    @StatusName VARCHAR(50) = NULL, @Description VARCHAR(255) = NULL,
    @ModifiedBy INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        IF @IsInactivation = 1
            UPDATE UserStatus SET IsActive = 0 WHERE StatusId = @StatusId;
        ELSE
            UPDATE UserStatus SET StatusName = @StatusName, Description = @Description
            WHERE StatusId = @StatusId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO