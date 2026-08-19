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

SELECT * FROM Careers