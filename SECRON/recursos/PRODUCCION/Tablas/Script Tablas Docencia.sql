-- =====================================================================
-- SECRON - DROP ordenado de tablas del modulo DOCENCIA
-- Ejecutar en QA y PRODUCCION antes de recrear
-- Orden: de tablas hijas (mas dependientes) a padres (menos dependientes)
-- =====================================================================

DROP TABLE IF EXISTS ScheduleDetails;
GO
DROP TABLE IF EXISTS Schedules;
GO
DROP TABLE IF EXISTS SectionCourses;
GO
DROP TABLE IF EXISTS TeacherCourses;
GO
DROP TABLE IF EXISTS Sections;
GO
DROP TABLE IF EXISTS Teachers;
GO
DROP TABLE IF EXISTS CareerCourses;
GO
DROP TABLE IF EXISTS Courses;
GO
DROP TABLE IF EXISTS Careers;
GO
DROP TABLE IF EXISTS ScheduleTypes;
GO
DROP TABLE IF EXISTS Coordinators;
GO


-- =====================================================
-- MÓDULO DE DOCENCIA - SECRON 2.0
-- Sistema de Gestión Académica Completo
-- =====================================================
DROP TABLE Teachers
-- -----------------------------------------------------
-- 1. TABLA: Coordinators (Coordinadores Académicos)
-- Entidad independiente - pueden o no tener usuario
-- -----------------------------------------------------
DROP TABLE IF EXISTS Coordinators;
GO

CREATE TABLE Coordinators (
    CoordinatorId             INT IDENTITY(1,1) PRIMARY KEY,   -- 0
    CoordinatorCode           NVARCHAR(20) NOT NULL,           -- 1
    FullName                  NVARCHAR(150) NOT NULL,          -- 2
    Phone                     NVARCHAR(20) NULL,               -- 3
    Email                     NVARCHAR(150) NULL,              -- 4
    DPI                       NVARCHAR(20) NULL,               -- 5
    NIT                       NVARCHAR(20) NULL,               -- 6
    Address                   NVARCHAR(255) NULL,              -- 7
    AcademicTitle             NVARCHAR(100) NULL,              -- 8
    Specialization            NVARCHAR(150) NULL,              -- 9
    IsCollegiateActive        BIT NOT NULL DEFAULT 0,          -- 10
    CollegiateNumber          NVARCHAR(50) NULL,               -- 11
    BankAccountNumber         NVARCHAR(50) NULL,               -- 12
    BankId                    INT NULL,                        -- 13
    LocationId                INT NULL,                        -- 14  (NULLABLE)
    HireDate                  DATETIME NULL,                   -- 15
    ContractType              NVARCHAR(50) NULL,               -- 16
    UserId                    INT NULL,                        -- 17
    RegisteredByCoordinatorId INT NULL,                        -- 18
    IsActive                  BIT NOT NULL DEFAULT 1,          -- 19
    CreatedDate               DATETIME NOT NULL DEFAULT GETDATE(), -- 20
    CreatedBy                 INT NULL,                        -- 21
    ModifiedDate              DATETIME NULL,                   -- 22
    ModifiedBy                INT NULL,                        -- 23
    FilePath_DPI              NVARCHAR(500) NULL,              -- 24
    FilePath_Titulos          NVARCHAR(500) NULL,              -- 25
    FilePath_RTU              NVARCHAR(500) NULL,              -- 26
    FilePath_Colegiado        NVARCHAR(500) NULL,              -- 27
    FilePath_RENAS            NVARCHAR(500) NULL,              -- 28
    FilePath_AntPoliciacos    NVARCHAR(500) NULL,              -- 29
    FilePath_AntPenales       NVARCHAR(500) NULL               -- 30
);
GO

-- -----------------------------------------------------
-- 2. TABLA: ScheduleTypes (Tipos de Horario)
-- Configurable: Lun-Mié, Lun-Vie, Sábados, Domingos, etc.
-- -----------------------------------------------------
CREATE TABLE ScheduleTypes (
    ScheduleTypeId INT IDENTITY(1,1) PRIMARY KEY,
    ScheduleTypeCode NVARCHAR(20) NOT NULL UNIQUE,
    ScheduleTypeName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255),
    
    -- Días de la semana que abarca este tipo de horario
    IncludesMonday BIT DEFAULT 0,
    IncludesTuesday BIT DEFAULT 0,
    IncludesWednesday BIT DEFAULT 0,
    IncludesThursday BIT DEFAULT 0,
    IncludesFriday BIT DEFAULT 0,
    IncludesSaturday BIT DEFAULT 0,
    IncludesSunday BIT DEFAULT 0,
    
    -- Jornada
    TimeShift NVARCHAR(50),  -- Mañana, Tarde, Noche, Mixto
    
    -- Control
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT,
    ModifiedDate DATETIME,
    ModifiedBy INT,
    
    CONSTRAINT FK_ScheduleTypes_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_ScheduleTypes_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId)
);

-- -----------------------------------------------------
-- 3. TABLA: Careers (Carreras Universitarias)
-- -----------------------------------------------------
CREATE TABLE Careers (
    CareerId INT IDENTITY(1,1) PRIMARY KEY,
    CareerCode NVARCHAR(20) NOT NULL UNIQUE,
    CareerName NVARCHAR(150) NOT NULL,
    Description NVARCHAR(500),
    DurationYears INT,  -- Duración en años
    TotalSemesters INT, -- Total de semestres/ciclos
    TotalCredits INT,   -- Créditos totales requeridos
    
    -- Control
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT,
    ModifiedDate DATETIME,
    ModifiedBy INT,
    
    CONSTRAINT FK_Careers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Careers_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId)
);

-- -----------------------------------------------------
-- 4. TABLA: Courses (Catálogo Maestro de Cursos)
-- -----------------------------------------------------
CREATE TABLE Courses (
    CourseId INT IDENTITY(1,1) PRIMARY KEY,
    CourseCode NVARCHAR(20) NOT NULL UNIQUE,
    CourseName NVARCHAR(150) NOT NULL,
    Description NVARCHAR(500),
    Credits INT DEFAULT 0,
    TheoryHours INT DEFAULT 0,      -- Horas teóricas semanales
    PracticeHours INT DEFAULT 0,    -- Horas prácticas semanales
    LabHours INT DEFAULT 0,         -- Horas de laboratorio semanales
    TotalHours AS (TheoryHours + PracticeHours + LabHours) PERSISTED,  -- Calculado automáticamente
    
    -- Control
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT,
    ModifiedDate DATETIME,
    ModifiedBy INT,
    
    CONSTRAINT FK_Courses_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Courses_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId)
);

-- -----------------------------------------------------
-- 5. TABLA: CareerCourses (Detalle: Cursos de una Carrera)
-- Qué cursos pertenecen al pensum de cada carrera
-- -----------------------------------------------------
CREATE TABLE CareerCourses (
    CareerCourseId INT IDENTITY(1,1) PRIMARY KEY,
    CareerId INT NOT NULL,
    CourseId INT NOT NULL,
    Semester INT NOT NULL,          -- Semestre/Ciclo en que se imparte (1, 2, 3...)
    IsRequired BIT DEFAULT 1,       -- Obligatorio (1) u Optativo (0)
    Prerequisites NVARCHAR(500),    -- IDs de cursos prerequisitos (ej: "12,15,18")
    
    -- Control
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT,
    ModifiedDate DATETIME,
    ModifiedBy INT,
    
    CONSTRAINT FK_CareerCourses_Career FOREIGN KEY (CareerId) REFERENCES Careers(CareerId),
    CONSTRAINT FK_CareerCourses_Course FOREIGN KEY (CourseId) REFERENCES Courses(CourseId),
    CONSTRAINT FK_CareerCourses_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_CareerCourses_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId),
    CONSTRAINT UQ_CareerCourses UNIQUE (CareerId, CourseId)
);

-- -----------------------------------------------------
-- 6. TABLA: Sections (Secciones por Carrera)
-- Una sección es un grupo específico de estudiantes
-- -----------------------------------------------------
CREATE TABLE Sections (
    SectionId INT IDENTITY(1,1) PRIMARY KEY,
    SectionCode NVARCHAR(20) NOT NULL UNIQUE,
    SectionName NVARCHAR(100) NOT NULL,
    
    -- Relaciones principales
    CareerId INT NOT NULL,
    LocationId INT NOT NULL,
    ScheduleTypeId INT NOT NULL,
    CoordinatorId INT NOT NULL,  -- Coordinador responsable de la sección
    
    -- Información académica
    CurrentSemester INT DEFAULT 1,  -- Semestre actual en que se encuentra la sección
    AcademicYear INT,               -- Año académico
    
    -- Información de estudiantes
    StudentCount INT DEFAULT 0,     -- Cantidad actual de estudiantes
    MaxCapacity INT DEFAULT 30,     -- Capacidad máxima
    
    -- Fechas
    StartDate DATE,
    EndDate DATE,
    
    -- Control
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT,
    ModifiedDate DATETIME,
    ModifiedBy INT,
    
    CONSTRAINT FK_Sections_Career FOREIGN KEY (CareerId) REFERENCES Careers(CareerId),
    CONSTRAINT FK_Sections_Location FOREIGN KEY (LocationId) REFERENCES Locations(LocationId),
    CONSTRAINT FK_Sections_ScheduleType FOREIGN KEY (ScheduleTypeId) REFERENCES ScheduleTypes(ScheduleTypeId),
    CONSTRAINT FK_Sections_Coordinator FOREIGN KEY (CoordinatorId) REFERENCES Coordinators(CoordinatorId),
    CONSTRAINT FK_Sections_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Sections_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId)
);

-- -----------------------------------------------------
-- 7. TABLA: SectionCourses (Detalle: Cursos de una Sección)
-- Qué cursos específicamente se imparten en esta sección
-- Pueden heredarse de la carrera o personalizarse
-- -----------------------------------------------------
CREATE TABLE SectionCourses (
    SectionCourseId INT IDENTITY(1,1) PRIMARY KEY,
    SectionId INT NOT NULL,
    CourseId INT NOT NULL,
    
    -- Información específica para esta asignación
    Semester INT,  -- En qué semestre de la sección se impartirá
    
    -- Control
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT,
    ModifiedDate DATETIME,
    ModifiedBy INT,
    
    CONSTRAINT FK_SectionCourses_Section FOREIGN KEY (SectionId) REFERENCES Sections(SectionId),
    CONSTRAINT FK_SectionCourses_Course FOREIGN KEY (CourseId) REFERENCES Courses(CourseId),
    CONSTRAINT FK_SectionCourses_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_SectionCourses_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId),
    CONSTRAINT UQ_SectionCourses UNIQUE (SectionId, CourseId)
);

-- -----------------------------------------------------
-- 8. TABLA: Teachers (OPTIMIZADA)
-- Docentes del sistema
-- -----------------------------------------------------
CREATE TABLE Teachers (
    TeacherId INT IDENTITY(1,1) PRIMARY KEY,
    TeacherCode NVARCHAR(20) NOT NULL UNIQUE,  -- Código público del docente
    
    -- Información Personal
    FullName NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(20),
    Email NVARCHAR(100),
    DPI NVARCHAR(20),
    NIT NVARCHAR(20),
    Address NVARCHAR(255),
    
    -- Información Académica
    AcademicTitle NVARCHAR(255),              -- Licenciado, Ingeniero, Máster, Doctor, etc.
    Specialization NVARCHAR(255),              -- Área de especialización
    IsCollegiateActive BIT DEFAULT 0,
    CollegiateNumber NVARCHAR(20),
    
    -- Información Bancaria
    BankAccountNumber NVARCHAR(30),
    BankId INT,
    
    -- Asignación de Sede principal
    LocationId INT NOT NULL,  -- Sede principal a la que pertenece el docente
    
    -- Información de contratación
    HireDate DATE,
    ContractType NVARCHAR(50),  -- Tiempo completo, Medio tiempo, Por hora
    
    -- Relación con Usuario (opcional)
    UserId INT NULL,  -- NULL si no tiene usuario del sistema
    
    -- Quién lo registró (Coordinador que lo agregó)
    RegisteredByCoordinatorId INT,
    
    -- Control
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT,
    ModifiedDate DATETIME,
    ModifiedBy INT,
    
    CONSTRAINT FK_Teachers_Bank FOREIGN KEY (BankId) REFERENCES Banks(BankId),
    CONSTRAINT FK_Teachers_Location FOREIGN KEY (LocationId) REFERENCES Locations(LocationId),
    CONSTRAINT FK_Teachers_User FOREIGN KEY (UserId) REFERENCES Users(UserId),
    CONSTRAINT FK_Teachers_RegisteredByCoordinator FOREIGN KEY (RegisteredByCoordinatorId) REFERENCES Coordinators(CoordinatorId),
    CONSTRAINT FK_Teachers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Teachers_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId)
);

ALTER TABLE Teachers ADD
    FilePath_DPI            NVARCHAR(500) NULL,
    FilePath_Titulos        NVARCHAR(500) NULL,
    FilePath_RTU            NVARCHAR(500) NULL,
    FilePath_Colegiado      NVARCHAR(500) NULL,
    FilePath_RENAS          NVARCHAR(500) NULL,
    FilePath_AntPoliciacos  NVARCHAR(500) NULL,
    FilePath_AntPenales     NVARCHAR(500) NULL;
GO



-- -----------------------------------------------------
-- 9. TABLA: TeacherCourses (Detalle: Cursos que imparte el Docente)
-- Qué cursos está capacitado para impartir un docente
-- -----------------------------------------------------
CREATE TABLE TeacherCourses (
    TeacherCourseId INT IDENTITY(1,1) PRIMARY KEY,
    TeacherId INT NOT NULL,
    CourseId INT NOT NULL,
    
    YearsOfExperience INT DEFAULT 0,  -- Años de experiencia impartiendo este curso
    Certification NVARCHAR(255),      -- Certificaciones relacionadas al curso
    
    -- Control
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT,
    ModifiedDate DATETIME,
    ModifiedBy INT,
    
    CONSTRAINT FK_TeacherCourses_Teacher FOREIGN KEY (TeacherId) REFERENCES Teachers(TeacherId),
    CONSTRAINT FK_TeacherCourses_Course FOREIGN KEY (CourseId) REFERENCES Courses(CourseId),
    CONSTRAINT FK_TeacherCourses_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_TeacherCourses_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId),
    CONSTRAINT UQ_TeacherCourses UNIQUE (TeacherId, CourseId)
);

-- -----------------------------------------------------
-- 10. TABLA: Schedules (Horarios)
-- Un horario es la programación de cursos para una sección
-- -----------------------------------------------------
CREATE TABLE Schedules (
    ScheduleId INT IDENTITY(1,1) PRIMARY KEY,
    ScheduleCode NVARCHAR(20) NOT NULL UNIQUE,
    ScheduleName NVARCHAR(150) NOT NULL,
    
    -- Relaciones
    SectionId INT NOT NULL,
    LocationId INT NOT NULL,        -- Debe coincidir con Location de la Sección
    ScheduleTypeId INT NOT NULL,    -- Debe coincidir con ScheduleType de la Sección
    
    -- Información académica
    AcademicYear INT NOT NULL,
    Semester INT NOT NULL,
    
    -- Fechas de vigencia
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    
    -- Control
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT,
    ModifiedDate DATETIME,
    ModifiedBy INT,
    
    CONSTRAINT FK_Schedules_Section FOREIGN KEY (SectionId) REFERENCES Sections(SectionId),
    CONSTRAINT FK_Schedules_Location FOREIGN KEY (LocationId) REFERENCES Locations(LocationId),
    CONSTRAINT FK_Schedules_ScheduleType FOREIGN KEY (ScheduleTypeId) REFERENCES ScheduleTypes(ScheduleTypeId),
    CONSTRAINT FK_Schedules_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Schedules_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId),
    CONSTRAINT CHK_Schedules_Dates CHECK (EndDate >= StartDate)
);

-- -----------------------------------------------------
-- 11. TABLA: ScheduleDetails (Detalle del Horario)
-- Las clases específicas dentro de un horario
-- -----------------------------------------------------
CREATE TABLE ScheduleDetails (
    ScheduleDetailId INT IDENTITY(1,1) PRIMARY KEY,
    ScheduleId INT NOT NULL,
    CourseId INT NOT NULL,
    TeacherId INT NOT NULL,
    
    -- Información de la clase
    DayOfWeek INT NOT NULL,         -- 1=Lunes, 2=Martes, 3=Miércoles, 4=Jueves, 5=Viernes, 6=Sábado, 7=Domingo
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    
    Classroom NVARCHAR(50),         -- Número de aula/salón
    Building NVARCHAR(50),          -- Edificio
    
    -- Control
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT,
    ModifiedDate DATETIME,
    ModifiedBy INT,
    
    CONSTRAINT FK_ScheduleDetails_Schedule FOREIGN KEY (ScheduleId) REFERENCES Schedules(ScheduleId),
    CONSTRAINT FK_ScheduleDetails_Course FOREIGN KEY (CourseId) REFERENCES Courses(CourseId),
    CONSTRAINT FK_ScheduleDetails_Teacher FOREIGN KEY (TeacherId) REFERENCES Teachers(TeacherId),
    CONSTRAINT FK_ScheduleDetails_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_ScheduleDetails_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId),
    CONSTRAINT CHK_ScheduleDetails_DayOfWeek CHECK (DayOfWeek BETWEEN 1 AND 7),
    CONSTRAINT CHK_ScheduleDetails_Time CHECK (EndTime > StartTime)
);

-- =====================================================
-- ÍNDICES PARA OPTIMIZACIÓN DE BÚSQUEDAS
-- =====================================================

-- Índices en códigos (búsquedas frecuentes por código público)
CREATE INDEX IX_Coordinators_Code ON Coordinators(CoordinatorCode) WHERE IsActive = 1;
CREATE INDEX IX_Teachers_Code ON Teachers(TeacherCode) WHERE IsActive = 1;
CREATE INDEX IX_Courses_Code ON Courses(CourseCode) WHERE IsActive = 1;
CREATE INDEX IX_Careers_Code ON Careers(CareerCode) WHERE IsActive = 1;
CREATE INDEX IX_Sections_Code ON Sections(SectionCode) WHERE IsActive = 1;
CREATE INDEX IX_Schedules_Code ON Schedules(ScheduleCode) WHERE IsActive = 1;
CREATE INDEX IX_ScheduleTypes_Code ON ScheduleTypes(ScheduleTypeCode) WHERE IsActive = 1;

-- Índices en relaciones frecuentes
CREATE INDEX IX_Coordinators_Location ON Coordinators(LocationId) WHERE IsActive = 1;
CREATE INDEX IX_Teachers_Location ON Teachers(LocationId) WHERE IsActive = 1;
CREATE INDEX IX_Teachers_Coordinator ON Teachers(RegisteredByCoordinatorId) WHERE IsActive = 1;
CREATE INDEX IX_Sections_Location ON Sections(LocationId) WHERE IsActive = 1;
CREATE INDEX IX_Sections_Career ON Sections(CareerId) WHERE IsActive = 1;
CREATE INDEX IX_Sections_Coordinator ON Sections(CoordinatorId) WHERE IsActive = 1;
CREATE INDEX IX_Schedules_Section ON Schedules(SectionId) WHERE IsActive = 1;

-- Índices en tablas detalle
CREATE INDEX IX_CareerCourses_Career ON CareerCourses(CareerId) WHERE IsActive = 1;
CREATE INDEX IX_CareerCourses_Course ON CareerCourses(CourseId) WHERE IsActive = 1;
CREATE INDEX IX_SectionCourses_Section ON SectionCourses(SectionId) WHERE IsActive = 1;
CREATE INDEX IX_TeacherCourses_Teacher ON TeacherCourses(TeacherId) WHERE IsActive = 1;
CREATE INDEX IX_ScheduleDetails_Schedule ON ScheduleDetails(ScheduleId) WHERE IsActive = 1;
CREATE INDEX IX_ScheduleDetails_Teacher ON ScheduleDetails(TeacherId) WHERE IsActive = 1;

-- =====================================================
-- DATOS DE EJEMPLO (INICIALIZACIÓN)
-- =====================================================

-- Tipos de Horario predefinidos
INSERT INTO ScheduleTypes (ScheduleTypeCode, ScheduleTypeName, Description, 
    IncludesMonday, IncludesTuesday, IncludesWednesday, IncludesThursday, IncludesFriday, 
    IncludesSaturday, IncludesSunday, TimeShift, CreatedBy)
VALUES 
('LV-MAN', 'Lunes a Viernes - Mañana', 'Clases de lunes a viernes en horario matutino (07:00-13:00)', 
    1, 1, 1, 1, 1, 0, 0, 'Mañana', 1),
('LV-TAR', 'Lunes a Viernes - Tarde', 'Clases de lunes a viernes en horario vespertino (13:00-18:00)', 
    1, 1, 1, 1, 1, 0, 0, 'Tarde', 1),
('LV-NOC', 'Lunes a Viernes - Noche', 'Clases de lunes a viernes en horario nocturno (18:00-22:00)', 
    1, 1, 1, 1, 1, 0, 0, 'Noche', 1),
('LMV', 'Lunes, Miércoles, Viernes', 'Clases lunes, miércoles y viernes', 
    1, 0, 1, 0, 1, 0, 0, 'Mixto', 1),
('MJ', 'Martes y Jueves', 'Clases martes y jueves', 
    0, 1, 0, 1, 0, 0, 0, 'Mixto', 1),
('SAB', 'Sábados', 'Clases únicamente los sábados', 
    0, 0, 0, 0, 0, 1, 0, 'Mixto', 1),
('DOM', 'Domingos', 'Clases únicamente los domingos', 
    0, 0, 0, 0, 0, 0, 1, 'Mixto', 1),
('FS', 'Fin de Semana', 'Clases sábados y domingos', 
    0, 0, 0, 0, 0, 1, 1, 'Mixto', 1);

-- =====================================================
-- STORED PROCEDURES ÚTILES
-- =====================================================

-- Procedimiento: Copiar cursos de carrera a sección
GO
CREATE PROCEDURE sp_CopyCourseFromCareerToSection
    @SectionId INT,
    @CreatedBy INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @CareerId INT;
    DECLARE @CurrentSemester INT;
    
    -- Obtener la carrera y semestre actual de la sección
    SELECT @CareerId = CareerId, @CurrentSemester = CurrentSemester 
    FROM Sections 
    WHERE SectionId = @SectionId;
    
    -- Copiar cursos activos de la carrera a la sección
    -- Solo los cursos correspondientes al semestre actual
    INSERT INTO SectionCourses (SectionId, CourseId, Semester, CreatedBy)
    SELECT 
        @SectionId,
        cc.CourseId,
        cc.Semester,
        @CreatedBy
    FROM CareerCourses cc
    WHERE cc.CareerId = @CareerId
        AND cc.Semester = @CurrentSemester
        AND cc.IsActive = 1
        AND NOT EXISTS (
            SELECT 1 FROM SectionCourses sc 
            WHERE sc.SectionId = @SectionId 
                AND sc.CourseId = cc.CourseId
        );
    
    SELECT @@ROWCOUNT AS CoursesAdded;
END;
GO

-- Procedimiento: Validar conflictos de horario para docente
GO
CREATE PROCEDURE sp_ValidateTeacherScheduleConflict
    @ScheduleId INT,
    @TeacherId INT,
    @DayOfWeek INT,
    @StartTime TIME,
    @EndTime TIME,
    @ScheduleDetailId INT = NULL,  -- NULL para nueva asignación, ID para edición
    @HasConflict BIT OUTPUT,
    @ConflictDetails NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ConflictScheduleId INT;
    DECLARE @ConflictCourse NVARCHAR(150);
    DECLARE @ConflictSection NVARCHAR(100);
    
    -- Verificar si el docente ya tiene asignación en ese día y hora
    SELECT TOP 1
        @ConflictScheduleId = s.ScheduleId,
        @ConflictCourse = c.CourseName,
        @ConflictSection = sec.SectionName
    FROM ScheduleDetails sd
    INNER JOIN Schedules s ON sd.ScheduleId = s.ScheduleId
    INNER JOIN Courses c ON sd.CourseId = c.CourseId
    INNER JOIN Sections sec ON s.SectionId = sec.SectionId
    WHERE sd.TeacherId = @TeacherId
        AND sd.DayOfWeek = @DayOfWeek
        AND sd.IsActive = 1
        AND s.IsActive = 1
        AND s.ScheduleId <> @ScheduleId
        AND (@ScheduleDetailId IS NULL OR sd.ScheduleDetailId <> @ScheduleDetailId)
        AND (
            -- Verificar solapamiento de horarios
            (@StartTime >= sd.StartTime AND @StartTime < sd.EndTime)
            OR (@EndTime > sd.StartTime AND @EndTime <= sd.EndTime)
            OR (@StartTime <= sd.StartTime AND @EndTime >= sd.EndTime)
        );
    
    IF @ConflictScheduleId IS NOT NULL
    BEGIN
        SET @HasConflict = 1;
        SET @ConflictDetails = 'El docente ya tiene asignado: ' + @ConflictCourse + ' en ' + @ConflictSection;
    END
    ELSE
    BEGIN
        SET @HasConflict = 0;
        SET @ConflictDetails = NULL;
    END
END;
GO

-- Procedimiento: Validar que curso pertenezca a tipo de horario correcto
GO
CREATE PROCEDURE sp_ValidateCourseScheduleTypeCompatibility
    @ScheduleId INT,
    @CourseId INT,
    @DayOfWeek INT,
    @IsCompatible BIT OUTPUT,
    @ErrorMessage NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ScheduleTypeId INT;
    DECLARE @IncludesDay BIT = 0;
    
    -- Obtener el tipo de horario de la sección
    SELECT @ScheduleTypeId = s.ScheduleTypeId
    FROM Schedules sch
    INNER JOIN Sections s ON sch.SectionId = s.SectionId
    WHERE sch.ScheduleId = @ScheduleId;
    
    -- Verificar si el día está incluido en el tipo de horario
    SELECT @IncludesDay = 
        CASE @DayOfWeek
            WHEN 1 THEN IncludesMonday
            WHEN 2 THEN IncludesTuesday
            WHEN 3 THEN IncludesWednesday
            WHEN 4 THEN IncludesThursday
            WHEN 5 THEN IncludesFriday
            WHEN 6 THEN IncludesSaturday
            WHEN 7 THEN IncludesSunday
        END
    FROM ScheduleTypes
    WHERE ScheduleTypeId = @ScheduleTypeId;
    
    IF @IncludesDay = 1
    BEGIN
        SET @IsCompatible = 1;
        SET @ErrorMessage = NULL;
    END
    ELSE
    BEGIN
        SET @IsCompatible = 0;
        SET @ErrorMessage = 'El día seleccionado no está disponible para el tipo de horario de esta sección.';
    END
END;
GO

-- Procedimiento: Obtener cursos disponibles para un docente en una sección
GO
CREATE PROCEDURE sp_GetAvailableCoursesForTeacherInSection
    @TeacherId INT,
    @SectionId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Obtener cursos que:
    -- 1. El docente puede impartir (TeacherCourses)
    -- 2. Están asignados a la sección (SectionCourses)
    -- 3. Están activos
    SELECT DISTINCT
        c.CourseId,
        c.CourseCode,
        c.CourseName,
        c.Credits,
        c.TotalHours,
        tc.YearsOfExperience
    FROM Courses c
    INNER JOIN TeacherCourses tc ON c.CourseId = tc.CourseId
    INNER JOIN SectionCourses sc ON c.CourseId = sc.CourseId
    WHERE tc.TeacherId = @TeacherId
        AND sc.SectionId = @SectionId
        AND c.IsActive = 1
        AND tc.IsActive = 1
        AND sc.IsActive = 1
    ORDER BY c.CourseName;
END;
GO

-- Procedimiento: Generar código automático para entidades
GO
CREATE PROCEDURE sp_GenerateEntityCode
    @EntityType NVARCHAR(20),  -- 'COORD', 'TEACH', 'COURSE', 'CAREER', 'SECTION', 'SCHEDULE'
    @LocationCode NVARCHAR(10) = NULL,
    @GeneratedCode NVARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @NextNumber INT;
    DECLARE @Prefix NVARCHAR(10);
    
    -- Determinar prefijo según tipo de entidad
    SET @Prefix = CASE @EntityType
        WHEN 'COORD' THEN 'COORD-'
        WHEN 'TEACH' THEN 'TEACH-'
        WHEN 'COURSE' THEN 'CRS-'
        WHEN 'CAREER' THEN 'CAR-'
        WHEN 'SECTION' THEN 'SEC-'
        WHEN 'SCHEDULE' THEN 'SCH-'
        ELSE 'ENT-'
    END;
    
    -- Agregar código de ubicación si aplica
    IF @LocationCode IS NOT NULL AND @EntityType IN ('COORD', 'TEACH', 'SECTION', 'SCHEDULE')
        SET @Prefix = @Prefix + @LocationCode + '-';
    
    -- Obtener siguiente número según tipo
    SELECT @NextNumber = 
        CASE @EntityType
            WHEN 'COORD' THEN (SELECT ISNULL(MAX(CAST(RIGHT(CoordinatorCode, 4) AS INT)), 0) + 1 FROM Coordinators WHERE CoordinatorCode LIKE @Prefix + '%')
            WHEN 'TEACH' THEN (SELECT ISNULL(MAX(CAST(RIGHT(TeacherCode, 4) AS INT)), 0) + 1 FROM Teachers WHERE TeacherCode LIKE @Prefix + '%')
            WHEN 'COURSE' THEN (SELECT ISNULL(MAX(CAST(RIGHT(CourseCode, 4) AS INT)), 0) + 1 FROM Courses WHERE CourseCode LIKE @Prefix + '%')
            WHEN 'CAREER' THEN (SELECT ISNULL(MAX(CAST(RIGHT(CareerCode, 4) AS INT)), 0) + 1 FROM Careers WHERE CareerCode LIKE @Prefix + '%')
            WHEN 'SECTION' THEN (SELECT ISNULL(MAX(CAST(RIGHT(SectionCode, 4) AS INT)), 0) + 1 FROM Sections WHERE SectionCode LIKE @Prefix + '%')
            WHEN 'SCHEDULE' THEN (SELECT ISNULL(MAX(CAST(RIGHT(ScheduleCode, 4) AS INT)), 0) + 1 FROM Schedules WHERE ScheduleCode LIKE @Prefix + '%')
        END;
    
    -- Generar código con formato: PREFIX-0001
    SET @GeneratedCode = @Prefix + RIGHT('0000' + CAST(@NextNumber AS NVARCHAR), 4);
END;
GO

-- =====================================================
-- VISTAS ÚTILES PARA REPORTERÍA
-- =====================================================

-- Vista: Resumen de secciones con información completa
GO
CREATE VIEW vw_SectionsSummary
AS
SELECT 
    s.SectionId,
    s.SectionCode,
    s.SectionName,
    c.CareerCode,
    c.CareerName,
    l.LocationCode,
    l.LocationName,
    st.ScheduleTypeCode,
    st.ScheduleTypeName,
    coord.CoordinatorCode,
    coord.FullName AS CoordinatorName,
    s.CurrentSemester,
    s.AcademicYear,
    s.StudentCount,
    s.MaxCapacity,
    s.IsActive,
    s.StartDate,
    s.EndDate
FROM Sections s
INNER JOIN Careers c ON s.CareerId = c.CareerId
INNER JOIN Locations l ON s.LocationId = l.LocationId
INNER JOIN ScheduleTypes st ON s.ScheduleTypeId = st.ScheduleTypeId
INNER JOIN Coordinators coord ON s.CoordinatorId = coord.CoordinatorId;
GO

-- Vista: Docentes con información completa
GO
CREATE VIEW vw_TeachersSummary
AS
SELECT 
    t.TeacherId,
    t.TeacherCode,
    t.FullName,
    t.Email,
    t.Phone,
    t.AcademicTitle,
    t.Specialization,
    l.LocationCode,
    l.LocationName,
    t.ContractType,
    t.IsCollegiateActive,
    t.CollegiateNumber,
    coord.CoordinatorCode AS RegisteredByCode,
    coord.FullName AS RegisteredByName,
    t.IsActive,
    t.HireDate,
    (SELECT COUNT(*) FROM TeacherCourses tc WHERE tc.TeacherId = t.TeacherId AND tc.IsActive = 1) AS TotalCoursesAssigned
FROM Teachers t
INNER JOIN Locations l ON t.LocationId = l.LocationId
LEFT JOIN Coordinators coord ON t.RegisteredByCoordinatorId = coord.CoordinatorId;
GO

-- Vista: Horarios detallados
GO
CREATE VIEW vw_ScheduleDetailsSummary
AS
SELECT 
    sd.ScheduleDetailId,
    sch.ScheduleCode,
    sch.ScheduleName,
    sec.SectionCode,
    sec.SectionName,
    c.CourseCode,
    c.CourseName,
    t.TeacherCode,
    t.FullName AS TeacherName,
    sd.DayOfWeek,
    CASE sd.DayOfWeek
        WHEN 1 THEN 'Lunes'
        WHEN 2 THEN 'Martes'
        WHEN 3 THEN 'Miércoles'
        WHEN 4 THEN 'Jueves'
        WHEN 5 THEN 'Viernes'
        WHEN 6 THEN 'Sábado'
        WHEN 7 THEN 'Domingo'
    END AS DayName,
    sd.StartTime,
    sd.EndTime,
    sd.Classroom,
    sd.Building,
    l.LocationName,
    sch.AcademicYear,
    sch.Semester,
    sd.IsActive
FROM ScheduleDetails sd
INNER JOIN Schedules sch ON sd.ScheduleId = sch.ScheduleId
INNER JOIN Sections sec ON sch.SectionId = sec.SectionId
INNER JOIN Courses c ON sd.CourseId = c.CourseId
INNER JOIN Teachers t ON sd.TeacherId = t.TeacherId
INNER JOIN Locations l ON sch.LocationId = l.LocationId;
GO