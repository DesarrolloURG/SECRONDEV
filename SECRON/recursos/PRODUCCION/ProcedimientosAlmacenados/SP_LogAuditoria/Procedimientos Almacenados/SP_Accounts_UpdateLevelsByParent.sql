CREATE OR ALTER PROCEDURE SP_Accounts_UpdateLevelsByParent
    @ParentAccountCode VARCHAR(20), @ParentLevel INT, @UpdatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UpdatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;
    DECLARE @UpdatedLevel INT = @ParentLevel + 1;

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE Accounts SET Level = @UpdatedLevel WHERE ParentAccountCode = @ParentAccountCode;
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO