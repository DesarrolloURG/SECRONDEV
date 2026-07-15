-- =============================================
-- CONFIGURACIÓN DE AUDITORÍA
-- =============================================
CREATE TABLE dbo.AuditConfig (
    TableName NVARCHAR(128) NOT NULL PRIMARY KEY,
    IsEnabled BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE dbo.AuditExcludedColumns (
    TableName NVARCHAR(128) NULL, -- NULL = exclusión global
    ColName   NVARCHAR(128) NOT NULL
);
GO

-- Exclusiones globales (equivalentes al trigger original)
INSERT INTO dbo.AuditExcludedColumns (TableName, ColName)
VALUES (NULL, 'CreatedDate'), (NULL, 'CreatedBy'), (NULL, 'ModifiedDate'), (NULL, 'ModifiedBy');
GO


INSERT INTO dbo.AuditConfig (TableName)
SELECT t.name
FROM sys.tables t
WHERE t.schema_id = SCHEMA_ID('dbo')
  AND t.name NOT IN ('AuditMaster', 'AuditDetail', 'AuditLog','AuditExcludedColumns','AuditConfig')
  AND NOT EXISTS (
      SELECT 1 FROM dbo.AuditConfig ac WHERE ac.TableName = t.name
  );
GO
