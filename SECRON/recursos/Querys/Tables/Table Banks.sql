-- TABLA: Banks (Bancos - Catálogo Nacional)
CREATE TABLE Banks (
    BankId INT IDENTITY(1,1) PRIMARY KEY,
    BankCode VARCHAR(10) NOT NULL UNIQUE,              -- Código del banco
    BankName VARCHAR(150) NOT NULL,                    -- Nombre del banco
    IsActive BIT DEFAULT 1
);
SELECT * FROM Banks