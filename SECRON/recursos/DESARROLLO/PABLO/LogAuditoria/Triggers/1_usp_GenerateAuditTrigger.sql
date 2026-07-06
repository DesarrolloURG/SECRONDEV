-- =============================================
-- SP: usp_GenerateAuditTrigger
-- Genera/actualiza el trigger de auditoría para una tabla
-- Castea ignoran los tipos de dato (text/ntext/image)
-- ya que esos campos no son visibles desde un trigger "after"
-- =============================================
CREATE OR ALTER PROCEDURE dbo.usp_GenerateAuditTrigger
    @TableName NVARCHAR(128)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = @TableName AND schema_id = SCHEMA_ID('dbo'))
    BEGIN
        RAISERROR('Tabla %s no existe.', 16, 1, @TableName);
        RETURN;
    END

    -- Detectar PK (soporta compuesta)
    DECLARE @PKCols TABLE (ColName NVARCHAR(128), Ord INT);
    INSERT INTO @PKCols (ColName, Ord)
    SELECT c.name, ic.key_ordinal
    FROM sys.indexes i
    JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
    JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
    WHERE i.is_primary_key = 1 AND i.object_id = OBJECT_ID('dbo.' + @TableName);

    IF NOT EXISTS (SELECT 1 FROM @PKCols)
    BEGIN
        RAISERROR('Tabla %s no tiene PK.', 16, 1, @TableName);
        RETURN;
    END

    DECLARE @PKCol NVARCHAR(128) = (SELECT TOP 1 ColName FROM @PKCols ORDER BY Ord);
    DECLARE @TriggerName NVARCHAR(256) = 'TR_' + @TableName + '_Audit';

    -- Lista explícita de columnas, casteando tipos deprecados para el FOR XML
    DECLARE @colList NVARCHAR(MAX);
    SELECT @colList = STRING_AGG(QUOTENAME(c.name), ', ') WITHIN GROUP (ORDER BY c.column_id)
    FROM sys.columns c
    JOIN sys.types ty ON c.user_type_id = ty.user_type_id
    WHERE c.object_id = OBJECT_ID('dbo.' + @TableName)
      AND ty.name NOT IN ('text', 'ntext', 'image');

    DECLARE @sql NVARCHAR(MAX) = N'
CREATE OR ALTER TRIGGER dbo.' + QUOTENAME(@TriggerName) + N'
ON dbo.' + QUOTENAME(@TableName) + N'
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Action CHAR(6);
    IF EXISTS (SELECT 1 FROM inserted) AND EXISTS (SELECT 1 FROM deleted)
        SET @Action = ''UPDATE'';
    ELSE IF EXISTS (SELECT 1 FROM inserted)
        SET @Action = ''INSERT'';
    ELSE
        SET @Action = ''DELETE'';

    DECLARE @UserId    INT;
    DECLARE @UserName  NVARCHAR(100);
    DECLARE @ctx       BINARY(128) = CONTEXT_INFO();

    IF @ctx IS NOT NULL AND CONVERT(INT, SUBSTRING(@ctx, 1, 4)) <> 0
        SET @UserId = CONVERT(INT, SUBSTRING(@ctx, 1, 4));
    ELSE
        SET @UserName = SUSER_SNAME();

    DECLARE @IPAddress NVARCHAR(50)  = CONVERT(NVARCHAR(50), CONNECTIONPROPERTY(''client_net_address''));
    DECLARE @HostName  NVARCHAR(100) = HOST_NAME();

    DECLARE @RecordId INT;
    IF @Action = ''DELETE''
        SELECT @RecordId = ' + QUOTENAME(@PKCol) + N' FROM deleted;
    ELSE
        SELECT @RecordId = ' + QUOTENAME(@PKCol) + N' FROM inserted;

    DECLARE @AuditId INT;
    INSERT INTO AuditMaster (TableName, Action, RecordId, UserId, UserName, ActionDate, HostName, IPAddress)
    VALUES (
        ''' + @TableName + N''',
        @Action,
        @RecordId,
        @UserId,
        ISNULL(@UserName, SUSER_SNAME()),
        GETDATE(),
        @HostName,
        @IPAddress
    );

    SET @AuditId = SCOPE_IDENTITY();

    DECLARE @xmlInserted XML, @xmlDeleted XML;

    IF EXISTS (SELECT 1 FROM inserted)
        SELECT @xmlInserted = (SELECT ' + @colList + N' FROM inserted FOR XML RAW(''row''), TYPE);
    IF EXISTS (SELECT 1 FROM deleted)
        SELECT @xmlDeleted  = (SELECT ' + @colList + N' FROM deleted  FOR XML RAW(''row''), TYPE);

    DECLARE @excludedCols TABLE (ColName NVARCHAR(100));
    INSERT INTO @excludedCols
    SELECT ColName FROM dbo.AuditExcludedColumns
    WHERE TableName IS NULL OR TableName = ''' + @TableName + N''';

    DECLARE @colTypes TABLE (ColName NVARCHAR(100), DataType NVARCHAR(50));
    INSERT INTO @colTypes
    SELECT COLUMN_NAME, DATA_TYPE
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = ''' + @TableName + N''';

    DECLARE @refNode XML = ISNULL(@xmlInserted, @xmlDeleted);

    DECLARE @colName  NVARCHAR(100);
    DECLARE @oldValue NVARCHAR(MAX);
    DECLARE @newValue NVARCHAR(MAX);
    DECLARE @isPK     BIT;
    DECLARE @dataType NVARCHAR(50);

    DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
        SELECT attr.value(''local-name(.)'', ''NVARCHAR(100)'')
        FROM @refNode.nodes(''/row/@*'') AS T(attr);

    OPEN cur;
    FETCH NEXT FROM cur INTO @colName;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM @excludedCols WHERE ColName = @colName)
        BEGIN
            SET @oldValue = @xmlDeleted.value(''(/row/@*[local-name()=sql:variable("@colName")])[1]'', ''NVARCHAR(MAX)'');
            SET @newValue = @xmlInserted.value(''(/row/@*[local-name()=sql:variable("@colName")])[1]'', ''NVARCHAR(MAX)'');
            SET @isPK     = CASE WHEN EXISTS (SELECT 1 FROM (VALUES ' + 
                (SELECT STRING_AGG('(''' + ColName + ''')', ',') FROM @PKCols) + N') AS pk(c) WHERE pk.c = @colName) THEN 1 ELSE 0 END;
            SELECT @dataType = DataType FROM @colTypes WHERE ColName = @colName;

            IF @Action <> ''UPDATE'' OR ISNULL(@oldValue, '''') <> ISNULL(@newValue, '''')
            BEGIN
                INSERT INTO AuditDetail (AuditId, FieldName, OldValue, NewValue, DataType, IsPrimaryKey, IsSensitive)
                VALUES (@AuditId, @colName, @oldValue, @newValue, @dataType, @isPK, 0);
            END
        END

        FETCH NEXT FROM cur INTO @colName;
    END

    CLOSE cur;
    DEALLOCATE cur;
END;';

    EXEC sp_executesql @sql;
END;
GO