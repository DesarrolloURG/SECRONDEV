-- Tabla de Unidades de Medida
CREATE TABLE MeasurementUnits (
    UnitId INT IDENTITY(1,1) PRIMARY KEY,
    UnitCode VARCHAR(10) NOT NULL UNIQUE,
    UnitName VARCHAR(50) NOT NULL,
    Abbreviation VARCHAR(10) NOT NULL,
    IsActive BIT DEFAULT 1
);
