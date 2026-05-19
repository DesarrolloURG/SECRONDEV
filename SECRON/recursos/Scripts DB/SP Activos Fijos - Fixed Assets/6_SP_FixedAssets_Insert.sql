-- ============================================================
-- SP_FixedAssets_Insert
-- ============================================================
CREATE OR ALTER PROCEDURE SP_FixedAssets_Insert
    @AssetName              VARCHAR(150),
    @Description            VARCHAR(500)  = NULL,
    @AssetCategoryId        INT,
    @Brand                  VARCHAR(100)  = NULL,
    @Model                  VARCHAR(100)  = NULL,
    @Serial                 VARCHAR(100)  = NULL,
    @PurchaseDate           DATE          = NULL,
    @PurchaseValue          DECIMAL(18,2),
    @ResidualValue          DECIMAL(5,2)  = 0,
    @InvoiceNumber          VARCHAR(50)   = NULL,
    @SupplierId             INT           = NULL,
    @WarrantyDocumentPath   VARCHAR(500)  = NULL,
    @WarrantyExpirationDate DATE          = NULL,
    @DepreciationStartDate  DATE          = NULL,
    @CurrentWarehouseId     INT           = NULL,
    @AssignedToEmployeeId   INT           = NULL,
    @AssetStatus            VARCHAR(30)   = 'ACTIVO',
    @DisposalDate           DATE          = NULL,
    @DisposalReason         VARCHAR(255)  = NULL,
    @DisposalValue          DECIMAL(18,2) = NULL,
    @Notes                  VARCHAR(1000) = NULL,
    @CreatedBy              INT           = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM FixedAssetCategories WHERE AssetCategoryId = @AssetCategoryId AND IsActive = 1)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -2
            RETURN
        END

        -- Generar código correlativo: CategoryCode-0000001
        DECLARE @CategoryCode VARCHAR(20)
        SELECT @CategoryCode = CategoryCode FROM FixedAssetCategories WHERE AssetCategoryId = @AssetCategoryId

        DECLARE @Correlativo INT
        SELECT @Correlativo = ISNULL(MAX(
            TRY_CAST(
                SUBSTRING(AssetCode, LEN(@CategoryCode) + 2, LEN(AssetCode))
            AS INT)
        ), 0) + 1
        FROM FixedAssets
        WHERE AssetCode LIKE @CategoryCode + '-%'

        DECLARE @AssetCode VARCHAR(30)
        SET @AssetCode = @CategoryCode + '-' + RIGHT('0000000' + CAST(@Correlativo AS VARCHAR), 7)

        INSERT INTO FixedAssets
            (AssetCode, AssetName, Description, AssetCategoryId,
             Brand, Model, Serial,
             PurchaseDate, PurchaseValue, ResidualValue,
             InvoiceNumber, SupplierId,
             WarrantyDocumentPath, WarrantyExpirationDate,
             DepreciationStartDate, ResidualValueAct,
             CurrentWarehouseId, AssignedToEmployeeId,
             AssetStatus, DisposalDate, DisposalReason, DisposalValue,
             Notes, IsActive, CreatedBy)
        VALUES
            (UPPER(@AssetCode), UPPER(@AssetName), UPPER(@Description), @AssetCategoryId,
             UPPER(@Brand), UPPER(@Model), UPPER(@Serial),
             @PurchaseDate, @PurchaseValue, @ResidualValue,
             UPPER(@InvoiceNumber), @SupplierId,
             @WarrantyDocumentPath, @WarrantyExpirationDate,
             @DepreciationStartDate, @PurchaseValue - @ResidualValue,
             @CurrentWarehouseId, @AssignedToEmployeeId,
             @AssetStatus, @DisposalDate, UPPER(@DisposalReason), @DisposalValue,
             UPPER(@Notes), 1, @CreatedBy)

        SELECT SCOPE_IDENTITY()
        COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        SELECT 0
    END CATCH
END
GO