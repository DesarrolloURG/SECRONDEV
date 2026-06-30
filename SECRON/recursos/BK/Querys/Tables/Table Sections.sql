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