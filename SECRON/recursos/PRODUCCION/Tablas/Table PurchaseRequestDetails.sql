-- Tabla de Solicitud de Compras Detalles
CREATE TABLE PurchaseRequestDetails (
    RequestDetailId INT IDENTITY(1,1) PRIMARY KEY,
    RequestMasterId INT NOT NULL,
    ItemId INT NOT NULL,
    SupplierId INT NOT NULL,
    Quantity DECIMAL(18,2) NOT NULL,
    UnitCost DECIMAL(18,2) NOT NULL,
    TotalCost AS (Quantity * UnitCost),
    PriorityId INT NOT NULL,
    StatusId INT NOT NULL,
    RequestReason VARCHAR(300),
    
    CONSTRAINT FK_PurchaseRequestDetails_Master FOREIGN KEY (RequestMasterId) REFERENCES PurchaseRequestMaster(RequestMasterId) ON DELETE CASCADE,
    CONSTRAINT FK_PurchaseRequestDetails_Item FOREIGN KEY (ItemId) REFERENCES Items(ItemId),
    CONSTRAINT FK_PurchaseRequestDetails_Supplier FOREIGN KEY (SupplierId) REFERENCES Suppliers(SupplierId),
    CONSTRAINT FK_PurchaseRequestDetails_Priority FOREIGN KEY (PriorityId) REFERENCES PurchasePriority(PriorityId),
    CONSTRAINT FK_PurchaseRequestDetails_Status FOREIGN KEY (StatusId) REFERENCES PurchaseRequestStatus(StatusId)
);