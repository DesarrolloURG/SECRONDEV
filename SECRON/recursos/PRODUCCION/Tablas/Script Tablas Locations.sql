------------------------------------- Tabla de Categorías de Establecimientos/Sedes -------------------------------

-- Categorías de Sedes (Plantillas: PEQUEÑA, MEDIANA, GRANDE)
CREATE TABLE LocationCategories (
    LocationCategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryCode VARCHAR(20) NOT NULL UNIQUE,
    CategoryName VARCHAR(100) NOT NULL,
    Description VARCHAR(255),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT,
    
    CONSTRAINT FK_LocationCategories_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId)
);

-- Datos iniciales
INSERT INTO LocationCategories (CategoryCode, CategoryName, Description) VALUES
('000001', 'SEDE PEQUEÑA', 'PLANTILLA PARA SEDES CON INVENTARIO REDUCIDO'),
('000002', 'SEDE MEDIANA', 'PLANTILLA PARA SEDES CON INVENTARIO MODERADO'),
('000003', 'SEDE GRANDE', 'PLANTILLA PARA SEDES CON INVENTARIO AMPLIO');

------------------------------------- Tabla de Locations -------------------------------

CREATE TABLE Locations (
    LocationId INT IDENTITY(1,1) PRIMARY KEY,
    LocationCode VARCHAR(10) NOT NULL UNIQUE,
    LocationName VARCHAR(100) NOT NULL,
    Address VARCHAR(255),
    City VARCHAR(100),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT,
    ModifiedDate DATETIME,
    ModifiedBy INT,
	LocationCategoryId INT NULL,
	PrimaryWarehouseId INT NULL
);

ALTER TABLE Locations 
ADD CONSTRAINT FK_Locations_Category 
    FOREIGN KEY (LocationCategoryId) REFERENCES LocationCategories(LocationCategoryId);

ALTER TABLE Locations 
ADD CONSTRAINT FK_Locations_PrimaryWarehouse 
    FOREIGN KEY (PrimaryWarehouseId) REFERENCES Warehouses(WarehouseId);


SELECT * FROM Locations