-- Tabla de Orden de Compra Detalles
CREATE TABLE PurchaseOrderDetails (
    PurchaseOrderDetailId INT IDENTITY(1,1) PRIMARY KEY,
    PurchaseOrderId INT NOT NULL,
    RequisitionDetailId INT,
    ItemId INT NOT NULL,
    Quantity DECIMAL(18,2) NOT NULL,
    UnitCost DECIMAL(18,2) NOT NULL,
    TotalCost AS (Quantity * UnitCost),
    
    CONSTRAINT FK_PurchaseOrderDetails_Master FOREIGN KEY (PurchaseOrderId) REFERENCES PurchaseOrderMaster(PurchaseOrderId) ON DELETE CASCADE,
    CONSTRAINT FK_PurchaseOrderDetails_RequisitionDetail FOREIGN KEY (RequisitionDetailId) REFERENCES PurchaseRequisitionDetails(RequisitionDetailId),
    CONSTRAINT FK_PurchaseOrderDetails_Item FOREIGN KEY (ItemId) REFERENCES Items(ItemId)
);