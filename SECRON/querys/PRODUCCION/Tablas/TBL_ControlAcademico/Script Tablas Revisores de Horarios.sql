-- =====================================================
-- TABLA: AcademicProcesses_Revisers
-- Usuarios designados como revisores de horarios (FR-ACA-05)
-- =====================================================
CREATE TABLE AcademicProcesses_Revisers (
    ReviserId       INT IDENTITY(1,1) PRIMARY KEY,
    UserId          INT NOT NULL,              -- Usuario que se convierte en revisor
    PersonType      NVARCHAR(20) NOT NULL,     -- 'DOCENTE' | 'TRABAJADOR' | 'PROVEEDOR' | 'COORDINADOR'
    PersonId        INT NOT NULL,              -- TeacherId / EmployeeId / SupplierId / CoordinatorId (trazabilidad, de qué ficha vino)
    IsActive        BIT NOT NULL DEFAULT 1,
    AssignedBy      INT NOT NULL,              -- Usuario (RRHH/Admin) que hizo la asignación
    AssignedDate    DATETIME NOT NULL DEFAULT GETDATE(),
    RemovedBy       INT NULL,
    RemovedDate     DATETIME NULL,

    CONSTRAINT FK_AcademicProcesses_Revisers_User FOREIGN KEY (UserId) REFERENCES Users(UserId),
    CONSTRAINT FK_AcademicProcesses_Revisers_AssignedBy FOREIGN KEY (AssignedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_AcademicProcesses_Revisers_RemovedBy FOREIGN KEY (RemovedBy) REFERENCES Users(UserId),
    CONSTRAINT UQ_AcademicProcesses_Revisers_User UNIQUE (UserId),
    CONSTRAINT CHK_AcademicProcesses_Revisers_PersonType CHECK (PersonType IN ('DOCENTE', 'TRABAJADOR', 'PROVEEDOR', 'COORDINADOR'))
);
GO

CREATE INDEX IX_AcademicProcesses_Revisers_IsActive ON AcademicProcesses_Revisers(IsActive);
GO

-- =====================================================
-- REGISTRO EN SISTEMA DE AUDITORÍA (paso obligatorio para tabla nueva)
-- =====================================================
INSERT INTO dbo.AuditConfig (TableName) VALUES ('AcademicProcesses_Revisers');
GO

EXEC dbo.usp_GenerateAuditTrigger 'AcademicProcesses_Revisers';
GO