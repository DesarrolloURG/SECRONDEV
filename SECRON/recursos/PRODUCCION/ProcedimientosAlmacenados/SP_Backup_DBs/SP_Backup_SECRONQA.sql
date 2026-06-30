USE SECRONQA
GO

CREATE OR ALTER PROCEDURE SP_Backup_SECRONQA
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @RutaBase       NVARCHAR(500)
    DECLARE @NombreArchivo  NVARCHAR(500)
    DECLARE @RutaCompleta   NVARCHAR(500)
    DECLARE @FechaHora      NVARCHAR(50)
    DECLARE @ID_Log         INT
    DECLARE @TamanioMB      DECIMAL(10,2)
    DECLARE @Cmd            NVARCHAR(1000)

    SET @RutaBase = 'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\Backup\SECRONQA\'

    SET @FechaHora = CONVERT(NVARCHAR, GETDATE(), 112) + '_' +
                     REPLACE(CONVERT(NVARCHAR, GETDATE(), 108), ':', '')
    SET @NombreArchivo = 'SECRONQA_' + @FechaHora + '.bak'
    SET @RutaCompleta  = @RutaBase + @NombreArchivo

    INSERT INTO LOG_BACKUPS (FECHA_INICIO, ESTADO)
    VALUES (GETDATE(), 'EN PROCESO')

    SET @ID_Log = SCOPE_IDENTITY()

    BEGIN TRY

        SET @Cmd = 'mkdir "' + @RutaBase + '"'
        EXEC xp_cmdshell @Cmd, NO_OUTPUT

        BACKUP DATABASE SECRONQA
        TO DISK = @RutaCompleta
        WITH FORMAT, INIT,
             NAME = 'SECRONQA - Backup Automático'

        SELECT @TamanioMB = ROUND(size * 8.0 / 1024, 2)
        FROM sys.master_files
        WHERE name = 'SECRONQA' AND type = 0

        UPDATE LOG_BACKUPS SET
            FECHA_FIN      = GETDATE(),
            NOMBRE_ARCHIVO = @NombreArchivo,
            TAMANIO_MB     = @TamanioMB,
            ESTADO         = 'EXITOSO'
        WHERE ID_BACKUP = @ID_Log

        DECLARE @ArchivoEliminar NVARCHAR(500)
        DECLARE cur CURSOR FOR
            SELECT NOMBRE_ARCHIVO
            FROM LOG_BACKUPS
            WHERE ESTADO = 'EXITOSO'
              AND NOMBRE_ARCHIVO IS NOT NULL
            ORDER BY FECHA_FIN DESC
            OFFSET 3 ROWS

        OPEN cur
        FETCH NEXT FROM cur INTO @ArchivoEliminar

        WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @Cmd = 'del /F /Q "' + @RutaBase + @ArchivoEliminar + '"'
            EXEC xp_cmdshell @Cmd, NO_OUTPUT
            FETCH NEXT FROM cur INTO @ArchivoEliminar
        END

        CLOSE cur
        DEALLOCATE cur

    END TRY
    BEGIN CATCH

        UPDATE LOG_BACKUPS SET
            FECHA_FIN     = GETDATE(),
            ESTADO        = 'ERROR',
            MENSAJE_ERROR = ERROR_MESSAGE()
        WHERE ID_BACKUP = @ID_Log

    END CATCH
END