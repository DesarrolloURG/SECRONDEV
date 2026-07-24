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