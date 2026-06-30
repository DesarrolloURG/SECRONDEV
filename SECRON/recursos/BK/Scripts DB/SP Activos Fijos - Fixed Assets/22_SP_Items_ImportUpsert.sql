CREATE OR ALTER PROCEDURE SP_Items_ImportUpsert
    @ItemName        NVARCHAR(200),
    @Description     NVARCHAR(500)   = NULL,
    @CategoryId      INT,
    @SubCategoryId   INT             = NULL,
    @UnitId          INT,
    @MinimumStock    DECIMAL(18,2)   = 0,
    @MaximumStock    DECIMAL(18,2)   = NULL,
    @ReorderPoint    DECIMAL(18,2)   = NULL,
    @UnitCost        DECIMAL(15,2)   = NULL,
    @LastPurchasePrice DECIMAL(15,2) = NULL,
    @HasLotControl   BIT             = 0,
    @HasExpiryDate   BIT             = 0,
    @CreatedBy       INT,
    @ItemCode        NVARCHAR(50)    OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Generar el ItemCode igual que ObtenerProximoCodigoArticulo
        DECLARE @CategoryCode NVARCHAR(50);
        DECLARE @SubCategoryCode NVARCHAR(50);

        SELECT @CategoryCode = CategoryCode FROM ItemCategories WHERE CategoryId = @CategoryId;
        SELECT @SubCategoryCode = SubCategoryCode FROM ItemSubCategories WHERE SubCategoryId = @SubCategoryId;

        -- Si no hay SubCategoría, usar solo CategoryCode
        DECLARE @Prefix NVARCHAR(100) = ISNULL(@CategoryCode, '') + ISNULL(@SubCategoryCode, '');

        -- Verificar si ya existe un artículo con ese mismo Nombre + Categoría + SubCategoría
        DECLARE @ExistingItemId INT = NULL;
        DECLARE @ExistingItemCode NVARCHAR(50) = NULL;

        SELECT TOP 1
            @ExistingItemId   = ItemId,
            @ExistingItemCode = ItemCode
        FROM Items
        WHERE UPPER(ItemName) = UPPER(@ItemName)
          AND CategoryId      = @CategoryId
          AND (SubCategoryId  = @SubCategoryId OR (@SubCategoryId IS NULL AND SubCategoryId IS NULL));

        IF @ExistingItemId IS NOT NULL
        BEGIN
            -- ACTUALIZAR
            UPDATE Items SET
                Description        = ISNULL(@Description, Description),
                UnitId             = @UnitId,
                MinimumStock       = ISNULL(@MinimumStock, MinimumStock),
                MaximumStock       = ISNULL(@MaximumStock, MaximumStock),
                ReorderPoint       = ISNULL(@ReorderPoint, ReorderPoint),
                UnitCost           = ISNULL(@UnitCost, UnitCost),
                LastPurchasePrice  = ISNULL(@LastPurchasePrice, LastPurchasePrice),
                HasLotControl      = @HasLotControl,
                HasExpiryDate      = @HasExpiryDate,
                ModifiedDate       = GETDATE(),
                ModifiedBy         = @CreatedBy
            WHERE ItemId = @ExistingItemId;

            SET @ItemCode = @ExistingItemCode;
            RETURN 2; -- UPDATE
        END
        ELSE
        BEGIN
            -- Generar próximo código
            DECLARE @UltimoNumero INT = 0;
            SELECT @UltimoNumero = ISNULL(MAX(
                TRY_CAST(RIGHT(ItemCode, LEN(ItemCode) - LEN(@Prefix)) AS INT)
            ), 0)
            FROM Items
            WHERE ItemCode LIKE @Prefix + '%'
              AND CategoryId = @CategoryId;

            SET @ItemCode = @Prefix + RIGHT('000000' + CAST(@UltimoNumero + 1 AS NVARCHAR), 6);

            -- INSERTAR
            INSERT INTO Items (
                ItemCode, ItemName, Description, CategoryId, SubCategoryId, UnitId,
                MinimumStock, MaximumStock, ReorderPoint, UnitCost, LastPurchasePrice,
                HasLotControl, HasExpiryDate, IsActive, CreatedDate, CreatedBy
            ) VALUES (
                @ItemCode, UPPER(@ItemName), UPPER(@Description), @CategoryId, @SubCategoryId, @UnitId,
                ISNULL(@MinimumStock, 0), @MaximumStock, @ReorderPoint, @UnitCost, @LastPurchasePrice,
                @HasLotControl, @HasExpiryDate, 1, GETDATE(), @CreatedBy
            );

            RETURN 1; -- INSERT
        END
    END TRY
    BEGIN CATCH
        RETURN -1; -- Error general
    END CATCH
END