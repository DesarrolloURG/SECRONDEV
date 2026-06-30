-- =============================================
-- TRIGGER: TR_WarehouseLocations_Audit
-- Tabla: WarehouseLocations
-- Acciones: INSERT, UPDATE, DELETE
-- =============================================
CREATE OR ALTER TRIGGER TR_WarehouseLocations_Audit
ON WarehouseLocations
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Action CHAR(6);
    IF EXISTS (SELECT 1 FROM inserted) AND EXISTS (SELECT 1 FROM deleted)
        SET @Action = 'UPDATE';
    ELSE IF EXISTS (SELECT 1 FROM inserted)
        SET @Action = 'INSERT';
    ELSE
        SET @Action = 'DELETE';

    DECLARE @UserId    INT;
    DECLARE @UserName  NVARCHAR(100);
    DECLARE @ctx       BINARY(128) = CONTEXT_INFO();

    IF @ctx IS NOT NULL AND CONVERT(INT, SUBSTRING(@ctx, 1, 4)) <> 0
        SET @UserId = CONVERT(INT, SUBSTRING(@ctx, 1, 4));
    ELSE
        SET @UserName = SUSER_SNAME();

    DECLARE @IPAddress NVARCHAR(50)  = CONVERT(NVARCHAR(50), CONNECTIONPROPERTY('client_net_address'));
    DECLARE @HostName  NVARCHAR(100) = HOST_NAME();

    DECLARE @RecordId INT;
    IF @Action = 'DELETE'
        SELECT @RecordId = WarehouseLocationId FROM deleted;
    ELSE
        SELECT @RecordId = WarehouseLocationId FROM inserted;

    DECLARE @AuditId INT;
    INSERT INTO AuditMaster (TableName, Action, RecordId, UserId, UserName, ActionDate, HostName, IPAddress)
    VALUES (
        'WarehouseLocations',
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
        SELECT @xmlInserted = (SELECT * FROM inserted FOR XML RAW('row'), TYPE);
    IF EXISTS (SELECT 1 FROM deleted)
        SELECT @xmlDeleted  = (SELECT * FROM deleted  FOR XML RAW('row'), TYPE);

    DECLARE @excludedCols TABLE (ColName NVARCHAR(100));
    INSERT INTO @excludedCols VALUES ('CreatedDate'), ('CreatedBy');

    -- Cargar tipos de dato una sola vez
    DECLARE @colTypes TABLE (ColName NVARCHAR(100), DataType NVARCHAR(50));
    INSERT INTO @colTypes
    SELECT COLUMN_NAME, DATA_TYPE
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'WarehouseLocations';

    DECLARE @refNode XML = ISNULL(@xmlInserted, @xmlDeleted);

    DECLARE @colName  NVARCHAR(100);
    DECLARE @oldValue NVARCHAR(MAX);
    DECLARE @newValue NVARCHAR(MAX);
    DECLARE @isPK     BIT;
    DECLARE @dataType NVARCHAR(50);

    DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
        SELECT attr.value('local-name(.)', 'NVARCHAR(100)')
        FROM @refNode.nodes('/row/@*') AS T(attr);

    OPEN cur;
    FETCH NEXT FROM cur INTO @colName;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM @excludedCols WHERE ColName = @colName)
        BEGIN
            SET @oldValue = @xmlDeleted.value('(/row/@*[local-name()=sql:variable("@colName")])[1]', 'NVARCHAR(MAX)');
            SET @newValue = @xmlInserted.value('(/row/@*[local-name()=sql:variable("@colName")])[1]', 'NVARCHAR(MAX)');
            SET @isPK     = CASE WHEN @colName = 'WarehouseLocationId' THEN 1 ELSE 0 END;
            SELECT @dataType = DataType FROM @colTypes WHERE ColName = @colName;

            IF @Action <> 'UPDATE' OR ISNULL(@oldValue, '') <> ISNULL(@newValue, '')
            BEGIN
                INSERT INTO AuditDetail (AuditId, FieldName, OldValue, NewValue, DataType, IsPrimaryKey, IsSensitive)
                VALUES (@AuditId, @colName, @oldValue, @newValue, @dataType, @isPK, 0);
            END
        END

        FETCH NEXT FROM cur INTO @colName;
    END

    CLOSE cur;
    DEALLOCATE cur;
END;
GO