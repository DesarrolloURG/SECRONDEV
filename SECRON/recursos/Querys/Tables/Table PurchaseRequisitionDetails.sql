-- Tabla de Requisicion de Compras Detalles
CREATE TABLE PurchaseRequisitionDetails (
    RequisitionDetailId INT IDENTITY(1,1) PRIMARY KEY,
    RequisitionMasterId INT NOT NULL,
    RequestDetailId INT,
    ItemId INT NOT NULL,
    SupplierId INT NOT NULL,
    Quantity DECIMAL(18,2) NOT NULL,
    UnitCost DECIMAL(18,2) NOT NULL,
    TotalCost AS (Quantity * UnitCost),
    PriorityId INT NOT NULL,
    StatusId INT NOT NULL,
    RequestReason VARCHAR(300),
    
    CONSTRAINT FK_PurchaseRequisitionDetails_Master FOREIGN KEY (RequisitionMasterId) REFERENCES PurchaseRequisitionMaster(RequisitionMasterId) ON DELETE CASCADE,
    CONSTRAINT FK_PurchaseRequisitionDetails_RequestDetail FOREIGN KEY (RequestDetailId) REFERENCES PurchaseRequestDetails(RequestDetailId),
    CONSTRAINT FK_PurchaseRequisitionDetails_Item FOREIGN KEY (ItemId) REFERENCES Items(ItemId),
    CONSTRAINT FK_PurchaseRequisitionDetails_Supplier FOREIGN KEY (SupplierId) REFERENCES Suppliers(SupplierId),
    CONSTRAINT FK_PurchaseRequisitionDetails_Priority FOREIGN KEY (PriorityId) REFERENCES PurchasePriority(PriorityId),
    CONSTRAINT FK_PurchaseRequisitionDetails_Status FOREIGN KEY (StatusId) REFERENCES PurchaseRequestStatus(StatusId)
);