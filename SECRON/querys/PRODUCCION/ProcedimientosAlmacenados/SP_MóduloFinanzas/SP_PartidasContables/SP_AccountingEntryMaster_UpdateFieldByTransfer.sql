CREATE OR ALTER PROCEDURE SP_AccountingEntryMaster_UpdateFieldByTransfer
    @TransferId INT, @Campo SYSNAME, @NuevoValor NVARCHAR(MAX) = NULL, @UpdatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM sys.columns
                   WHERE object_id = OBJECT_ID('AccountingEntryMaster') AND name = @Campo)
    BEGIN SELECT 0; RETURN; END

    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UpdatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    DECLARE @sql NVARCHAR(MAX) =
        N'UPDATE m SET m.' + QUOTENAME(@Campo) + N' = @NuevoValor
          FROM AccountingEntryMaster m
          INNER JOIN AccountingEntryTransfers t ON t.EntryMasterId = m.EntryMasterId
          WHERE t.TransferId = @TransferId;
          SELECT @@ROWCOUNT;';

    BEGIN TRANSACTION
    BEGIN TRY
        DECLARE @rows INT;
        DECLARE @tbl TABLE (r INT);
        INSERT INTO @tbl
        EXEC sp_executesql @sql,
             N'@NuevoValor NVARCHAR(MAX), @TransferId INT',
             @NuevoValor, @TransferId;
        SELECT @rows = r FROM @tbl;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO