-- =============================================
-- SP_AccountingEntryMaster_UpdateFieldByCheck
-- Campo dinámico validado contra sys.columns.
-- 1 = éxito, -1 = campo inválido o CheckId sin partida, 0 = error
-- =============================================
CREATE OR ALTER PROCEDURE SP_AccountingEntryMaster_UpdateFieldByCheck
    @CheckId    INT,
    @Campo      SYSNAME,
    @NuevoValor NVARCHAR(MAX) = NULL,
    @UpdatedBy  INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UpdatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    DECLARE @sql NVARCHAR(MAX) =
        N'UPDATE m SET m.' + QUOTENAME(@Campo) + N' = @NuevoValor
          FROM AccountingEntryMaster m
          INNER JOIN AccountingEntryChecks c ON c.EntryMasterId = m.EntryMasterId
          WHERE c.CheckId = @CheckId;
          SELECT @@ROWCOUNT;';

    BEGIN TRANSACTION
    BEGIN TRY
        DECLARE @rows INT;
        DECLARE @tbl TABLE (r INT);
        INSERT INTO @tbl
        EXEC sp_executesql @sql,
             N'@NuevoValor NVARCHAR(MAX), @CheckId INT',
             @NuevoValor, @CheckId;
        SELECT @rows = r FROM @tbl;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO