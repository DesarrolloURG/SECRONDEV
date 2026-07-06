CREATE OR ALTER PROCEDURE SP_AccountingEntryMaster_Insert
    @EntryDate DATETIME, @Concept VARCHAR(255), @StatusId INT,
    @TotalAmount DECIMAL(18,2), @CreatedBy INT, @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO AccountingEntryMaster (EntryDate, Concept, StatusId, TotalAmount, CreatedBy, IsActive)
        VALUES (@EntryDate, @Concept, @StatusId, @TotalAmount, @CreatedBy, @IsActive);

        DECLARE @NewId INT = CAST(SCOPE_IDENTITY() AS INT);
        COMMIT TRANSACTION; SELECT @NewId;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO