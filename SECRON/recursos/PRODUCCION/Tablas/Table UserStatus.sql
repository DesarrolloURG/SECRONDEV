-- Tabla de Estados de Usuario
CREATE TABLE UserStatus (
    StatusId INT IDENTITY(1,1) PRIMARY KEY,
    StatusName VARCHAR(25) NOT NULL UNIQUE,  -- 'Activo', 'Inactivo', 'Suspendido', 'Temporal'
    Description VARCHAR(255),
    IsActive BIT DEFAULT 1
);
SELECT * FROM UserStatus