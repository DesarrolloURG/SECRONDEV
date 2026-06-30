-- 1. TABLA: Coordinators (Coordinadores Académicos)
-- -----------------------------------------------------
CREATE TABLE Coordinators (
    CoordinatorId INT IDENTITY(1,1) PRIMARY KEY,
    CoordinatorCode NVARCHAR(20) NOT NULL UNIQUE,
    
    -- Información Personal
    FullName NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(20),
    Email NVARCHAR(100),
    DPI NVARCHAR(20),
    NIT NVARCHAR(20),
    
    -- Información Académica/Profesional
    AcademicTitle NVARCHAR(255),
    Specialization NVARCHAR(255),
    
    -- Información Bancaria
    BankAccountNumber NVARCHAR(30),
    BankId INT,
    
    -- Asignación de Sede principal
    LocationId INT NOT NULL,
    
    -- Relación con Usuario (opcional)
    UserId INT NULL,  -- NULL si no tiene usuario del sistema
    
    -- Control
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT,
    ModifiedDate DATETIME,
    ModifiedBy INT,
    
    CONSTRAINT FK_Coordinators_Bank FOREIGN KEY (BankId) REFERENCES Banks(BankId),
    CONSTRAINT FK_Coordinators_Location FOREIGN KEY (LocationId) REFERENCES Locations(LocationId),
    CONSTRAINT FK_Coordinators_User FOREIGN KEY (UserId) REFERENCES Users(UserId),
    CONSTRAINT FK_Coordinators_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Coordinators_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId)
);

SELECT * FROM Coordinators