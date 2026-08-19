-- Tabla de Solicitud de Compras Master
CREATE TABLE PurchaseRequestMaster (
    RequestMasterId INT IDENTITY(1,1) PRIMARY KEY,
    RequestNumber VARCHAR(30) NOT NULL UNIQUE,
    RequestDate DATE NOT NULL,
    ResponsibleUserId INT NOT NULL,
    StatusId INT NOT NULL,
    LocationId INT NOT NULL,
    DepartmentId INT NOT NULL,                     -- CORREGIDO: Era AcademicAreaId
    TotalBudget DECIMAL(18,2) DEFAULT 0,
    
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT NOT NULL,
    ModifiedDate DATETIME,
    ModifiedBy INT,
    IsActive BIT DEFAULT 1,
    
    CONSTRAINT FK_PurchaseRequest_ResponsibleUser FOREIGN KEY (ResponsibleUserId) REFERENCES Users(UserId),
    CONSTRAINT FK_PurchaseRequest_Status FOREIGN KEY (StatusId) REFERENCES PurchaseRequestStatus(StatusId),
    CONSTRAINT FK_PurchaseRequest_Location FOREIGN KEY (LocationId) REFERENCES Locations(LocationId),
    CONSTRAINT FK_PurchaseRequest_Department FOREIGN KEY (DepartmentId) REFERENCES Departments(DepartmentId),  -- CORREGIDO
    CONSTRAINT FK_PurchaseRequest_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_PurchaseRequest_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId)
);