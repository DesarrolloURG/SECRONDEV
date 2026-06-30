-- =============================================
-- SP_ItemMovement_Insert
-- Maneja COMPRA, ENTRADA_AJUSTE, SALIDA, SALIDA_AJUSTE,
-- TRANSFERENCIA (resta origen / suma destino), DEVOLUCION
-- Detalle de items recibido como JSON
-- =============================================
CREATE OR ALTER PROCEDURE SP_ItemMovement_Insert
    @MovementTypeId         INT,
    @WarehouseId             INT,
    @DestinationWarehouseId  INT = NULL,
    @SupplierId              INT = NULL,
    @ReferenceDocument       VARCHAR(100) = NULL,
    @PurchaseDate            DATE = NULL,
    @Remarks                 VARCHAR(MAX) = NULL,
    @ItemsJson               NVARCHAR(MAX),
    @CreatedBy               INT = NULL,
    @DestinationEmployeeId   INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        DECLARE @AffectsStock VARCHAR(10);
        DECLARE @RequiresDestination BIT;

        SELECT @AffectsStock = AffectsStock, @RequiresDestination = RequiresDestination
          FROM MovementTypes
         WHERE MovementTypeId = @MovementTypeId AND IsActive = 1;

        IF @AffectsStock IS NULL
        BEGIN ROLLBACK TRANSACTION; SELECT -1; RETURN; END

        IF @RequiresDestination = 1 AND @DestinationWarehouseId IS NULL
        BEGIN ROLLBACK TRANSACTION; SELECT -2; RETURN; END

        IF @RequiresDestination = 1 AND @DestinationWarehouseId = @WarehouseId
        BEGIN ROLLBACK TRANSACTION; SELECT -3; RETURN; END

        IF NOT EXISTS (SELECT 1 FROM OPENJSON(@ItemsJson))
        BEGIN ROLLBACK TRANSACTION; SELECT -4; RETURN; END

        DECLARE @NextNumber INT;
        SELECT @NextNumber = ISNULL(MAX(CAST(SUBSTRING(MovementNumber, 5, 10) AS INT)), 0) + 1
          FROM ItemMovementMaster;

        DECLARE @MovementNumber VARCHAR(30) = 'MOV-' + RIGHT('000000' + CAST(@NextNumber AS VARCHAR(6)), 6);

        DECLARE @Items TABLE (
            ItemId      INT,
            Quantity    DECIMAL(18,2),
            UnitCost    DECIMAL(15,2),
            LotNumber   VARCHAR(50),
            ExpiryDate  DATE,
            Remarks     VARCHAR(500)
        );

        INSERT INTO @Items (ItemId, Quantity, UnitCost, LotNumber, ExpiryDate, Remarks)
        SELECT ItemId, Quantity, UnitCost, LotNumber, ExpiryDate, Remarks
          FROM OPENJSON(@ItemsJson)
          WITH (
              ItemId     INT           '$.ItemId',
              Quantity   DECIMAL(18,2) '$.Quantity',
              UnitCost   DECIMAL(15,2) '$.UnitCost',
              LotNumber  VARCHAR(50)   '$.LotNumber',
              ExpiryDate DATE          '$.ExpiryDate',
              Remarks    VARCHAR(500)  '$.Remarks'
          );

        IF EXISTS (SELECT 1 FROM @Items WHERE Quantity IS NULL OR Quantity <= 0)
        BEGIN ROLLBACK TRANSACTION; SELECT -5; RETURN; END

        DECLARE @TotalAmount DECIMAL(15,2) = (SELECT SUM(ISNULL(Quantity,0) * ISNULL(UnitCost,0)) FROM @Items);

        INSERT INTO ItemMovementMaster
            (MovementNumber, MovementDate, MovementTypeId, WarehouseId, DestinationWarehouseId,
             SupplierId, ReferenceDocument, PurchaseDate, DestinationEmployeeId, Remarks, TotalAmount,
             CreatedDate, CreatedBy, IsActive)
        VALUES
            (@MovementNumber, GETDATE(), @MovementTypeId, @WarehouseId, @DestinationWarehouseId,
             @SupplierId, @ReferenceDocument, @PurchaseDate, @DestinationEmployeeId, @Remarks, @TotalAmount,
             GETDATE(), @CreatedBy, 1);

        DECLARE @MovementMasterId INT = SCOPE_IDENTITY();

        DECLARE @ItemId INT, @Quantity DECIMAL(18,2), @UnitCost DECIMAL(15,2),
                @LotNumber VARCHAR(50), @ExpiryDate DATE, @ItemRemarks VARCHAR(500),
                @StockBefore DECIMAL(18,2), @StockAfter DECIMAL(18,2),
                @DefMin DECIMAL(18,2), @DefMax DECIMAL(18,2), @DefReorder DECIMAL(18,2);

        DECLARE item_cursor CURSOR LOCAL FAST_FORWARD FOR
            SELECT ItemId, Quantity, UnitCost, LotNumber, ExpiryDate, Remarks FROM @Items;

        OPEN item_cursor;
        FETCH NEXT FROM item_cursor INTO @ItemId, @Quantity, @UnitCost, @LotNumber, @ExpiryDate, @ItemRemarks;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- Asegurar registro de stock en bodega origen, heredando defaults de Items si es nuevo
            IF NOT EXISTS (SELECT 1 FROM ItemWarehouseStock WHERE ItemId = @ItemId AND WarehouseId = @WarehouseId)
            BEGIN
                SELECT @DefMin = MinimumStock, @DefMax = MaximumStock, @DefReorder = ReorderPoint
                  FROM Items WHERE ItemId = @ItemId;

                INSERT INTO ItemWarehouseStock (ItemId, WarehouseId, CurrentStock, MinimumStock, MaximumStock, ReorderPoint, MovementCounter, CreatedDate)
                VALUES (@ItemId, @WarehouseId, 0, @DefMin, @DefMax, @DefReorder, 0, GETDATE());
            END

            SELECT @StockBefore = CurrentStock FROM ItemWarehouseStock WHERE ItemId = @ItemId AND WarehouseId = @WarehouseId;

            IF @AffectsStock = '-' OR (@AffectsStock = '0' AND @RequiresDestination = 1)
            BEGIN
                IF @StockBefore < @Quantity
                BEGIN
                    CLOSE item_cursor; DEALLOCATE item_cursor;
                    ROLLBACK TRANSACTION; SELECT -6; RETURN;
                END
                SET @StockAfter = @StockBefore - @Quantity;
            END
            ELSE IF @AffectsStock = '+'
            BEGIN
                SET @StockAfter = @StockBefore + @Quantity;
            END
            ELSE
                SET @StockAfter = @StockBefore;

            UPDATE ItemWarehouseStock
               SET CurrentStock = @StockAfter,
                   MovementCounter = MovementCounter + 1,
                   LastMovementDate = GETDATE(),
                   ModifiedDate = GETDATE()
             WHERE ItemId = @ItemId AND WarehouseId = @WarehouseId;

            -- Si es transferencia, sumar en destino, heredando defaults de Items si es nuevo
            IF @AffectsStock = '0' AND @RequiresDestination = 1
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM ItemWarehouseStock WHERE ItemId = @ItemId AND WarehouseId = @DestinationWarehouseId)
                BEGIN
                    SELECT @DefMin = MinimumStock, @DefMax = MaximumStock, @DefReorder = ReorderPoint
                      FROM Items WHERE ItemId = @ItemId;

                    INSERT INTO ItemWarehouseStock (ItemId, WarehouseId, CurrentStock, MinimumStock, MaximumStock, ReorderPoint, MovementCounter, CreatedDate)
                    VALUES (@ItemId, @DestinationWarehouseId, 0, @DefMin, @DefMax, @DefReorder, 0, GETDATE());
                END

                UPDATE ItemWarehouseStock
                   SET CurrentStock = CurrentStock + @Quantity,
                       MovementCounter = MovementCounter + 1,
                       LastMovementDate = GETDATE(),
                       ModifiedDate = GETDATE()
                 WHERE ItemId = @ItemId AND WarehouseId = @DestinationWarehouseId;
            END

            INSERT INTO ItemMovementDetails
                (MovementMasterId, ItemId, Quantity, UnitCost, StockBeforeMovement, StockAfterMovement,
                 LotNumber, ExpiryDate, Remarks)
            VALUES
                (@MovementMasterId, @ItemId, @Quantity, @UnitCost, @StockBefore, @StockAfter,
                 @LotNumber, @ExpiryDate, @ItemRemarks);

            FETCH NEXT FROM item_cursor INTO @ItemId, @Quantity, @UnitCost, @LotNumber, @ExpiryDate, @ItemRemarks;
        END

        CLOSE item_cursor; DEALLOCATE item_cursor;

        COMMIT TRANSACTION; SELECT 1;
    END TRY
    BEGIN CATCH
        IF CURSOR_STATUS('local','item_cursor') >= -1
        BEGIN CLOSE item_cursor; DEALLOCATE item_cursor; END
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO