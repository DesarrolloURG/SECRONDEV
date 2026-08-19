-- Tabla de Requisicion de Compras Master
CREATE TABLE PurchaseRequisitionMaster (
    RequisitionMasterId INT IDENTITY(1,1) PRIMARY KEY,
    RequisitionNumber VARCHAR(30) NOT NULL UNIQUE,
    RequisitionDate DATE NOT NULL,
    ResponsibleUserId INT NOT NULL,
    StatusId INT NOT NULL,
    TotalBudget DECIMAL(18,2) DEFAULT 0,
    
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT NOT NULL,
    ModifiedDate DATETIME,
    ModifiedBy INT,
    IsActive BIT DEFAULT 1,
    
    CONSTRAINT FK_PurchaseRequisition_ResponsibleUser FOREIGN KEY (ResponsibleUserId) REFERENCES Users(UserId),
    CONSTRAINT FK_PurchaseRequisition_Status FOREIGN KEY (StatusId) REFERENCES PurchaseRequestStatus(StatusId),
    CONSTRAINT FK_PurchaseRequisition_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_PurchaseRequisition_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId)
);