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