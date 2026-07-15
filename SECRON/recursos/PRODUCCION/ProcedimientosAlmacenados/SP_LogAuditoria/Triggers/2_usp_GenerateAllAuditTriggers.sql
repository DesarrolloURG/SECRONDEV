-- =============================================
-- SP: usp_GenerateAllAuditTriggers
-- Genera triggers para todas las tablas habilitadas en AuditConfig
-- =============================================
CREATE OR ALTER PROCEDURE dbo.usp_GenerateAllAuditTriggers
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TableName NVARCHAR(128);
    DECLARE cur CURSOR LOCAL STATIC FOR
        SELECT TableName FROM dbo.AuditConfig WHERE IsEnabled = 1;

    OPEN cur;
    FETCH NEXT FROM cur INTO @TableName;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        BEGIN TRY
            EXEC dbo.usp_GenerateAuditTrigger @TableName;
            PRINT 'OK: ' + @TableName;
        END TRY
        BEGIN CATCH
            PRINT 'ERROR: ' + @TableName + ' -> ' + ERROR_MESSAGE();
        END CATCH

        FETCH NEXT FROM cur INTO @TableName;
    END

    CLOSE cur;
    DEALLOCATE cur;
END;
GO



EXEC dbo.usp_GenerateAllAuditTriggers;
GO