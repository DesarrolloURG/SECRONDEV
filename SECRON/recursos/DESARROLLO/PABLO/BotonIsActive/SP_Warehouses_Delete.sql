CREATE OR ALTER PROCEDURE SP_Warehouses_Delete
    @WarehouseId INT,
    @IsActive    BIT,
    @ModifiedBy  INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (SELECT 1 FROM Warehouses WHERE WarehouseId = @WarehouseId)
        BEGIN
            ROLLBACK TRANSACTION;
            RETURN -1;
        END

        UPDATE Warehouses
        SET IsActive     = @IsActive,
            ModifiedBy   = @ModifiedBy,
            ModifiedDate = GETDATE()
        WHERE WarehouseId = @WarehouseId;

        COMMIT TRANSACTION;
        RETURN 1;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        RETURN 0;
    END CATCH
END