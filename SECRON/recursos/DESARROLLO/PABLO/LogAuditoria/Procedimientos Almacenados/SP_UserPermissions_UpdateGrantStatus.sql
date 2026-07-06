-- Método original no recibe usuario, por eso no lleva @ctx
CREATE OR ALTER PROCEDURE SP_UserPermissions_UpdateGrantStatus
    @UserPermissionId INT, @IsGranted BIT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE UserPermissions SET IsGranted = @IsGranted, GrantedDate = GETDATE()
        WHERE UserPermissionId = @UserPermissionId;
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO