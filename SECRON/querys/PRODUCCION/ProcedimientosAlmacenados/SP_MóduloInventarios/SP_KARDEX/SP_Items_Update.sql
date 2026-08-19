-- @IsInactivation = 1 => solo IsActive=0 (InactivarArticulo)
-- @IsInactivation = 0 => update normal (ActualizarArticulo)
CREATE OR ALTER PROCEDURE SP_Items_Update
    @ItemId INT, @IsInactivation BIT,
    @ItemCode VARCHAR(50) = NULL, @ItemName VARCHAR(150) = NULL, @Description VARCHAR(255) = NULL,
    @CategoryId INT = NULL, @SubCategoryId INT = NULL, @UnitId INT = NULL,
    @MinimumStock DECIMAL(18,2) = NULL, @MaximumStock DECIMAL(18,2) = NULL, @ReorderPoint DECIMAL(18,2) = NULL,
    @UnitCost DECIMAL(18,2) = NULL, @LastPurchasePrice DECIMAL(18,2) = NULL,
    @HasLotControl BIT = NULL, @HasExpiryDate BIT = NULL, @ModifiedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        IF @IsInactivation = 1
            UPDATE Items SET IsActive = 0, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
            WHERE ItemId = @ItemId;
        ELSE
            UPDATE Items SET
                ItemCode = @ItemCode, ItemName = @ItemName, Description = @Description,
                CategoryId = @CategoryId, SubCategoryId = @SubCategoryId, UnitId = @UnitId,
                MinimumStock = @MinimumStock, MaximumStock = @MaximumStock, ReorderPoint = @ReorderPoint,
                UnitCost = @UnitCost, LastPurchasePrice = @LastPurchasePrice,
                HasLotControl = @HasLotControl, HasExpiryDate = @HasExpiryDate,
                ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
            WHERE ItemId = @ItemId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO