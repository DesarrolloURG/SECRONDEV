-- ============================================================
-- SP_FixedAssets_Insert
-- ============================================================
CREATE OR ALTER PROCEDURE SP_FixedAssets_Insert
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
    @AssetStatus            VARCHAR(30)   = 'ACTIVO',
    @DisposalDate           DATE          = NULL,
    @DisposalReason         VARCHAR(255)  = NULL,
    @DisposalValue          DECIMAL(18,2) = NULL,
    @Notes                  VARCHAR(1000) = NULL,
    @Brand                  VARCHAR(100)  = NULL,
    @Model                  VARCHAR(100)  = NULL,
    @Serial                 VARCHAR(100)  = NULL,
    @CreatedBy              INT           = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRANSACTION
    BEGIN TRY
 
        IF NOT EXISTS (SELECT 1 FROM FixedAssetCategories WHERE AssetCategoryId = @AssetCategoryId AND IsActive = 1)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -2
            RETURN
        END
 
        -- Generar código correlativo: CategoryCode-000001
        DECLARE @CategoryCode VARCHAR(20)
        SELECT @CategoryCode = CategoryCode FROM FixedAssetCategories WHERE AssetCategoryId = @AssetCategoryId
 
        DECLARE @Correlativo INT
        SELECT @Correlativo = ISNULL(MAX(
            TRY_CAST(SUBSTRING(AssetCode, LEN(@CategoryCode) + 2, LEN(AssetCode)) AS INT)
        ), 0) + 1
        FROM FixedAssets
        WHERE AssetCode LIKE @CategoryCode + '-%'
 
        DECLARE @AssetCode VARCHAR(30)
        SET @AssetCode = @CategoryCode + '-' + RIGHT('000000' + CAST(@Correlativo AS VARCHAR), 6)
 
        INSERT INTO FixedAssets
            (AssetCode, AssetName, Description, AssetCategoryId,
             PurchaseDate, PurchaseValue, ResidualValue,
             InvoiceNumber, SupplierId,
             WarrantyDocumentPath, WarrantyExpirationDate,
             DepreciationStartDate, ResidualValueAct,
             CurrentWarehouseId, AssignedToEmployeeId,
             AssetStatus, DisposalDate, DisposalReason, DisposalValue,
             Notes, IsActive, CreatedBy)
        VALUES
            (UPPER(@AssetCode), UPPER(@AssetName), UPPER(@Description), @AssetCategoryId,
             @PurchaseDate, @PurchaseValue, @ResidualValue,
             UPPER(@InvoiceNumber), @SupplierId,
             @WarrantyDocumentPath, @WarrantyExpirationDate,
             @DepreciationStartDate, @PurchaseValue - @ResidualValue,
             @CurrentWarehouseId, @AssignedToEmployeeId,
             @AssetStatus, @DisposalDate, UPPER(@DisposalReason), @DisposalValue,
             UPPER(@Notes), 1, @CreatedBy)
 
        DECLARE @NewAssetId INT = SCOPE_IDENTITY()
 
        DECLARE @AttrDefId INT
 
        EXEC SP_FA_ObtenerOCrearAtributoSistema @AssetCategoryId, 'BRAND',  'Marca',  @AttrDefId OUTPUT
        EXEC SP_FixedAssetAttributeValues_Insert @NewAssetId, @AttrDefId, @Brand,  @CreatedBy
        SET @AttrDefId = NULL
 
        EXEC SP_FA_ObtenerOCrearAtributoSistema @AssetCategoryId, 'MODEL',  'Modelo', @AttrDefId OUTPUT
        EXEC SP_FixedAssetAttributeValues_Insert @NewAssetId, @AttrDefId, @Model,  @CreatedBy
        SET @AttrDefId = NULL
 
        EXEC SP_FA_ObtenerOCrearAtributoSistema @AssetCategoryId, 'SERIAL', 'Serie',  @AttrDefId OUTPUT
        EXEC SP_FixedAssetAttributeValues_Insert @NewAssetId, @AttrDefId, @Serial, @CreatedBy
        SET @AttrDefId = NULL
 
        SELECT @NewAssetId
        COMMIT TRANSACTION
 
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        SELECT 0
    END CATCH
END
GO