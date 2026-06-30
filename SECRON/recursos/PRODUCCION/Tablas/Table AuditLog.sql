-- TABLA: AuditLog (Auditoría del Sistema)
CREATE TABLE AuditLog (
    AuditId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT,
    Action VARCHAR(100) NOT NULL,
    TableName VARCHAR(50),
    RecordId INT,
    OldValues NVARCHAR(MAX), -- JSON con valores anteriores
    NewValues NVARCHAR(MAX), -- JSON con valores nuevos
    ActionDate DATETIME DEFAULT GETDATE(),
    IPAddress VARCHAR(45),
    UserAgent VARCHAR(500),
    
    CONSTRAINT FK_AuditLog_User FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

SELECT * FROM AuditLog