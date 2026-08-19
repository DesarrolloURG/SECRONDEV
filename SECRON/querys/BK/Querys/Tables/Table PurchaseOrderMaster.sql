-- Tabla de Orden de Compra Master
CREATE TABLE PurchaseOrderMaster (
    PurchaseOrderId INT IDENTITY(1,1) PRIMARY KEY,
    OrderNumber VARCHAR(30) NOT NULL UNIQUE,
    OrderDate DATE NOT NULL,
    RequisitionMasterId INT NOT NULL,
    SupplierId INT NOT NULL,
    DeliveryLocationId INT NOT NULL,
    ExpectedDeliveryDate DATE,
    TotalAmount DECIMAL(18,2) DEFAULT 0,
    StatusId INT NOT NULL,
    
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT NOT NULL,
    ApprovedDate DATETIME,
    ApprovedBy INT,
    ModifiedDate DATETIME,
    ModifiedBy INT,
    IsActive BIT DEFAULT 1,
    
    CONSTRAINT FK_PurchaseOrder_Requisition FOREIGN KEY (RequisitionMasterId) REFERENCES PurchaseRequisitionMaster(RequisitionMasterId),
    CONSTRAINT FK_PurchaseOrder_Supplier FOREIGN KEY (SupplierId) REFERENCES Suppliers(SupplierId),
    CONSTRAINT FK_PurchaseOrder_Location FOREIGN KEY (DeliveryLocationId) REFERENCES Locations(LocationId),
    CONSTRAINT FK_PurchaseOrder_Status FOREIGN KEY (StatusId) REFERENCES PurchaseRequestStatus(StatusId),
    CONSTRAINT FK_PurchaseOrder_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_PurchaseOrder_ApprovedBy FOREIGN KEY (ApprovedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_PurchaseOrder_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId)
);