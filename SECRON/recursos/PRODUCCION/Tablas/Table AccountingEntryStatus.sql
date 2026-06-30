-- TABLA CATÁLOGO: de Estados AccountingEntryStatus
CREATE TABLE AccountingEntryStatus (
    StatusId INT IDENTITY(1,1) PRIMARY KEY,
    StatusCode VARCHAR(20) NOT NULL UNIQUE,            -- 'APROBADO', 'ANULADO', 'REVERSION'
    StatusName VARCHAR(50) NOT NULL,
    IsActive BIT DEFAULT 1
);

-- DATOS INICIALES
INSERT INTO AccountingEntryStatus (StatusCode, StatusName) VALUES
('APROBADO', 'APROBADO'),
('ANULADO', 'ANULADO'),
('REVERSION', 'REVERSIÓN');

SELECT * FROM AccountingEntryStatus