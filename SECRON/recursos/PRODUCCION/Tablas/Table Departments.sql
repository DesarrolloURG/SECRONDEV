-- TABLA: Departments
CREATE TABLE Departments (
    DepartmentId INT IDENTITY(1,1) PRIMARY KEY,
    LocationId INT NOT NULL, -- FK hacia Locations
    DepartmentCode VARCHAR(10) NOT NULL UNIQUE,
    DepartmentName VARCHAR(100) NOT NULL,
    Description VARCHAR(255),
    ManagerEmployeeId INT,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT,
    ModifiedDate DATETIME,
    ModifiedBy INT,
    
    CONSTRAINT FK_Departments_Location FOREIGN KEY (LocationId) REFERENCES Locations(LocationId)
);

SELECT * FROM Departments