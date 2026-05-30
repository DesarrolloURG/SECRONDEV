CREATE OR ALTER PROCEDURE SP_FixedAssets_Update
    @AssetId                INT,
    @AssetCode              VARCHAR(30),
    @AssetName              VARCHAR(150),
    @Description            VARCHAR(500)  = NULL,
    @AssetCategoryId        INT,
    @PurchaseDate           DATE          = NULL,
    @PurchaseValue          DECIMAL(18,2),
    @ResidualValue          DECIMAL(18,2) = 0,
    @InvoiceNumber          VARCHAR(50)   = NULL,
    @SupplierId             INT           = NULL,
    @WarrantyDocumentPath   VARCHAR(500)  = NULL,
    @WarrantyExpirationDate DATE          = NULL,
    @DepreciationStartDate  DATE          = NULL,
    @CurrentWarehouseId     INT           = NULL,
    @AssignedToEmployeeId   INT           = NULL,
    @AssetStatus            VARCHAR(30),
    @DisposalDate           DATE          = NULL,
    @DisposalReason         VARCHAR(255)  = NULL,
    @DisposalValue          DECIMAL(18,2) = NULL,
    @Notes                  VARCHAR(1000) = NULL,
    @IsActive               BIT,
    @ModifiedBy             INT           = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM FixedAssets WHERE AssetId = @AssetId)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -1
            RETURN
        END

        IF EXISTS (SELECT 1 FROM FixedAssets WHERE AssetCode = UPPER(@AssetCode) AND AssetId != @AssetId)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -2
            RETURN
        END

        IF NOT EXISTS (SELECT 1 FROM FixedAssetCategories WHERE AssetCategoryId = @AssetCategoryId AND IsActive = 1)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -3
            RETURN
        END

        DECLARE @CategoriaAnterior INT
        SELECT @CategoriaAnterior = AssetCategoryId FROM FixedAssets WHERE AssetId = @AssetId

        IF @CategoriaAnterior != @AssetCategoryId
        BEGIN
            DELETE av
            FROM FixedAssetAttributeValues av
            INNER JOIN FixedAssetAttributeDefinitions ad ON av.AttributeDefId = ad.AttributeDefId
            WHERE av.AssetId = @AssetId
        END

        UPDATE FixedAssets SET
            AssetCode              = UPPER(@AssetCode),
            AssetName              = UPPER(@AssetName),
            Description            = UPPER(@Description),
            AssetCategoryId        = @AssetCategoryId,
            PurchaseDate           = @PurchaseDate,
            PurchaseValue          = @PurchaseValue,
            ResidualValue          = @ResidualValue,
            InvoiceNumber          = UPPER(@InvoiceNumber),
            SupplierId             = @SupplierId,
            WarrantyDocumentPath   = @WarrantyDocumentPath,
            WarrantyExpirationDate = @WarrantyExpirationDate,
            DepreciationStartDate  = @DepreciationStartDate,
            CurrentWarehouseId     = @CurrentWarehouseId,
            AssignedToEmployeeId   = @AssignedToEmployeeId,
            AssetStatus            = @AssetStatus,
            DisposalDate           = @DisposalDate,
            DisposalReason         = UPPER(@DisposalReason),
            DisposalValue          = @DisposalValue,
            Notes                  = UPPER(@Notes),
            IsActive               = @IsActive,
            ModifiedDate           = GETDATE(),
            ModifiedBy             = @ModifiedBy
        WHERE AssetId = @AssetId

        COMMIT TRANSACTION
        SELECT 1

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        SELECT 0
    END CATCH
END
GO