CREATE OR ALTER PROCEDURE SP_AuditLog_InsertFull
    @UserId INT = NULL, @Action VARCHAR(100) = NULL, @TableName VARCHAR(100) = NULL,
    @RecordId INT = NULL, @OldValues NVARCHAR(MAX) = NULL, @NewValues NVARCHAR(MAX) = NULL,
    @ActionDate DATETIME, @IPAddress VARCHAR(50) = NULL, @UserAgent VARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UserId, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO AuditLog (UserId, Action, TableName, RecordId, OldValues, NewValues, ActionDate, IPAddress, UserAgent)
        VALUES (@UserId, @Action, @TableName, @RecordId, @OldValues, @NewValues, @ActionDate, @IPAddress, @UserAgent);
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO