-- @IsInactivation = 1 => solo IsActive=0 (InactivarRol)
-- @IsInactivation = 0 => update normal (ActualizarRol)
-- Ninguno de los métodos originales recibe usuario, por eso no lleva @ctx
CREATE OR ALTER PROCEDURE SP_Roles_Update
    @RoleId INT, @IsInactivation BIT,
    @RoleName VARCHAR(100) = NULL, @Description VARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;

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