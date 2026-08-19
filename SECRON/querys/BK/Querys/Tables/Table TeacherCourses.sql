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