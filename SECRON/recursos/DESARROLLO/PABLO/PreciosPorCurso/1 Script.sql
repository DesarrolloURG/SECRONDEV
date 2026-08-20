-- 1. Relación Sede-Carrera
CREATE TABLE LocationCareers (
    LocationCareerId INT IDENTITY(1,1) PRIMARY KEY,
    LocationId INT NOT NULL,
    CareerId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy INT NOT NULL,
    ModifiedDate DATETIME NULL,
    ModifiedBy INT NULL,
    CONSTRAINT FK_LocationCareers_Locations FOREIGN KEY (LocationId) REFERENCES Locations(LocationId),
    CONSTRAINT FK_LocationCareers_Careers FOREIGN KEY (CareerId) REFERENCES Careers(CareerId),
    CONSTRAINT UQ_LocationCareers UNIQUE (LocationId, CareerId)
);

-- 2. Catálogo de modalidades
CREATE TABLE CourseModalities (
    ModalityId INT IDENTITY(1,1) PRIMARY KEY,
    ModalityCode VARCHAR(20) NOT NULL,
    ModalityName VARCHAR(50) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy INT NOT NULL,
    ModifiedDate DATETIME NULL,
    ModifiedBy INT NULL,
    CONSTRAINT UQ_CourseModalities_Code UNIQUE (ModalityCode)
);

-- 3. Combinación válida (Master)
CREATE TABLE CourseLocationPricingMaster (
    CourseLocationPricingId INT IDENTITY(1,1) PRIMARY KEY,
    CareerCourseId INT NOT NULL,
    LocationId INT NOT NULL,
    ModalityId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy INT NOT NULL,
    ModifiedDate DATETIME NULL,
    ModifiedBy INT NULL,
    CONSTRAINT FK_CLPMaster_CareerCourses FOREIGN KEY (CareerCourseId) REFERENCES CareerCourses(CareerCourseId),
    CONSTRAINT FK_CLPMaster_Locations FOREIGN KEY (LocationId) REFERENCES Locations(LocationId),
    CONSTRAINT FK_CLPMaster_Modalities FOREIGN KEY (ModalityId) REFERENCES CourseModalities(ModalityId),
    CONSTRAINT UQ_CLPMaster UNIQUE (CareerCourseId, LocationId, ModalityId)
);

-- 4. Historial de precios (Detail)
CREATE TABLE CourseLocationPricingDetail (
    CourseLocationPricingDetailId INT IDENTITY(1,1) PRIMARY KEY,
    CourseLocationPricingId INT NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    EffectiveFrom DATETIME NOT NULL DEFAULT GETDATE(),
    EffectiveTo DATETIME NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy INT NOT NULL,
    ModifiedDate DATETIME NULL,
    ModifiedBy INT NULL,
    CONSTRAINT FK_CLPDetail_Master FOREIGN KEY (CourseLocationPricingId) REFERENCES CourseLocationPricingMaster(CourseLocationPricingId)
);