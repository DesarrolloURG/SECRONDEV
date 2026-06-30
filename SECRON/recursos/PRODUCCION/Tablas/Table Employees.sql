-- TABLA: Employees (Empleados)
CREATE TABLE Employees (
    EmployeeId INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeCode VARCHAR(20) NOT NULL UNIQUE,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    FullName AS (FirstName + ' ' + LastName) PERSISTED,
    IdentificationNumber VARCHAR(20) NULL, -- DPI/Cédula Corregir a NOT NULL
    Email VARCHAR(255) NULL,
	InstitutionalEmail VARCHAR(255) NULL, -- Corregir a NOT NULL
    Phone VARCHAR(20),
    MobilePhone VARCHAR(20),
    Address VARCHAR(500),
    BirthDate DATE,
    HireDate DATE NOT NULL,
    TerminationDate DATE,
    DepartmentId INT NOT NULL,
    PositionId INT NOT NULL,
    DirectSupervisorId INT, -- FK a otro empleado
    EmployeeStatusId INT NOT NULL,
    EmergencyContactName VARCHAR(200),
    EmergencyContactPhone VARCHAR(20),
    EmergencyContactRelation VARCHAR(50),
    Salary DECIMAL(10,2),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT,
    ModifiedDate DATETIME,
    ModifiedBy INT,
	LocationId INT NULL,
	TipoContratacion VARCHAR(20) NULL,
    
    CONSTRAINT FK_Employees_Department FOREIGN KEY (DepartmentId) REFERENCES Departments(DepartmentId),
    CONSTRAINT FK_Employees_Position FOREIGN KEY (PositionId) REFERENCES Positions(PositionId),
    CONSTRAINT FK_Employees_Supervisor FOREIGN KEY (DirectSupervisorId) REFERENCES Employees(EmployeeId),
    CONSTRAINT FK_Employees_Status FOREIGN KEY (EmployeeStatusId) REFERENCES EmployeeStatus(EmployeeStatusId),
	CONSTRAINT FK_Employees_Location FOREIGN KEY (LocationId) REFERENCES Locations(LocationId)
);

SELECT * FROM Employees order by EmployeeCode

--Script de modificación de Base de Datos para el cambio
--de calculo de salario para empleados.
ALTER TABLE Employees
ADD
    nominal_salary   DECIMAL(18,2) NULL,
    base_salary      DECIMAL(18,2) NULL,
    additional_bonus DECIMAL(18,2) NULL,
    legal_bonus      DECIMAL(18,2) NULL,
    IGSS             DECIMAL(18,2) NULL,
    ISR              DECIMAL(18,2) NULL,
    net_salary       DECIMAL(18,2) NULL;
 
ALTER TABLE Employees
DROP COLUMN Salary;
 
ALTER TABLE Employees
ADD
    IGSS_MANUAL   BIT NULL;