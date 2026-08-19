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



-- =========================================================
-- Catálogo de roles para asignación de personal a sedes
-- =========================================================
CREATE TABLE LocationStaffRoles
(
    RoleTypeId  TINYINT IDENTITY(1,1) PRIMARY KEY,
    RoleName    VARCHAR(50) NOT NULL,
    IsActive    BIT NOT NULL DEFAULT 1,
    CONSTRAINT UX_LocationStaffRoles_RoleName UNIQUE (RoleName)
);
GO

INSERT INTO LocationStaffRoles (RoleName, IsActive)
VALUES
    ('COORDINADOR', 1),
    ('ASISTENTE', 1);
GO

-- =========================================================
-- Asignación de personal (coordinadores/asistentes) a sedes
-- =========================================================
CREATE TABLE LocationStaffAssignments
(
    AssignmentId    INT IDENTITY(1,1) PRIMARY KEY,
    LocationId      INT NOT NULL,
    UserId          INT NOT NULL,
    RoleTypeId      TINYINT NOT NULL,
    IsActive        BIT NOT NULL DEFAULT 1,
    CreatedBy       INT NOT NULL,
    CreatedDate     DATETIME NOT NULL DEFAULT GETDATE(),
    ModifiedBy      INT NULL,
    ModifiedDate    DATETIME NULL,

    CONSTRAINT FK_LocationStaffAssignments_Location
        FOREIGN KEY (LocationId) REFERENCES Locations(LocationId),

    CONSTRAINT FK_LocationStaffAssignments_User
        FOREIGN KEY (UserId) REFERENCES Users(UserId),

    CONSTRAINT FK_LocationStaffAssignments_RoleType
        FOREIGN KEY (RoleTypeId) REFERENCES LocationStaffRoles(RoleTypeId),

    CONSTRAINT FK_LocationStaffAssignments_CreatedBy
        FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),

    CONSTRAINT FK_LocationStaffAssignments_ModifiedBy
        FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId)
);
GO

-- Rol excluyente por sede: un usuario solo puede tener 1 rol activo por sede
CREATE UNIQUE INDEX UX_LocationStaffAssignments_Active
ON LocationStaffAssignments (LocationId, UserId)
WHERE IsActive = 1;
GO

CREATE INDEX IX_LocationStaffAssignments_UserId
ON LocationStaffAssignments (UserId)
WHERE IsActive = 1;
GO

