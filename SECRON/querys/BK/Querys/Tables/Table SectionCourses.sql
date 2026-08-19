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

SELECT * FROM SectionCourses