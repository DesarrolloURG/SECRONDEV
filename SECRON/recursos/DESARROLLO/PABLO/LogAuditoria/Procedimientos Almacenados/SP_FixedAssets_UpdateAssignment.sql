CREATE OR ALTER PROCEDURE SP_FixedAssets_UpdateAssignment
    @AssetId INT, @EmpleadoId INT = NULL, @BodegaId INT = NULL, @ModifiedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE FixedAssets SET
            AssignedToEmployeeId = @EmpleadoId,
            CurrentWarehouseId = @BodegaId,
            ModifiedDate = GETDATE(),
            ModifiedBy = @ModifiedBy
        WHERE AssetId = @AssetId;

        IF @@ROWCOUNT = 0
        BEGIN ROLLBACK TRANSACTION; SELECT -1; RETURN; END

        COMMIT TRANSACTION; SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO