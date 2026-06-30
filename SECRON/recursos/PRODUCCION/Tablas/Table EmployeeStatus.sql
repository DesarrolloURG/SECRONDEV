-- TABLA: EmployeeStatus (Estados de Empleados)
CREATE TABLE EmployeeStatus (
    EmployeeStatusId INT IDENTITY(1,1) PRIMARY KEY,
    StatusName VARCHAR(25) NOT NULL UNIQUE, -- 'Activo', 'Inactivo', 'Suspendido', 'Temporal'
    Description VARCHAR(255),
    IsActive BIT DEFAULT 1
);
SELECT * FROM EmployeeStatus