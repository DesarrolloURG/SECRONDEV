-- Tabla de los tipos de movimientos que se tienen
CREATE TABLE MovementTypes (
    MovementTypeId INT IDENTITY(1,1) PRIMARY KEY,
    TypeCode VARCHAR(20) NOT NULL UNIQUE,
    TypeName VARCHAR(50) NOT NULL,
    AffectsStock VARCHAR(10) NOT NULL,                 -- '+', '-', '0'
    RequiresSupplier BIT DEFAULT 0,
    RequiresDestination BIT DEFAULT 0,
    IsActive BIT DEFAULT 1
);

SELECT * FROM MovementTypes

INSERT INTO MovementTypes (TypeCode, TypeName, AffectsStock, RequiresSupplier, RequiresDestination) VALUES
('COMPRA', 'COMPRA A PROVEEDOR', '+', 1, 0),
('ENTRADA_AJUSTE', 'AJUSTE DE ENTRADA', '+', 0, 0),
('SALIDA', 'SALIDA DE INVENTARIO', '-', 0, 0),
('SALIDA_AJUSTE', 'AJUSTE DE SALIDA', '-', 0, 0),
('TRANSFERENCIA', 'TRANSFERENCIA ENTRE SEDES', '0', 0, 1),
('DEVOLUCION', 'DEVOLUCIÓN A PROVEEDOR', '-', 1, 0);