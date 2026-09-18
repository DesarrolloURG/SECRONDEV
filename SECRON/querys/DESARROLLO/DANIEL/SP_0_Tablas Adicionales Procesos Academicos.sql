--------------------- 1. PERIODO DE TIEMPO HABILITADO PARA HORARIOS -----------------
CREATE TABLE AcademicProcesses_SchedulePeriods (
    SchedulePeriodId INT IDENTITY(1,1) PRIMARY KEY,
    StartDate        DATETIME NOT NULL,
    EndDate          DATETIME NOT NULL,
    IsActive         BIT NOT NULL DEFAULT 1,
    Notes            NVARCHAR(200) NULL,
    CreatedBy        INT NULL,
    CreatedDate      DATETIME NOT NULL DEFAULT GETDATE(),
    ModifiedBy       INT NULL,
    ModifiedDate     DATETIME NULL,

    CONSTRAINT FK_AcademicProcesses_SchedulePeriods_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_AcademicProcesses_SchedulePeriods_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId)
);

--------------------- 2. CLASIFICACIÓN DE SALONES POR TAMAÑOS -----------------

CREATE TABLE ClassroomSizeClassifications (
    ClassificationId    INT IDENTITY(1,1) PRIMARY KEY,
    ClassificationCode  NVARCHAR(20) NOT NULL UNIQUE,   -- PEQ / MED / GRA
    ClassificationName  NVARCHAR(100) NOT NULL,         -- PEQUEÑO / MEDIANO / GRANDE
    Description         NVARCHAR(255) NULL,
    DisplayOrder        INT NOT NULL DEFAULT 0,
    IsActive            BIT NOT NULL DEFAULT 1,
    CreatedDate         DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy           INT NULL,
    ModifiedDate        DATETIME NULL,
    ModifiedBy          INT NULL
);

CREATE TABLE LocationClassroomCapacities (
    LocationClassroomCapacityId INT IDENTITY(1,1) PRIMARY KEY,
    LocationId       INT NOT NULL,
    ClassificationId INT NOT NULL,
    Capacity         INT NOT NULL,      -- ej: Pequeño = 25 en Sede A, 35 en Sede B
    IsActive         BIT NOT NULL DEFAULT 1,
    CreatedDate      DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy        INT NULL,
    ModifiedDate     DATETIME NULL,
    ModifiedBy       INT NULL,

    CONSTRAINT FK_LocationClassroomCapacities_Location FOREIGN KEY (LocationId) REFERENCES Locations(LocationId),
    CONSTRAINT FK_LocationClassroomCapacities_Classification FOREIGN KEY (ClassificationId) REFERENCES ClassroomSizeClassifications(ClassificationId),
    CONSTRAINT UQ_LocationClassroomCapacities UNIQUE (LocationId, ClassificationId)
);

--------------------- 3. TABLA PARA SALONES FÍSICOS -----------------
CREATE TABLE Classrooms (
    ClassroomId      INT IDENTITY(1,1) PRIMARY KEY,
    ClassroomCode    NVARCHAR(20) NOT NULL,
    ClassroomName    NVARCHAR(100) NOT NULL,
    LocationId       INT NOT NULL,
    ClassificationId INT NOT NULL,
    Building         NVARCHAR(100) NULL,
    IsActive         BIT NOT NULL DEFAULT 1,
    CreatedDate      DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedBy        INT NULL,
    ModifiedDate     DATETIME NULL,
    ModifiedBy       INT NULL,

    CONSTRAINT FK_Classrooms_Location FOREIGN KEY (LocationId) REFERENCES Locations(LocationId),
    CONSTRAINT FK_Classrooms_Classification FOREIGN KEY (ClassificationId) REFERENCES ClassroomSizeClassifications(ClassificationId),
    CONSTRAINT UQ_Classrooms_Code UNIQUE (LocationId, ClassroomCode)
);