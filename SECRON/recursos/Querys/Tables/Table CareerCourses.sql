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

SELECT * FROM CareerCourses