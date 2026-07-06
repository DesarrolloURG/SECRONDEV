-- @IsInactivation = 1 => solo IsActive=0 (InactivarPlantilla)
-- @IsInactivation = 0 => update normal (ActualizarPlantilla)
CREATE OR ALTER PROCEDURE SP_ItemStockTemplates_Update
    @TemplateId INT, @IsInactivation BIT,
    @MinimumStock DECIMAL(18,2) = NULL, @MaximumStock DECIMAL(18,2) = NULL,
    @ReorderPoint DECIMAL(18,2) = NULL, @ModifiedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        IF @IsInactivation = 1
            UPDATE ItemStockTemplates SET IsActive = 0 WHERE TemplateId = @TemplateId;
        ELSE
            UPDATE ItemStockTemplates SET
                MinimumStock = @MinimumStock, MaximumStock = @MaximumStock,
                ReorderPoint = @ReorderPoint, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
            WHERE TemplateId = @TemplateId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO