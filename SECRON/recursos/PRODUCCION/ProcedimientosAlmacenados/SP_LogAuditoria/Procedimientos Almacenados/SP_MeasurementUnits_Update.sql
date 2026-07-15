-- @IsInactivation = 1 => solo IsActive=0 (InactivarUnidad)
-- @IsInactivation = 0 => update normal (ActualizarUnidad)
CREATE OR ALTER PROCEDURE SP_MeasurementUnits_Update
    @UnitId INT, @IsInactivation BIT,
    @UnitCode VARCHAR(20) = NULL, @UnitName VARCHAR(100) = NULL, @Abbreviation VARCHAR(10) = NULL,
    @ModifiedBy INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        IF @IsInactivation = 1
            UPDATE MeasurementUnits SET IsActive = 0 WHERE UnitId = @UnitId;
        ELSE
            UPDATE MeasurementUnits SET UnitCode = @UnitCode, UnitName = @UnitName,
                Abbreviation = @Abbreviation
            WHERE UnitId = @UnitId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO