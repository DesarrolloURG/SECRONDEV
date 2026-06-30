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

SELECT * FROM ScheduleTypes