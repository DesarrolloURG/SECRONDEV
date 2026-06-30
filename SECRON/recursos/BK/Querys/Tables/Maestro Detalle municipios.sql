CREATE TABLE Pais (
    PaisId INT IDENTITY(1,1) NOT NULL,
    NombrePais VARCHAR(100) NOT NULL,
    CodigoPais VARCHAR(10) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Pais_IsActive DEFAULT (1),
    CreatedDate DATETIME NOT NULL CONSTRAINT DF_Pais_CreatedDate DEFAULT (GETDATE()),
    CreatedBy INT NULL,
    ModifiedDate DATETIME NULL,
    ModifiedBy INT NULL,
    CONSTRAINT PK_Pais PRIMARY KEY (PaisId)
);
GO

CREATE TABLE Departamento (
    DepartamentoId INT IDENTITY(1,1) NOT NULL,
    PaisId INT NOT NULL,
    NombreDepartamento VARCHAR(100) NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Departamento_IsActive DEFAULT (1),
    CreatedDate DATETIME NOT NULL CONSTRAINT DF_Departamento_CreatedDate DEFAULT (GETDATE()),
    CreatedBy INT NULL,
    ModifiedDate DATETIME NULL,
    ModifiedBy INT NULL,
    CONSTRAINT PK_Departamento PRIMARY KEY (DepartamentoId),
    CONSTRAINT FK_Departamento_Pais FOREIGN KEY (PaisId)
        REFERENCES Pais(PaisId)
);
GO

CREATE TABLE Municipio (
    MunicipioId INT IDENTITY(1,1) NOT NULL,
    DepartamentoId INT NOT NULL,
    NombreMunicipio VARCHAR(100) NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Municipio_IsActive DEFAULT (1),
    CreatedDate DATETIME NOT NULL CONSTRAINT DF_Municipio_CreatedDate DEFAULT (GETDATE()),
    CreatedBy INT NULL,
    ModifiedDate DATETIME NULL,
    ModifiedBy INT NULL,
    CONSTRAINT PK_Municipio PRIMARY KEY (MunicipioId),
    CONSTRAINT FK_Municipio_Departamento FOREIGN KEY (DepartamentoId)
        REFERENCES Departamento(DepartamentoId)
);
GO

ALTER TABLE Locations
ADD MunicipioId INT NULL;
GO

ALTER TABLE Locations
ADD CONSTRAINT FK_Locations_Municipio
FOREIGN KEY (MunicipioId)
REFERENCES Municipio(MunicipioId);
GO

INSERT INTO Pais (NombrePais, CodigoPais)
VALUES ('Guatemala', 'GT');

INSERT INTO Departamento (PaisId, NombreDepartamento)
SELECT p.PaisId, d.NombreDepartamento
FROM Pais p
CROSS JOIN (
    VALUES
        ('Guatemala'),
        ('Jutiapa'),
        ('Escuintla'),
        ('Santa Rosa'),
        ('Chimaltenango'),
        ('Suchitepéquez')
) d(NombreDepartamento)
WHERE p.NombrePais = 'Guatemala';


INSERT INTO Municipio (DepartamentoId, NombreMunicipio)
SELECT d.DepartamentoId, m.NombreMunicipio
FROM Departamento d
JOIN (
    VALUES
        -- GUATEMALA
        ('Guatemala', 'Guatemala'),
        ('Guatemala', 'Santa Catarina Pinula'),
        ('Guatemala', 'San José Pinula'),
        ('Guatemala', 'San José del Golfo'),
        ('Guatemala', 'Palencia'),
        ('Guatemala', 'Chinautla'),
        ('Guatemala', 'San Pedro Ayampuc'),
        ('Guatemala', 'Mixco'),
        ('Guatemala', 'San Pedro Sacatepéquez'),
        ('Guatemala', 'San Juan Sacatepéquez'),
        ('Guatemala', 'San Raymundo'),
        ('Guatemala', 'Chuarrancho'),
        ('Guatemala', 'Fraijanes'),
        ('Guatemala', 'Amatitlán'),
        ('Guatemala', 'Villa Nueva'),
        ('Guatemala', 'Villa Canales'),
        ('Guatemala', 'San Miguel Petapa'),

        -- JUTIAPA
        ('Jutiapa', 'Jutiapa'),
        ('Jutiapa', 'El Progreso'),
        ('Jutiapa', 'Santa Catarina Mita'),
        ('Jutiapa', 'Agua Blanca'),
        ('Jutiapa', 'Asunción Mita'),
        ('Jutiapa', 'Yupiltepeque'),
        ('Jutiapa', 'Atescatempa'),
        ('Jutiapa', 'Jerez'),
        ('Jutiapa', 'El Adelanto'),
        ('Jutiapa', 'Zapotitlán'),
        ('Jutiapa', 'Comapa'),
        ('Jutiapa', 'Jalpatagua'),
        ('Jutiapa', 'Conguaco'),
        ('Jutiapa', 'Moyuta'),
        ('Jutiapa', 'Pasaco'),
        ('Jutiapa', 'San José Acatempa'),
        ('Jutiapa', 'Quesada'),

        -- ESCUINTLA
        ('Escuintla', 'Sipacate'),
        ('Escuintla', 'Escuintla'),
        ('Escuintla', 'Santa Lucía Cotzumalguapa'),
        ('Escuintla', 'La Democracia'),
        ('Escuintla', 'Siquinalá'),
        ('Escuintla', 'Masagua'),
        ('Escuintla', 'Tiquisate'),
        ('Escuintla', 'La Gomera'),
        ('Escuintla', 'Guanagazapa'),
        ('Escuintla', 'San José'),
        ('Escuintla', 'Iztapa'),
        ('Escuintla', 'Palín'),
        ('Escuintla', 'San Vicente Pacaya'),
        ('Escuintla', 'Nueva Concepción'),

        -- SANTA ROSA
        ('Santa Rosa', 'Cuilapa'),
        ('Santa Rosa', 'Barberena'),
        ('Santa Rosa', 'Santa Rosa de Lima'),
        ('Santa Rosa', 'Casillas'),
        ('Santa Rosa', 'San Rafael Las Flores'),
        ('Santa Rosa', 'Oratorio'),
        ('Santa Rosa', 'San Juan Tecuaco'),
        ('Santa Rosa', 'Chiquimulilla'),
        ('Santa Rosa', 'Taxisco'),
        ('Santa Rosa', 'Santa María Ixhuatán'),
        ('Santa Rosa', 'Guazacapán'),
        ('Santa Rosa', 'Santa Cruz Naranjo'),
        ('Santa Rosa', 'Pueblo Nuevo Viñas'),
        ('Santa Rosa', 'Nueva Santa Rosa'),

        -- CHIMALTENANGO
        ('Chimaltenango', 'Chimaltenango'),
        ('Chimaltenango', 'San José Poaquil'),
        ('Chimaltenango', 'San Martín Jilotepeque'),
        ('Chimaltenango', 'Comalapa'),
        ('Chimaltenango', 'Santa Apolonia'),
        ('Chimaltenango', 'Tecpán Guatemala'),
        ('Chimaltenango', 'Patzún'),
        ('Chimaltenango', 'Pochuta'),
        ('Chimaltenango', 'Patzicía'),
        ('Chimaltenango', 'Santa Cruz Balanyá'),
        ('Chimaltenango', 'Acatenango'),
        ('Chimaltenango', 'Yepocapa'),
        ('Chimaltenango', 'San Andrés Itzapa'),
        ('Chimaltenango', 'Parramos'),
        ('Chimaltenango', 'Zaragoza'),
        ('Chimaltenango', 'El Tejar'),

        -- SUCHITEPÉQUEZ
        ('Suchitepéquez', 'Mazatenango'),
        ('Suchitepéquez', 'Cuyotenango'),
        ('Suchitepéquez', 'San Francisco Zapotitlán'),
        ('Suchitepéquez', 'San Bernardino'),
        ('Suchitepéquez', 'San José El Ídolo'),
        ('Suchitepéquez', 'Santo Domingo Suchitepéquez'),
        ('Suchitepéquez', 'San Lorenzo'),
        ('Suchitepéquez', 'Samayac'),
        ('Suchitepéquez', 'San Pablo Jocopilas'),
        ('Suchitepéquez', 'San Antonio Suchitepéquez'),
        ('Suchitepéquez', 'San Miguel Panán'),
        ('Suchitepéquez', 'San Gabriel'),
        ('Suchitepéquez', 'Chicacao'),
        ('Suchitepéquez', 'Patulul'),
        ('Suchitepéquez', 'Santa Bárbara'),
        ('Suchitepéquez', 'San Juan Bautista'),
        ('Suchitepéquez', 'Santo Tomás La Unión'),
        ('Suchitepéquez', 'Zunilito'),
        ('Suchitepéquez', 'Pueblo Nuevo'),
        ('Suchitepéquez', 'Río Bravo'),
        ('Suchitepéquez', 'San José La Máquina')
) m(NombreDepartamento, NombreMunicipio)
    ON d.NombreDepartamento = m.NombreDepartamento
WHERE NOT EXISTS (
    SELECT 1
    FROM Municipio x
    WHERE x.DepartamentoId = d.DepartamentoId
      AND x.NombreMunicipio = m.NombreMunicipio
);

/* =========================================
   1) TABLES
   ========================================= */
EXEC sp_rename 'dbo.Pais', 'Country';
GO

EXEC sp_rename 'dbo.Departamento', 'Department';
GO

EXEC sp_rename 'dbo.Municipio', 'Municipality';
GO


/* =========================================
   2) COLUMNS - COUNTRY
   ========================================= */
EXEC sp_rename 'dbo.Country.PaisId', 'CountryId', 'COLUMN';
GO

EXEC sp_rename 'dbo.Country.NombrePais', 'CountryName', 'COLUMN';
GO

EXEC sp_rename 'dbo.Country.CodigoPais', 'CountryCode', 'COLUMN';
GO


/* =========================================
   3) COLUMNS - DEPARTMENT
   ========================================= */
EXEC sp_rename 'dbo.Department.DepartamentoId', 'DepartmentId', 'COLUMN';
GO

EXEC sp_rename 'dbo.Department.PaisId', 'CountryId', 'COLUMN';
GO

EXEC sp_rename 'dbo.Department.NombreDepartamento', 'DepartmentName', 'COLUMN';
GO


/* =========================================
   4) COLUMNS - MUNICIPALITY
   ========================================= */
EXEC sp_rename 'dbo.Municipality.MunicipioId', 'MunicipalityId', 'COLUMN';
GO

EXEC sp_rename 'dbo.Municipality.DepartamentoId', 'DepartmentId', 'COLUMN';
GO

EXEC sp_rename 'dbo.Municipality.NombreMunicipio', 'MunicipalityName', 'COLUMN';
GO


/* =========================================
   5) COLUMNS - LOCATIONS
   ========================================= */
EXEC sp_rename 'dbo.Locations.MunicipioId', 'MunicipalityId', 'COLUMN';
GO


/* =========================================
   6) PRIMARY KEYS
   ========================================= */
EXEC sp_rename 'dbo.PK_Pais', 'PK_Country', 'OBJECT';
GO

EXEC sp_rename 'dbo.PK_Departamento', 'PK_Department', 'OBJECT';
GO

EXEC sp_rename 'dbo.PK_Municipio', 'PK_Municipality', 'OBJECT';
GO


/* =========================================
   7) FOREIGN KEYS
   ========================================= */
EXEC sp_rename 'dbo.FK_Departamento_Pais', 'FK_Department_Country', 'OBJECT';
GO

EXEC sp_rename 'dbo.FK_Municipio_Departamento', 'FK_Municipality_Department', 'OBJECT';
GO

EXEC sp_rename 'dbo.FK_Locations_Municipio', 'FK_Locations_Municipality', 'OBJECT';
GO


/* =========================================
   8) DEFAULT CONSTRAINTS
   ========================================= */
EXEC sp_rename 'dbo.DF_Pais_IsActive', 'DF_Country_IsActive', 'OBJECT';
GO

EXEC sp_rename 'dbo.DF_Pais_CreatedDate', 'DF_Country_CreatedDate', 'OBJECT';
GO

EXEC sp_rename 'dbo.DF_Departamento_IsActive', 'DF_Department_IsActive', 'OBJECT';
GO

EXEC sp_rename 'dbo.DF_Departamento_CreatedDate', 'DF_Department_CreatedDate', 'OBJECT';
GO

EXEC sp_rename 'dbo.DF_Municipio_IsActive', 'DF_Municipality_IsActive', 'OBJECT';
GO

EXEC sp_rename 'dbo.DF_Municipio_CreatedDate', 'DF_Municipality_CreatedDate', 'OBJECT';
GO

------CAMBIO DE LA SECUENCIA DE LOCATION CATEGORY

ALTER TABLE LocationCategories ADD CONSTRAINT UQ_LocationCategories_CategoryCode UNIQUE (CategoryCode);

CREATE PROCEDURE SP_InsertLocationCategory
    @CategoryName VARCHAR(100),
    @Description VARCHAR(255) = NULL,
    @CreatedBy INT = NULL,
    @CategoryCode VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRAN;

    IF @CategoryCode IS NULL OR LTRIM(RTRIM(@CategoryCode)) = ''
    BEGIN
        DECLARE @NextCode INT;

        SELECT @NextCode = ISNULL(MAX(CAST(CategoryCode AS INT)), 0) + 1
        FROM LocationCategories WITH (UPDLOCK, HOLDLOCK)
        WHERE ISNUMERIC(CategoryCode) = 1;

        SET @CategoryCode = RIGHT(REPLICATE('0', 6) + CAST(@NextCode AS VARCHAR(6)), 6);
    END

    INSERT INTO LocationCategories
    (
        CategoryCode,
        CategoryName,
        Description,
        IsActive,
        CreatedDate,
        CreatedBy
    )
    VALUES
    (
        @CategoryCode,
        @CategoryName,
        @Description,
        1,
        GETDATE(),
        @CreatedBy
    );

    COMMIT TRAN;
END

--ejemplo de ejecucion
--EXEC SP_InsertLocationCategory
--    @CategoryName = 'SEDE PEQUEÑA',
--    @Description = 'PLANTILLA PARA SEDES CON INVENTARIO REDUCICO';

SELECT * FROM EMPLOYEES