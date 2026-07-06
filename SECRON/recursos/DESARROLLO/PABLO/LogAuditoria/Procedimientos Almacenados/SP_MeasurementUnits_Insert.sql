CREATE OR ALTER PROCEDURE SP_MeasurementUnits_Insert
    @UnitCode VARCHAR(20), @UnitName VARCHAR(100), @Abbreviation VARCHAR(10), @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;

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