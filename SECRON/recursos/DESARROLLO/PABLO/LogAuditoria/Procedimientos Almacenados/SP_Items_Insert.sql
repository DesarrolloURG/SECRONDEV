CREATE OR ALTER PROCEDURE SP_Items_Insert
    @ItemCode VARCHAR(50), @ItemName VARCHAR(150), @Description VARCHAR(255) = NULL,
    @CategoryId INT, @SubCategoryId INT, @UnitId INT,
    @MinimumStock DECIMAL(18,2), @MaximumStock DECIMAL(18,2) = NULL, @ReorderPoint DECIMAL(18,2) = NULL,
    @UnitCost DECIMAL(18,2), @LastPurchasePrice DECIMAL(18,2) = NULL,
    @HasLotControl BIT, @HasExpiryDate BIT, @IsActive BIT, @CreatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO Items
            (ItemCode, ItemName, Description, CategoryId, SubCategoryId, UnitId,
             MinimumStock, MaximumStock, ReorderPoint, UnitCost, LastPurchasePrice,
             HasLotControl, HasExpiryDate, IsActive, CreatedBy)
        VALUES
            (@ItemCode, @ItemName, @Description, @CategoryId, @SubCategoryId, @UnitId,
             @MinimumStock, @MaximumStock, @ReorderPoint, @UnitCost, @LastPurchasePrice,
             @HasLotControl, @HasExpiryDate, @IsActive, @CreatedBy);
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO