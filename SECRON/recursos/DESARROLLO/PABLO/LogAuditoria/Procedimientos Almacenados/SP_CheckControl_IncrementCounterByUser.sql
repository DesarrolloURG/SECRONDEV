CREATE OR ALTER PROCEDURE SP_CheckControl_IncrementCounterByUser
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE CheckControl
        SET CurrentCounter = CurrentCounter + 1, ModifiedDate = GETDATE()
        WHERE UserId = @UserId AND IsActive = 1;
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO