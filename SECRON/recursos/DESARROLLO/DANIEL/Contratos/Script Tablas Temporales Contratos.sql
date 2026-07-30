-- =========================================================
-- SECRON - Submódulo Emisión de Contratos Docentes (PROVISIONAL)
-- Tablas: DocentesTemporal (maestro) / DocentesTemporal_Cursos (detalle)
-- =========================================================

CREATE TABLE DocentesTemporal (
    TeacherTempId       INT IDENTITY(1,1)   NOT NULL,
    ContractCode        VARCHAR(20)         NOT NULL,
    DPI                 VARCHAR(13)         NOT NULL,
    FirstName           VARCHAR(150)        NOT NULL,
    LastName            VARCHAR(150)        NOT NULL,
    BirthDate           DATE                NULL,
    MaritalStatus       VARCHAR(30)         NULL,
    Gender              VARCHAR(20)         NULL,
    Address             VARCHAR(300)        NULL,
    Nationality         VARCHAR(100)        NULL,
    CollegiateNumber    VARCHAR(30)         NULL,
    NIT                 VARCHAR(20)         NULL,
    Cycle               VARCHAR(20)         NULL,
    ContractYear        INT                 NULL,
    IssueDate           DATE                NULL,
    CreatedBy           INT                 NULL,
    CreatedDate         DATETIME            NOT NULL CONSTRAINT DF_DocentesTemporal_CreatedDate DEFAULT (GETDATE()),
    CONSTRAINT PK_DocentesTemporal PRIMARY KEY (TeacherTempId),
    CONSTRAINT UQ_DocentesTemporal_ContractCode UNIQUE (ContractCode)
);
GO

CREATE TABLE DocentesTemporal_Cursos (
    TeacherTempCourseId INT IDENTITY(1,1)   NOT NULL,
    TeacherTempId       INT                 NOT NULL,
    AcademicLocation    VARCHAR(150)        NOT NULL,
    CourseToTeach       VARCHAR(200)        NOT NULL,
    Schedule            VARCHAR(200)        NULL,
    Fees                DECIMAL(10,2)       NULL,
    CreatedBy           INT                 NULL,
    CreatedDate         DATETIME            NOT NULL CONSTRAINT DF_DocentesTemporal_Cursos_CreatedDate DEFAULT (GETDATE()),
    CONSTRAINT PK_DocentesTemporal_Cursos PRIMARY KEY (TeacherTempCourseId),
    CONSTRAINT FK_DocentesTemporal_Cursos_Docente FOREIGN KEY (TeacherTempId)
        REFERENCES DocentesTemporal (TeacherTempId)
);
GO

CREATE INDEX IX_DocentesTemporal_DPI ON DocentesTemporal (DPI);
GO
CREATE INDEX IX_DocentesTemporal_Cursos_TeacherTempId ON DocentesTemporal_Cursos (TeacherTempId);
GO

CREATE TABLE Portal_Contratos_Vigencia (
    VigenciaId      INT IDENTITY(1,1) PRIMARY KEY,
    FechaInicio     DATETIME NOT NULL,
    FechaFin        DATETIME NOT NULL,
    Activo          BIT NOT NULL DEFAULT 1,
    Observaciones   VARCHAR(200) NULL,
    CreatedBy       INT NULL,
    CreatedDate     DATETIME NOT NULL DEFAULT GETDATE(),
    ModifiedBy      INT NULL,
    ModifiedDate    DATETIME NULL
);
GO
-------------------------------- VALOR DE EJEMPLO PARA INSERCIÓN DE PERIODO DE TIEMPO HABILITADO -----------------------------
USE SECRONDEV;
GO

INSERT INTO Portal_Contratos_Vigencia (FechaInicio, FechaFin, Activo, Observaciones, CreatedBy, CreatedDate)
VALUES ('2026-07-01', '2026-08-31', 1, 'VENTANA DE PRUEBA PARA DESARROLLO', NULL, GETDATE());

GO-------------------------------- ************************************************************* -----------------------------