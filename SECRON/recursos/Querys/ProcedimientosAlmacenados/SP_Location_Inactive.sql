-- =============================================
-- SP_Location_Inactive
-- =============================================
CREATE OR ALTER PROCEDURE SP_Location_Inactive
    @LocationId  INT,
    @ModifiedBy  INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM Locations WHERE LocationId = @LocationId AND IsActive = 1)
        BEGIN ROLLBACK TRANSACTION; SELECT -1; RETURN; END

        UPDATE Locations
           SET IsActive     = 0,
               ModifiedDate = GETDATE(),
               ModifiedBy   = @ModifiedBy
         WHERE LocationId   = @LocationId;

        COMMIT TRANSACTION; SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO