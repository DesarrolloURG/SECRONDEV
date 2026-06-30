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

SELECT * FROM Teachers