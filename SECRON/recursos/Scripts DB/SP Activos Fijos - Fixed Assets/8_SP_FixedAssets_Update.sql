-- ============================================================
-- SP_FixedAssets_Update
-- ============================================================
CREATE OR ALTER PROCEDURE SP_FixedAssets_Update
    @AssetId                INT,
    @AssetCode              VARCHAR(30),
    @AssetName              VARCHAR(150),
    @Description            VARCHAR(500)  = NULL,
    @AssetCategoryId        INT,
    @Brand                  VARCHAR(100)  = NULL,
    @Model                  VARCHAR(100)  = NULL,
    @Serial                 VARCHAR(100)  = NULL,
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
 
        -- Upsert atributos de sistema BRAND, MODEL, SERIAL
        DECLARE @AttrDefId INT
 
        EXEC SP_FA_ObtenerOCrearAtributoSistema @AssetCategoryId, 'BRAND',  'Marca',  @AttrDefId OUTPUT
        EXEC SP_FixedAssetAttributeValues_Insert @AssetId, @AttrDefId, @Brand,  @ModifiedBy
 
        EXEC SP_FA_ObtenerOCrearAtributoSistema @AssetCategoryId, 'MODEL',  'Modelo', @AttrDefId OUTPUT
        EXEC SP_FixedAssetAttributeValues_Insert @AssetId, @AttrDefId, @Model,  @ModifiedBy
 
        EXEC SP_FA_ObtenerOCrearAtributoSistema @AssetCategoryId, 'SERIAL', 'Serie',  @AttrDefId OUTPUT
        EXEC SP_FixedAssetAttributeValues_Insert @AssetId, @AttrDefId, @Serial, @ModifiedBy
 
        COMMIT TRANSACTION
        SELECT 1
 
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        SELECT 0
    END CATCH
END
GO