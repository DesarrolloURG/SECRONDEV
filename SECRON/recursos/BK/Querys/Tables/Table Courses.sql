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

SELECT * FROM Courses