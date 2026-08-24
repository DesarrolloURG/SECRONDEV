-- =====================================================================
-- MÓDULO: PENSUM DE CARRERAS Y PRECIOS POR CURSO/SEDE/MODALIDAD
-- Script consolidado e idempotente (seguro de re-ejecutar)
-- Funciona tanto en ambiente NUEVO (crea todo desde cero) como en
-- ambiente con estructura VIEJA parcial (migra columnas faltantes).
-- =====================================================================

-- =====================================================================
-- 1. CourseModalities (catálogo)
-- =====================================================================
IF OBJECT_ID('CourseModalities', 'U') IS NULL
BEGIN
    CREATE TABLE CourseModalities (
        ModalityId INT IDENTITY(1,1) PRIMARY KEY,
        ModalityCode VARCHAR(20) NOT NULL,
        ModalityName VARCHAR(50) NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        CreatedBy INT NOT NULL,
        ModifiedDate DATETIME NULL,
        ModifiedBy INT NULL,
        CONSTRAINT UQ_CourseModalities_Code UNIQUE (ModalityCode)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM CourseModalities WHERE ModalityCode = 'FDS_SAB')
INSERT INTO CourseModalities (ModalityCode, ModalityName, CreatedBy)
VALUES ('FDS_SAB', 'FIN DE SEMANA (SÁBADO)', 1);

IF NOT EXISTS (SELECT 1 FROM CourseModalities WHERE ModalityCode = 'FDS_DOM')
INSERT INTO CourseModalities (ModalityCode, ModalityName, CreatedBy)
VALUES ('FDS_DOM', 'FIN DE SEMANA (DOMINGO)', 1);

IF NOT EXISTS (SELECT 1 FROM CourseModalities WHERE ModalityCode = 'ES_MAT')
INSERT INTO CourseModalities (ModalityCode, ModalityName, CreatedBy)
VALUES ('ES_MAT', 'ENTRE SEMANA (MATUTINO)', 1);

IF NOT EXISTS (SELECT 1 FROM CourseModalities WHERE ModalityCode = 'ES_VES')
INSERT INTO CourseModalities (ModalityCode, ModalityName, CreatedBy)
VALUES ('ES_VES', 'ENTRE SEMANA (VESPERTINO)', 1);
GO

-- =====================================================================
-- 2. LocationCareers (relación Sede-Carrera-Modalidad)
-- =====================================================================
IF OBJECT_ID('LocationCareers', 'U') IS NULL
BEGIN
    CREATE TABLE LocationCareers (
        LocationCareerId INT IDENTITY(1,1) PRIMARY KEY,
        LocationId INT NOT NULL,
        CareerId INT NOT NULL,
        ModalityId INT NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        CreatedBy INT NOT NULL,
        ModifiedDate DATETIME NULL,
        ModifiedBy INT NULL,
        CONSTRAINT FK_LocationCareers_Locations FOREIGN KEY (LocationId) REFERENCES Locations(LocationId),
        CONSTRAINT FK_LocationCareers_Careers FOREIGN KEY (CareerId) REFERENCES Careers(CareerId),
        CONSTRAINT FK_LocationCareers_Modality FOREIGN KEY (ModalityId) REFERENCES CourseModalities(ModalityId),
        CONSTRAINT UQ_LocationCareers UNIQUE (LocationId, CareerId, ModalityId)
    );
END
GO

-- --- Migración: si LocationCareers ya existía en versión vieja (sin ModalityId) ---
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('LocationCareers') AND name = 'ModalityId')
BEGIN
    ALTER TABLE LocationCareers ADD ModalityId INT NULL;
END
GO

-- Solo forzar NOT NULL si la tabla está vacía (evita romper si ya tiene datos sin modalidad asignada)
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('LocationCareers') AND name = 'ModalityId' AND is_nullable = 1)
   AND NOT EXISTS (SELECT 1 FROM LocationCareers)
BEGIN
    ALTER TABLE LocationCareers ALTER COLUMN ModalityId INT NOT NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_LocationCareers_Modality')
    ALTER TABLE LocationCareers ADD CONSTRAINT FK_LocationCareers_Modality
        FOREIGN KEY (ModalityId) REFERENCES CourseModalities(ModalityId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'UQ_LocationCareers')
    ALTER TABLE LocationCareers ADD CONSTRAINT UQ_LocationCareers UNIQUE (LocationId, CareerId, ModalityId);
GO

-- =====================================================================
-- 3. CareerPensums (versiones de pensum por carrera)
-- =====================================================================
IF OBJECT_ID('CareerPensums', 'U') IS NULL
BEGIN
    CREATE TABLE CareerPensums (
        CareerPensumId INT IDENTITY(1,1) PRIMARY KEY,
        CareerId INT NOT NULL,
        PensumCode VARCHAR(50) NOT NULL,
        PensumName VARCHAR(150) NOT NULL,
        IsCurrent BIT NOT NULL DEFAULT 0,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        CreatedBy INT NOT NULL,
        ModifiedDate DATETIME NULL,
        ModifiedBy INT NULL,
        CONSTRAINT FK_CareerPensums_Careers FOREIGN KEY (CareerId) REFERENCES Careers(CareerId),
        CONSTRAINT UQ_CareerPensums UNIQUE (CareerId, PensumCode)
    );
END
GO

-- Migración: por si la tabla ya existía con PensumCode más corto que VARCHAR(50)
IF EXISTS (
    SELECT 1 FROM sys.columns c
    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE c.object_id = OBJECT_ID('CareerPensums') AND c.name = 'PensumCode' AND c.max_length < 50
)
BEGIN
    ALTER TABLE CareerPensums ALTER COLUMN PensumCode VARCHAR(50) NOT NULL;
END
GO

-- =====================================================================
-- 4. CareerCourses (cursos dentro de un pensum, con precio estándar)
-- =====================================================================
IF OBJECT_ID('CareerCourses', 'U') IS NULL
BEGIN
    CREATE TABLE CareerCourses (
        CareerCourseId INT IDENTITY(1,1) PRIMARY KEY,
        CareerPensumId INT NOT NULL,
        CourseId INT NOT NULL,
        Semester INT NOT NULL,
        IsRequired BIT NULL DEFAULT 1,
        StandardPrice DECIMAL(10,2) NOT NULL,
        IsActive BIT NULL DEFAULT 1,
        CreatedDate DATETIME NULL DEFAULT GETDATE(),
        CreatedBy INT NULL,
        ModifiedDate DATETIME NULL,
        ModifiedBy INT NULL,
        CONSTRAINT FK_CareerCourses_CareerPensum FOREIGN KEY (CareerPensumId) REFERENCES CareerPensums(CareerPensumId),
        CONSTRAINT FK_CareerCourses_Course FOREIGN KEY (CourseId) REFERENCES Courses(CourseId),
        CONSTRAINT UQ_CareerCourses UNIQUE (CareerPensumId, CourseId)
    );
END
GO

-- --- Migración: si CareerCourses ya existía en versión vieja (con CareerId, sin pensum) ---
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CareerCourses') AND name = 'CareerId')
BEGIN
    IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_CareerCourses_Career')
        ALTER TABLE CareerCourses DROP CONSTRAINT FK_CareerCourses_Career;

    IF EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'UQ_CareerCourses')
        ALTER TABLE CareerCourses DROP CONSTRAINT UQ_CareerCourses;

    ALTER TABLE CareerCourses DROP COLUMN CareerId;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CareerCourses') AND name = 'CareerPensumId')
    ALTER TABLE CareerCourses ADD CareerPensumId INT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CareerCourses') AND name = 'StandardPrice')
    ALTER TABLE CareerCourses ADD StandardPrice DECIMAL(10,2) NULL;
GO

-- Solo forzar NOT NULL si la tabla está vacía (evita romper si ya tiene datos sin migrar)
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CareerCourses') AND name = 'CareerPensumId' AND is_nullable = 1)
   AND NOT EXISTS (SELECT 1 FROM CareerCourses)
BEGIN
    ALTER TABLE CareerCourses ALTER COLUMN CareerPensumId INT NOT NULL;
    ALTER TABLE CareerCourses ALTER COLUMN StandardPrice DECIMAL(10,2) NOT NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_CareerCourses_CareerPensum')
    ALTER TABLE CareerCourses ADD CONSTRAINT FK_CareerCourses_CareerPensum
        FOREIGN KEY (CareerPensumId) REFERENCES CareerPensums(CareerPensumId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'UQ_CareerCourses')
    ALTER TABLE CareerCourses ADD CONSTRAINT UQ_CareerCourses UNIQUE (CareerPensumId, CourseId);
GO

-- --- Migración: eliminar Prerequisites (columna vieja de texto libre, reemplazada por CareerCoursePrerequisites) ---
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('CareerCourses') AND name = 'Prerequisites')
    ALTER TABLE CareerCourses DROP COLUMN Prerequisites;
GO

-- =====================================================================
-- 5. CourseLocationPricingMaster (combinación válida de precio)
-- =====================================================================
IF OBJECT_ID('CourseLocationPricingMaster', 'U') IS NULL
BEGIN
    CREATE TABLE CourseLocationPricingMaster (
        CourseLocationPricingId INT IDENTITY(1,1) PRIMARY KEY,
        CareerCourseId INT NOT NULL,
        LocationId INT NOT NULL,
        ModalityId INT NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        CreatedBy INT NOT NULL,
        ModifiedDate DATETIME NULL,
        ModifiedBy INT NULL,
        CONSTRAINT FK_CLPMaster_CareerCourses FOREIGN KEY (CareerCourseId) REFERENCES CareerCourses(CareerCourseId),
        CONSTRAINT FK_CLPMaster_Locations FOREIGN KEY (LocationId) REFERENCES Locations(LocationId),
        CONSTRAINT FK_CLPMaster_Modalities FOREIGN KEY (ModalityId) REFERENCES CourseModalities(ModalityId),
        CONSTRAINT UQ_CLPMaster UNIQUE (CareerCourseId, LocationId, ModalityId)
    );
END
GO

-- =====================================================================
-- 6. CourseLocationPricingDetail (historial de precios)
-- =====================================================================
IF OBJECT_ID('CourseLocationPricingDetail', 'U') IS NULL
BEGIN
    CREATE TABLE CourseLocationPricingDetail (
        CourseLocationPricingDetailId INT IDENTITY(1,1) PRIMARY KEY,
        CourseLocationPricingId INT NOT NULL,
        Price DECIMAL(10,2) NOT NULL,
        EffectiveFrom DATETIME NOT NULL DEFAULT GETDATE(),
        EffectiveTo DATETIME NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        CreatedBy INT NOT NULL,
        ModifiedDate DATETIME NULL,
        ModifiedBy INT NULL,
        CONSTRAINT FK_CLPDetail_Master FOREIGN KEY (CourseLocationPricingId) REFERENCES CourseLocationPricingMaster(CourseLocationPricingId)
    );
END
GO

-- =====================================================================
-- 7. CareerCoursePrerequisites (prerequisitos múltiples)
-- =====================================================================
IF OBJECT_ID('CareerCoursePrerequisites', 'U') IS NULL
BEGIN
    CREATE TABLE CareerCoursePrerequisites (
        CareerCoursePrerequisiteId INT IDENTITY(1,1) PRIMARY KEY,
        CareerCourseId INT NOT NULL,
        PrerequisiteCareerCourseId INT NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        CreatedBy INT NOT NULL,
        ModifiedDate DATETIME NULL,
        ModifiedBy INT NULL,
        CONSTRAINT FK_CCP_CareerCourse FOREIGN KEY (CareerCourseId) REFERENCES CareerCourses(CareerCourseId),
        CONSTRAINT FK_CCP_PrerequisiteCareerCourse FOREIGN KEY (PrerequisiteCareerCourseId) REFERENCES CareerCourses(CareerCourseId),
        CONSTRAINT UQ_CCP UNIQUE (CareerCourseId, PrerequisiteCareerCourseId)
    );
END
GO

-- =====================================================================
-- 8. Permisos del módulo de precios
-- =====================================================================
INSERT INTO Permissions (PermissionCode, PermissionName, Description, ModuleName, ActionType)
SELECT * FROM (VALUES
('ACP_005', 'ACADEMICPROCESSES_COURSESPRICING_CREATE', 'PERMITE REGISTRAR NUEVAS PRECIOS DE CURSOS EN EL SISTEMA.', 'ACADEMICPROCESSES', 'CREATE'),
('ACP_006', 'ACADEMICPROCESSES_COURSESPRICING_UPDATE', 'PERMITE MODIFICAR PRECIOS DE CURSOS EN EL SISTEMA.', 'ACADEMICPROCESSES', 'UPDATE'),
('ACP_007', 'ACADEMICPROCESSES_COURSESPRICING_INACTIVE', 'PERMITE ACTIVAR/DESACTIVAR PRECIOS EN EL SISTEMA.', 'ACADEMICPROCESSES', 'INACTIVE'),
('ACP_008', 'ACADEMICPROCESSES_COURSESPRICING_EXPORT', 'PERMITE EXPORTAR LISTADOS DE PRECIOS EN EL SISTEMA.', 'ACADEMICPROCESSES', 'EXPORT')
) AS nuevos(PermissionCode, PermissionName, Description, ModuleName, ActionType)
WHERE NOT EXISTS (SELECT 1 FROM Permissions p WHERE p.PermissionCode = nuevos.PermissionCode)
GO

INSERT INTO Permissions (PermissionCode, PermissionName, Description, ModuleName, ActionType)
SELECT * FROM (VALUES
('ACP_013', 'ACADEMICPROCESSES_PENSUM_CREATE', 'PERMITE REGISTRAR NUEVAS PRECIOS DE CURSOS EN EL SISTEMA.', 'ACADEMICPROCESSES', 'CREATE'),
('ACP_014', 'ACADEMICPROCESSES_PENSUM_UPDATE', 'PERMITE MODIFICAR PRECIOS DE CURSOS EN EL SISTEMA.', 'ACADEMICPROCESSES', 'UPDATE'),
('ACP_015', 'ACADEMICPROCESSES_PENSUM_INACTIVE', 'PERMITE ACTIVAR/DESACTIVAR PRECIOS EN EL SISTEMA.', 'ACADEMICPROCESSES', 'INACTIVE'),
('ACP_016', 'ACADEMICPROCESSES_PENSUM_EXPORT', 'PERMITE EXPORTAR LISTADOS DE PRECIOS EN EL SISTEMA.', 'ACADEMICPROCESSES', 'EXPORT'),
('ACP_017', 'ACADEMICPROCESSES_PENSUM', 'PERMITE ABRIR LA VENTANA DE PENSUM DE CARRERAS.', 'ACADEMICPROCESSES', 'TAB')
) AS nuevos(PermissionCode, PermissionName, Description, ModuleName, ActionType)
WHERE NOT EXISTS (SELECT 1 FROM Permissions p WHERE p.PermissionCode = nuevos.PermissionCode)
GO

-- =====================================================================
-- 9. Regenerar triggers de auditoría 
-- =====================================================================
EXEC dbo.usp_GenerateAllAuditTriggers;
GO

-- =====================================================================
-- 10. Parámetro de precio sugerido
-- =====================================================================
INSERT INTO ParametersConfiguration (ParameterName, ParameterValue, Description, CreateDate)
SELECT 'PrecioSugeridoCursosPensum', '1000', 'Precio estándar sugerido al agregar un curso nuevo al pensum (editable por el usuario)', GETDATE()
WHERE NOT EXISTS (SELECT 1 FROM ParametersConfiguration WHERE ParameterName = 'PrecioSugeridoCursosPensum');
GO