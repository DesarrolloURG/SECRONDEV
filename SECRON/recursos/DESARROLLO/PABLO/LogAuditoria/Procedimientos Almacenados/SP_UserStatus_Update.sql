-- @IsInactivation = 1 => solo IsActive=0 (InactivarEstadoUsuario)
-- @IsInactivation = 0 => update normal (ActualizarEstadoUsuario)
CREATE OR ALTER PROCEDURE SP_UserStatus_Update
    @StatusId INT, @IsInactivation BIT,
    @StatusName VARCHAR(50) = NULL, @Description VARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;

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