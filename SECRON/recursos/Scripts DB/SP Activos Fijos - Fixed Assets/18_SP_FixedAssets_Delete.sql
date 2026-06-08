CREATE OR ALTER PROCEDURE SP_FixedAssets_Delete
    @AssetId INT
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

        DECLARE @AccountExpenseId  INT
        DECLARE @AccountAccumDepId INT
        DECLARE @TotalExpense      DECIMAL(18,2)
        DECLARE @TotalAccumDep     DECIMAL(18,2)
        DECLARE @SignoExpense       VARCHAR(1)
        DECLARE @SignoAccumDep      VARCHAR(1)

        -- Obtener cuentas de la categoría del activo
        SELECT
            @AccountExpenseId  = fc.AccountExpenseId,
            @AccountAccumDepId = fc.AccountAccumDepId
        FROM FixedAssets fa
        INNER JOIN FixedAssetCategories fc ON fa.AssetCategoryId = fc.AssetCategoryId
        WHERE fa.AssetId = @AssetId

        -- Obtener signos
        SELECT @SignoExpense  = Sign FROM Accounts WHERE AccountId = @AccountExpenseId
        SELECT @SignoAccumDep = Sign FROM Accounts WHERE AccountId = @AccountAccumDepId

        -- Sumar montos de gasto (Debit)
        SELECT @TotalExpense = ISNULL(SUM(aed.Debit), 0)
        FROM AccountingEntryFixedAssetsDetail aed
        INNER JOIN AccountingEntryFixedAssets ae ON aed.EntryMasterId = ae.EntryMasterId
        WHERE ae.AssetId      = @AssetId
          AND ae.MovementType = 'DEPRECIACION'
          AND aed.AccountId   = @AccountExpenseId

        -- Sumar montos de acumulada (Credit)
        SELECT @TotalAccumDep = ISNULL(SUM(aed.Credit), 0)
        FROM AccountingEntryFixedAssetsDetail aed
        INNER JOIN AccountingEntryFixedAssets ae ON aed.EntryMasterId = ae.EntryMasterId
        WHERE ae.AssetId      = @AssetId
          AND ae.MovementType = 'DEPRECIACION'
          AND aed.AccountId   = @AccountAccumDepId

        -- Revertir saldo Gasto Depreciación
        UPDATE Accounts
        SET Balance = Balance - CASE WHEN @SignoExpense = '+' THEN @TotalExpense ELSE -@TotalExpense END
        WHERE AccountId = @AccountExpenseId

        -- Revertir saldo Depreciación Acumulada
        UPDATE Accounts
        SET Balance = Balance - CASE WHEN @SignoAccumDep = '-' THEN @TotalAccumDep ELSE -@TotalAccumDep END
        WHERE AccountId = @AccountAccumDepId

        -- 1. Eliminar detalle de partidas contables
        DELETE aed
        FROM AccountingEntryFixedAssetsDetail aed
        INNER JOIN AccountingEntryFixedAssets ae ON aed.EntryMasterId = ae.EntryMasterId
        WHERE ae.AssetId = @AssetId

        -- 2. Eliminar partidas contables maestro
        DELETE FROM AccountingEntryFixedAssets
        WHERE AssetId = @AssetId

        -- 3. Eliminar detalle de traslados
        DELETE FROM FixedAssetTransferDetails
        WHERE AssetId = @AssetId

        -- 4. Eliminar valores de atributos
        DELETE FROM FixedAssetAttributeValues
        WHERE AssetId = @AssetId

        -- 5. Eliminar el activo
        DELETE FROM FixedAssets
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