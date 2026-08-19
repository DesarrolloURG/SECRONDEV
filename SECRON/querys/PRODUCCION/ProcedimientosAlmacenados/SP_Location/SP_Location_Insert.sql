-- =============================================
-- SP_Location_Insert
-- =============================================
CREATE OR ALTER PROCEDURE SP_Location_Insert
    @LocationCode        VARCHAR(20),
    @LocationName        VARCHAR(100),
    @Address             VARCHAR(255) = NULL,
    @City                VARCHAR(100) = NULL,
    @LocationCategoryId  INT          = NULL,
    @PrimaryWarehouseId  INT          = NULL,
    @MunicipalityId      INT          = NULL,
    @CreatedBy           INT          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    -- Setear CONTEXT_INFO para auditoría
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM Locations WHERE LocationCode = UPPER(@LocationCode))
        BEGIN ROLLBACK TRANSACTION; SELECT -1; RETURN; END

        INSERT INTO Locations
            (LocationCode, LocationName, Address, City,
             LocationCategoryId, PrimaryWarehouseId, MunicipalityId,
             IsActive, CreatedDate, CreatedBy)
        VALUES
            (UPPER(@LocationCode), UPPER(@LocationName), UPPER(@Address), UPPER(@City),
             @LocationCategoryId, @PrimaryWarehouseId, @MunicipalityId,
             1, GETDATE(), @CreatedBy);

        COMMIT TRANSACTION; SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO