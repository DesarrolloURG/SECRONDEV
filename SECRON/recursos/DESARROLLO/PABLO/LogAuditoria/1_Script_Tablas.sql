-- =============================================
-- TABLA MAESTRA DE AUDITORÍA
-- =============================================
CREATE TABLE AuditMaster (
    AuditId       INT IDENTITY(1,1) NOT NULL,
    TableName     NVARCHAR(100)     NOT NULL,
    Action        CHAR(6)           NOT NULL, -- INSERT, UPDATE, DELETE
    RecordId      INT               NULL,
    UserId        INT               NULL,
    UserName      NVARCHAR(100)     NULL,
    ActionDate    DATETIME2         NOT NULL  DEFAULT GETDATE(),
    HostName      NVARCHAR(100)     NULL,
    IPAddress     NVARCHAR(50)      NULL,
    CONSTRAINT PK_AuditMaster PRIMARY KEY (AuditId),
    CONSTRAINT CK_AuditMaster_Action CHECK (Action IN ('INSERT', 'UPDATE', 'DELETE'))
);

-- =============================================
-- TABLA DETALLE DE AUDITORÍA
-- =============================================
CREATE TABLE AuditDetail (
    DetailId      INT IDENTITY(1,1) NOT NULL,
    AuditId       INT               NOT NULL,
    FieldName     NVARCHAR(100)     NOT NULL,
    OldValue      NVARCHAR(MAX)     NULL,
    NewValue      NVARCHAR(MAX)     NULL,
    DataType      NVARCHAR(50)      NULL,
    IsPrimaryKey  BIT               NOT NULL  DEFAULT 0,
    IsSensitive   BIT               NOT NULL  DEFAULT 0,
    CONSTRAINT PK_AuditDetail PRIMARY KEY (DetailId),
    CONSTRAINT FK_AuditDetail_AuditMaster FOREIGN KEY (AuditId) REFERENCES AuditMaster(AuditId)
);

-- =============================================
-- ÍNDICES
-- =============================================
CREATE INDEX IX_AuditMaster_TableName_ActionDate ON AuditMaster (TableName, ActionDate);
CREATE INDEX IX_AuditMaster_RecordId             ON AuditMaster (RecordId);
CREATE INDEX IX_AuditDetail_AuditId              ON AuditDetail (AuditId);