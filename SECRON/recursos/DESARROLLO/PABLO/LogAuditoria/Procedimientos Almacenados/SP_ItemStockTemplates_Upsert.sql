CREATE OR ALTER PROCEDURE SP_ItemStockTemplates_Upsert
    @LocationCategoryId INT, @ItemId INT, @MinimumStock DECIMAL(18,2),
    @MaximumStock DECIMAL(18,2), @ReorderPoint DECIMAL(18,2) = NULL, @CreatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM ItemStockTemplates
                   WHERE LocationCategoryId = @LocationCategoryId AND ItemId = @ItemId)
            UPDATE ItemStockTemplates
            SET MinimumStock = @MinimumStock, MaximumStock = @MaximumStock,
                ReorderPoint = @ReorderPoint, IsActive = 1,
                ModifiedDate = GETDATE(), ModifiedBy = @CreatedBy
            WHERE LocationCategoryId = @LocationCategoryId AND ItemId = @ItemId;
        ELSE
            INSERT INTO ItemStockTemplates
                (LocationCategoryId, ItemId, MinimumStock, MaximumStock, ReorderPoint, IsActive, CreatedBy)
            VALUES
                (@LocationCategoryId, @ItemId, @MinimumStock, @MaximumStock, @ReorderPoint, 1, @CreatedBy);

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO