----------------- Tabla de Marcas ------------------------
CREATE TABLE Brands (
    BrandId INT IDENTITY(1,1) PRIMARY KEY,
    BrandName VARCHAR(100) NOT NULL UNIQUE,
    Category VARCHAR(50),
    IsActive BIT DEFAULT 1
);
-- Insertar marcas sin duplicados (TODO EN MAYÚSCULAS)
INSERT INTO Brands (BrandName, Category) VALUES
-- COMPUTADORAS Y TECNOLOGÍA
('HP', 'COMPUTADORAS'),
('DELL', 'COMPUTADORAS'),
('APPLE', 'COMPUTADORAS'),
('LENOVO', 'COMPUTADORAS'),
('SAMSUNG', 'COMPUTADORAS'),
('SONY', 'COMPUTADORAS'),
('ACER', 'COMPUTADORAS'),
('ASUS', 'COMPUTADORAS'),
('MICROSOFT', 'COMPUTADORAS'),
('LG', 'COMPUTADORAS'),
('RAZER', 'COMPUTADORAS'),
('LOGITECH', 'COMPUTADORAS'),
('HUAWEI', 'COMPUTADORAS'),
('TOSHIBA', 'COMPUTADORAS'),

-- IMPRESORAS
('CANON', 'IMPRESORAS'),
('EPSON', 'IMPRESORAS'),
('BROTHER', 'IMPRESORAS'),
('XEROX', 'IMPRESORAS'),
('RICOH', 'IMPRESORAS'),
('KYOCERA', 'IMPRESORAS'),
('LEXMARK', 'IMPRESORAS'),
('OKI', 'IMPRESORAS'),

-- MÓVILES
('NOKIA', 'MÓVILES'),
('XIAOMI', 'MÓVILES'),
('MOTOROLA', 'MÓVILES'),
('ONEPLUS', 'MÓVILES'),
('GOOGLE', 'MÓVILES'),

-- TELEFONÍA
('AVAYA', 'TELEFONÍA'),
('CISCO', 'TELEFONÍA'),
('ALCATEL-LUCENT', 'TELEFONÍA'),
('PANASONIC', 'TELEFONÍA'),
('NEC', 'TELEFONÍA'),
('SIEMENS', 'TELEFONÍA'),
('MITEL', 'TELEFONÍA'),
('3CX', 'TELEFONÍA'),
('ASTERISK', 'TELEFONÍA'),
('GRANDSTREAM', 'TELEFONÍA'),

-- OTROS
('VIZIO', 'OTROS'),
('BENQ', 'OTROS'),
('VIEWSONIC', 'OTROS'),
('JVC', 'OTROS'),
('SHARP', 'OTROS'),
('PHILIPS', 'OTROS'),
('OTRAS', 'OTROS');

----------------- Tabla de Solicitud de Compras Maestra ------------------------

-- CATÁLOGOS ESTADOS DE SOLICITUDES DE COMPRAS
CREATE TABLE PurchaseRequestStatus (
    StatusId INT IDENTITY(1,1) PRIMARY KEY,
    StatusCode VARCHAR(20) NOT NULL UNIQUE,
    StatusName VARCHAR(50) NOT NULL,
    IsActive BIT DEFAULT 1
);
-- 'EN_REVISION', 'REVISADA', 'APROBADA', 'RECHAZADA', 'CORREGIR'

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


----------------- Tabla de Requisicón Maestra ------------------------

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

----------------- Tabla de Requisicón Detalles ------------------------

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

----------------- Tabla de Prioridad de Ordenes de Compra ------------------------

-- PRIORIDAD DE LA SOLICITUD DE COMPRA
CREATE TABLE PurchasePriority (
    PriorityId INT IDENTITY(1,1) PRIMARY KEY,
    PriorityCode VARCHAR(10) NOT NULL UNIQUE,
    PriorityName VARCHAR(20) NOT NULL,
    IsActive BIT DEFAULT 1
);
-- 'BAJA', 'MEDIA', 'ALTA', 'URGENTE'

----------------- Tabla de Orden de Compra Maestra ------------------------

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

----------------- Tabla de Orden de Compra Detalles ------------------------
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

