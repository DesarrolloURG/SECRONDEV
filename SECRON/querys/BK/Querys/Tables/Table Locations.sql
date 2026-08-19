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