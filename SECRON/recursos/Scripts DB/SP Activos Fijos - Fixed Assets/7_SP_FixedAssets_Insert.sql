CREATE OR ALTER PROCEDURE SP_FixedAssets_Insert
    @AssetName              VARCHAR(150),
    @Description            VARCHAR(500)  = NULL,
    @AssetCategoryId        INT,
    @SubCategoryId          INT,
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
    @CreatedBy              INT           = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRANSACTION
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM FixedAssetCategories
                       WHERE AssetCategoryId = @AssetCategoryId AND IsActive = 1)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -2
            RETURN
        END

        IF NOT EXISTS (SELECT 1 FROM FixedAssetSubCategories
                       WHERE SubCategoryId   = @SubCategoryId
                         AND AssetCategoryId = @AssetCategoryId
                         AND IsActive        = 1)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -3
            RETURN
        END

        DECLARE @CategoryCode    VARCHAR(20)
        DECLARE @SubCategoryCode VARCHAR(10)

        SELECT @CategoryCode = CategoryCode
        FROM FixedAssetCategories
        WHERE AssetCategoryId = @AssetCategoryId

        SELECT @SubCategoryCode = SubCategoryCode
        FROM FixedAssetSubCategories
        WHERE SubCategoryId = @SubCategoryId

        DECLARE @Prefix VARCHAR(35)
        SET @Prefix = @CategoryCode + '-' + @SubCategoryCode + '-'

        DECLARE @Correlativo INT
        SELECT @Correlativo = ISNULL(MAX(
            TRY_CAST(SUBSTRING(AssetCode, LEN(@Prefix) + 1, LEN(AssetCode)) AS INT)
        ), 0) + 1
        FROM FixedAssets
        WHERE AssetCode LIKE @Prefix + '%'

        DECLARE @AssetCode VARCHAR(50)
        SET @AssetCode = @Prefix + RIGHT('000000' + CAST(@Correlativo AS VARCHAR), 6)

        INSERT INTO FixedAssets
            (AssetCode, AssetName, Description, AssetCategoryId, SubCategoryId,
             PurchaseDate, PurchaseValue, ResidualValue,
             InvoiceNumber, SupplierId,
             WarrantyDocumentPath, WarrantyExpirationDate,
             DepreciationStartDate, ResidualValueAct,
             CurrentWarehouseId, AssignedToEmployeeId,
             AssetStatus, DisposalDate, DisposalReason, DisposalValue,
             Notes, IsActive, CreatedBy)
        VALUES
            (UPPER(@AssetCode), UPPER(@AssetName), UPPER(@Description), @AssetCategoryId, @SubCategoryId,
             @PurchaseDate, @PurchaseValue, @ResidualValue,
             UPPER(@InvoiceNumber), @SupplierId,
             @WarrantyDocumentPath, @WarrantyExpirationDate,
             @DepreciationStartDate, @PurchaseValue,
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