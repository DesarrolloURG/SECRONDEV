-- =============================================
-- SP_Location_Update
-- =============================================
CREATE OR ALTER PROCEDURE SP_Location_Update
    @LocationId          INT,
    @LocationCode        VARCHAR(20),
    @LocationName        VARCHAR(100),
    @Address             VARCHAR(255) = NULL,
    @City                VARCHAR(100) = NULL,
    @LocationCategoryId  INT          = NULL,
    @PrimaryWarehouseId  INT          = NULL,
    @MunicipalityId      INT          = NULL,
    @ModifiedBy          INT          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    -- Setear CONTEXT_INFO para auditoría
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM Locations WHERE LocationCode = UPPER(@LocationCode) AND LocationId <> @LocationId)
        BEGIN ROLLBACK TRANSACTION; SELECT -1; RETURN; END

        UPDATE Locations
           SET LocationCode       = UPPER(@LocationCode),
               LocationName       = UPPER(@LocationName),
               Address            = UPPER(@Address),
               City               = UPPER(@City),
               LocationCategoryId = @LocationCategoryId,
               PrimaryWarehouseId = @PrimaryWarehouseId,
               MunicipalityId     = @MunicipalityId,
               ModifiedDate       = GETDATE(),
               ModifiedBy         = @ModifiedBy
         WHERE LocationId = @LocationId;

        COMMIT TRANSACTION; SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO