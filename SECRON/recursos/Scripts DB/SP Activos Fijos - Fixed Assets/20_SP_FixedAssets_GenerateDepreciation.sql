CREATE OR ALTER PROCEDURE SP_FixedAssets_GenerateDepreciation
    @ExecutionDate  DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT OFF;

    IF @ExecutionDate IS NULL
        SET @ExecutionDate = CAST(GETDATE() AS DATE)

    DECLARE @CurrentFirstDay DATE
    SET @CurrentFirstDay = DATEFROMPARTS(YEAR(@ExecutionDate), MONTH(@ExecutionDate), 1)

    DECLARE @StatusAprobado INT
    SELECT @StatusAprobado = StatusId
    FROM AccountingFixedAssetStatus
    WHERE StatusCode = 'APROBADO'

    DECLARE @AssetId            INT
    DECLARE @AssetCode          VARCHAR(30)
    DECLARE @AssetName          VARCHAR(150)
    DECLARE @PurchaseValue      DECIMAL(18,2)
    DECLARE @ResidualValue      DECIMAL(18,2)
    DECLARE @ResidualValueAct   DECIMAL(18,2)
    DECLARE @DepreciationYears  DECIMAL(4,1)
    DECLARE @DepreciationMethod VARCHAR(30)
    DECLARE @DepStartDate       DATE
    DECLARE @AccountAccumDepId  INT
    DECLARE @AccountExpenseId   INT

    DECLARE @PeriodFirstDay     DATE
    DECLARE @Period             VARCHAR(7)
    DECLARE @DepMensual         DECIMAL(18,2)
    DECLARE @DepMensualBase     DECIMAL(18,2)
    DECLARE @NewEntryMasterId   INT
    DECLARE @ResidualValueTemp  DECIMAL(18,2)

    DECLARE cur CURSOR FOR
        SELECT
            fa.AssetId,
            fa.AssetCode,
            fa.AssetName,
            fa.PurchaseValue,
            fa.ResidualValue,
            fa.ResidualValueAct,
            fc.DepreciationYears,
            fc.DepreciationMethod,
            fa.DepreciationStartDate,
            fc.AccountAccumDepId,
            fc.AccountExpenseId
        FROM FixedAssets fa
        INNER JOIN FixedAssetCategories fc ON fa.AssetCategoryId = fc.AssetCategoryId
        WHERE fa.IsActive = 1
          AND fa.AssetStatus = 'ACTIVO'
          AND fc.DepreciationMethod = 'LINEA_RECTA'
          AND fa.DepreciationStartDate IS NOT NULL
          AND fa.DepreciationStartDate <= @CurrentFirstDay
          AND fa.ResidualValueAct > fa.ResidualValue
          AND EXISTS (
              SELECT 1
              FROM (
                  SELECT FORMAT(DATEADD(MONTH, n, DATEFROMPARTS(
                      YEAR(fa.DepreciationStartDate),
                      MONTH(fa.DepreciationStartDate), 1)), 'yyyy-MM') AS PeriodExpected
                  FROM (
                      SELECT TOP (DATEDIFF(MONTH, fa.DepreciationStartDate, @CurrentFirstDay))
                             ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
                      FROM sys.objects
                  ) nums
              ) esperados
              WHERE NOT EXISTS (
                  SELECT 1 FROM AccountingEntryFixedAssets aefa
                  WHERE aefa.AssetId      = fa.AssetId
                    AND aefa.Period       = esperados.PeriodExpected
                    AND aefa.MovementType = 'DEPRECIACION'
              )
          )

    OPEN cur
    FETCH NEXT FROM cur INTO
        @AssetId, @AssetCode, @AssetName,
        @PurchaseValue, @ResidualValue, @ResidualValueAct,
        @DepreciationYears, @DepreciationMethod, @DepStartDate,
        @AccountAccumDepId, @AccountExpenseId

    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF @DepreciationMethod = 'LINEA_RECTA'
        BEGIN
            SET @DepMensualBase = ROUND(@PurchaseValue / (@DepreciationYears * 12), 2)
        END
        -- AQUÍ SE AGREGARÁN OTROS MÉTODOS EN EL FUTURO

        SET @ResidualValueTemp = @ResidualValueAct

        SET @PeriodFirstDay = DATEADD(MONTH, 1, DATEFROMPARTS(
            YEAR(@DepStartDate), MONTH(@DepStartDate), 1))

        WHILE @PeriodFirstDay <= @CurrentFirstDay AND @ResidualValueTemp > @ResidualValue
        BEGIN
            SET @Period = FORMAT(@PeriodFirstDay, 'yyyy-MM')

            IF NOT EXISTS (
                SELECT 1 FROM AccountingEntryFixedAssets
                WHERE AssetId      = @AssetId
                  AND Period       = @Period
                  AND MovementType = 'DEPRECIACION'
            )
            BEGIN
                BEGIN TRANSACTION
                BEGIN TRY

                    SET @DepMensual = @DepMensualBase

                    -- Último período: usar saldo exacto para absorber diferencias de redondeo
                    IF @ResidualValueTemp - @DepMensualBase <= @DepMensualBase
                        SET @DepMensual = @ResidualValueTemp - @ResidualValue

                    -- Validación de seguridad: nunca bajar del valor residual
                    IF @ResidualValueTemp - @DepMensual < @ResidualValue
                        SET @DepMensual = @ResidualValueTemp - @ResidualValue

                    INSERT INTO AccountingEntryFixedAssets
                        (AssetId, EntryDate, Period, MovementType, Concept,
                         TotalAmount, StatusId, IsActive, CreatedDate)
                    VALUES
                        (@AssetId, @PeriodFirstDay, @Period, 'DEPRECIACION',
                         'DEPRECIACIÓN MENSUAL — ' + UPPER(@AssetCode) + ' — ' + UPPER(@AssetName),
                         @DepMensual, @StatusAprobado, 1, GETDATE())

                    SET @NewEntryMasterId = SCOPE_IDENTITY()

                    -- DEBE: Gasto por Depreciación
                    INSERT INTO AccountingEntryFixedAssetsDetail
                        (EntryMasterId, AccountId, Debit, Credit, Remarks)
                    VALUES
                        (@NewEntryMasterId, @AccountExpenseId, @DepMensual, 0,
                         'Gasto depreciación ' + @Period)

                    -- HABER: Depreciación Acumulada
                    INSERT INTO AccountingEntryFixedAssetsDetail
                        (EntryMasterId, AccountId, Debit, Credit, Remarks)
                    VALUES
                        (@NewEntryMasterId, @AccountAccumDepId, 0, @DepMensual,
                         'Depreciación acumulada ' + @Period)

                    UPDATE FixedAssets
                    SET ResidualValueAct = ResidualValueAct - @DepMensual,
                        ModifiedDate     = GETDATE()
                    WHERE AssetId = @AssetId

                    SET @ResidualValueTemp = @ResidualValueTemp - @DepMensual

                    COMMIT TRANSACTION

                END TRY
                BEGIN CATCH
                    IF @@TRANCOUNT > 0
                        ROLLBACK TRANSACTION
                END CATCH
            END
            ELSE
            BEGIN
                SET @ResidualValueTemp = @ResidualValueTemp - @DepMensualBase
                IF @ResidualValueTemp < @ResidualValue
                    SET @ResidualValueTemp = @ResidualValue
            END

            SET @PeriodFirstDay = DATEADD(MONTH, 1, @PeriodFirstDay)
        END

        FETCH NEXT FROM cur INTO
            @AssetId, @AssetCode, @AssetName,
            @PurchaseValue, @ResidualValue, @ResidualValueAct,
            @DepreciationYears, @DepreciationMethod, @DepStartDate,
            @AccountAccumDepId, @AccountExpenseId
    END

    CLOSE cur
    DEALLOCATE cur

    SELECT 1
END
GO