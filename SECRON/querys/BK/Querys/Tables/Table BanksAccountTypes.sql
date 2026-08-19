-- TABLA: AccountTypes (Tipos de Cuenta Bancaria)
CREATE TABLE BanksAccountTypes (
    BanksAccountTypeId   INT IDENTITY(1,1) PRIMARY KEY,
    BanksAccountTypeCode VARCHAR(10)  NOT NULL UNIQUE, -- Ej: MON, AHO
    BanksAccountTypeName VARCHAR(50)  NOT NULL,        -- Ej: MONETARIA, AHORRO
    IsActive        BIT          NOT NULL DEFAULT 1
);


-- Ejemplo de catálogo inicial (opcional)
INSERT INTO BanksAccountTypes (BanksAccountTypeCode, BanksAccountTypeName)
VALUES
('MON', 'MONETARIA'),
('AHO', 'AHORRO');

SELECT * FROM BanksAccountTypes