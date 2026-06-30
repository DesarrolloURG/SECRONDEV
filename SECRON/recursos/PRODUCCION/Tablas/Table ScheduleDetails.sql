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