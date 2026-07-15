CREATE OR ALTER PROCEDURE SP_MeasurementUnits_Insert
    @UnitCode VARCHAR(20), @UnitName VARCHAR(100), @Abbreviation VARCHAR(10), @IsActive BIT, @CreatedBy INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO MeasurementUnits (UnitCode, UnitName, Abbreviation, IsActive)
        VALUES (@UnitCode, @UnitName, @Abbreviation, @IsActive);
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO