-- Tabla de Proveedores
CREATE TABLE Suppliers (
    SupplierId INT IDENTITY(1,1) PRIMARY KEY,
    SupplierCode VARCHAR(20) NOT NULL UNIQUE,
    SupplierName VARCHAR(200) NOT NULL,                -- Nombre comercial
    LegalName VARCHAR(200) NOT NULL,                   -- Razón social
    TaxId VARCHAR(50),                                 -- NIT
    
    -- Contacto
    ContactName VARCHAR(150),
    Phone VARCHAR(15) NOT NULL,
    Phone2 VARCHAR(15),
    Email VARCHAR(100),
    Address VARCHAR(200),
    
    -- Información comercial
    CommercialActivity VARCHAR(200) NOT NULL,
    Classification VARCHAR(100) NOT NULL,
    
    -- Información bancaria
    BankAccountNumber VARCHAR(75),
    BankName VARCHAR(50),
    
    -- Control
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT,
    ModifiedDate DATETIME,
    ModifiedBy INT,
    
    CONSTRAINT FK_Suppliers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Suppliers_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId)
);

SELECT * FROM Suppliers ORDER BY SupplierName