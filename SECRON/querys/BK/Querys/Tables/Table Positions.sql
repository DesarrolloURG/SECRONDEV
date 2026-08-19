-- Positions (Puestos de Trabajo)
CREATE TABLE Positions (
    PositionId INT IDENTITY(1,1) PRIMARY KEY,
    PositionCode VARCHAR(10) NOT NULL UNIQUE,
    PositionName VARCHAR(100) NOT NULL,
    Description VARCHAR(255),
    DepartmentId INT NOT NULL,
    SalaryRange VARCHAR(50),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    
    CONSTRAINT FK_Positions_Department FOREIGN KEY (DepartmentId) REFERENCES Departments(DepartmentId)
);
SELECT * FROM Positions