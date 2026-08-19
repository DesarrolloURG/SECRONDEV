----1. Creación de la tabla de Bancos ------
-- TABLA: Banks (Bancos - Catálogo Nacional)
CREATE TABLE Banks (
    BankId INT IDENTITY(1,1) PRIMARY KEY,
    BankCode VARCHAR(10) NOT NULL UNIQUE,              -- Código del banco
    BankName VARCHAR(150) NOT NULL,                    -- Nombre del banco
    IsActive BIT DEFAULT 1
);

-- TABLA: AccountTypes (Tipos de Cuenta Bancaria)
CREATE TABLE BanksAccountTypes (
    BanksAccountTypeId   INT IDENTITY(1,1) PRIMARY KEY,
    BanksAccountTypeCode VARCHAR(10)  NOT NULL UNIQUE, -- Ej: MON, AHO
    BanksAccountTypeName VARCHAR(50)  NOT NULL,        -- Ej: MONETARIA, AHORRO
    IsActive        BIT          NOT NULL DEFAULT 1
);

----2. Creación la tabla para los tipos de bancos ------
-- Ejemplo de catálogo inicial (opcional)
INSERT INTO BanksAccountTypes (BanksAccountTypeCode, BanksAccountTypeName)
VALUES
('MON', 'MONETARIA'),
('AHO', 'AHORRO');