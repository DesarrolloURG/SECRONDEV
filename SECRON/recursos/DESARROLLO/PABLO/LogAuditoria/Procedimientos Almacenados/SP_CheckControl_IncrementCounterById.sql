CREATE OR ALTER PROCEDURE SP_CheckControl_IncrementCounterById
    @CheckControlId INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE CheckControl
        SET CurrentCounter = CurrentCounter + 1, ModifiedDate = GETDATE()
        WHERE CheckControlId = @CheckControlId AND IsActive = 1;
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO