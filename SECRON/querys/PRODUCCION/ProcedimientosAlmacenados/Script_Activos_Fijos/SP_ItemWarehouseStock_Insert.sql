-- ============================================================
-- SP_ItemWarehouseStock_Insert
-- Registra el stock inicial de un artículo en una bodega.
-- Retorno: 1 = éxito, -1 = ya existe la combinación Item/Warehouse, 0 = error inesperado
-- ============================================================
CREATE OR ALTER PROCEDURE SP_ItemWarehouseStock_Insert
    @ItemId         INT,
    @WarehouseId    INT,
    @CurrentStock   DECIMAL(18,2),
    @MinimumStock   DECIMAL(18,2),
    @MaximumStock   DECIMAL(18,2) = NULL,
    @ReorderPoint   DECIMAL(18,2) = NULL,
    @CreatedBy      INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Result INT;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));

    BEGIN TRY
        SET CONTEXT_INFO @ctx;

        IF EXISTS (
            SELECT 1 FROM ItemWarehouseStock
            WHERE ItemId = @ItemId AND WarehouseId = @WarehouseId
        )
        BEGIN
            SET @Result = -1;
        END
        ELSE
        BEGIN
            INSERT INTO ItemWarehouseStock
                (ItemId, WarehouseId, CurrentStock, MinimumStock, MaximumStock, ReorderPoint, MovementCounter, CreatedDate)
            VALUES
                (@ItemId, @WarehouseId, @CurrentStock, @MinimumStock, @MaximumStock, @ReorderPoint, 0, GETDATE());

            SET @Result = 1;
        END
    END TRY
    BEGIN CATCH
        SET @Result = 0;
    END CATCH

    SELECT @Result;
END