----------------------------- Tabla de Cheques  ---------------------------------------------


-- TABLA: Checks (Cheques)
CREATE TABLE Checks (
    CheckId INT IDENTITY(1,1) PRIMARY KEY,
    CheckNumber VARCHAR(20) NOT NULL UNIQUE,           -- Número de Transferencia
    IssueDate DATE NOT NULL,                           -- Fecha de emisión
    IssuePlace VARCHAR(50) DEFAULT 'Guatemala' NOT NULL, -- Lugar de emisión
    
    -- Montos principales
    Amount DECIMAL(15, 2) NOT NULL,                    -- Monto total del cheque
    PrintedAmount DECIMAL(15, 2) NOT NULL,             -- Monto impreso
    
    -- Beneficiario
    BeneficiaryName VARCHAR(250) NOT NULL,             -- Nombre del beneficiario
    EmployeeId INT NULL,                               -- FK si el beneficiario es empleado
    
    -- Información bancaria
    BankId INT NOT NULL,                               -- FK a Banks
    BankAccountNumber VARCHAR(50) NULL,                -- Número de cuenta (opcional)
    
    -- Estado y concepto
    StatusId INT NOT NULL,                             -- FK a CheckStatus
    Concept VARCHAR(300) NOT NULL,                     -- Concepto resumido
    DetailDescription TEXT NULL,                       -- Detalle completo
    
    -- Período y organización
    Period VARCHAR(50) NOT NULL,                       -- Formato: "Enero 2025" o "Q1-2025"
    LocationId INT NULL,                               -- FK a Locations (Sede)
    DepartmentId INT NULL,                             -- FK a Departments
    
    -- Desgloses financieros
    Exemption DECIMAL(15,2) DEFAULT 0 NOT NULL,        -- Exención
    TaxFreeAmount DECIMAL(15,2) DEFAULT 0 NOT NULL,    -- Monto libre de impuestos
    FoodAllowance DECIMAL(15,2) DEFAULT 0 NOT NULL,    -- Alimentación
    IGSS DECIMAL(15,2) DEFAULT 0 NOT NULL,             -- IGSS
    WithholdingTax DECIMAL(15,2) DEFAULT 0 NOT NULL,   -- ITH
    Retention DECIMAL(15,2) DEFAULT 0 NOT NULL,        -- Retención
    Bonus DECIMAL(15,2) DEFAULT 0 NOT NULL,            -- Bonificación
    Discounts DECIMAL(15,2) DEFAULT 0 NOT NULL,        -- Descuentos
    Advances DECIMAL(15,2) DEFAULT 0 NOT NULL,         -- Anticipos
	Viaticos DECIMAL(15,2) DEFAULT 0 NOT NULL,		   -- Viaticos
    
    -- Referencias adicionales
    PurchaseOrderNumber VARCHAR(25) NULL,              -- No. Orden de Compra
    Complement VARCHAR(25) NULL,                       -- Complemento
    
    -- Control y auditoría
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT NULL,
    ModifiedDate DATETIME NULL,
    ModifiedBy INT NULL,
    AuthorizedDate DATETIME NULL,
    AuthorizedBy INT NULL,
    CashedDate DATETIME NULL,                          -- Fecha de cobro

	--Valores agregados de último
	Stamps DECIMAL(15,2) DEFAULT 0 NOT NULL,	-- TIMBRES
	
	Predeclared BIT DEFAULT 0 NOT NULL, -- Cheques predeclarados
	Compensation DECIMAL(15,2) DEFAULT 0 NOT NULL, -- Indemnización
	Vacation DECIMAL(15,2) DEFAULT 0 NOT NULL, -- Vacaciones
	Bill VARCHAR(50) NULL, -- Factura
	Aguinaldo DECIMAL(18, 2) NOT NULL DEFAULT 0,
	LastComplement BIT NOT NULL DEFAULT 0,
	FileControl VARCHAR(20) NULL DEFAULT 'PENDIENTE',
    
    -- Foreign Keys
    CONSTRAINT FK_Checks_Employee FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId),
    CONSTRAINT FK_Checks_Bank FOREIGN KEY (BankId) REFERENCES Banks(BankId),
    CONSTRAINT FK_Checks_Status FOREIGN KEY (StatusId) REFERENCES CheckStatus(StatusId),
    CONSTRAINT FK_Checks_Location FOREIGN KEY (LocationId) REFERENCES Locations(LocationId),
    CONSTRAINT FK_Checks_Department FOREIGN KEY (DepartmentId) REFERENCES Departments(DepartmentId),
    CONSTRAINT FK_Checks_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Checks_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Checks_AuthorizedBy FOREIGN KEY (AuthorizedBy) REFERENCES Users(UserId)
);

----------------------------- Tabla control de cheques ---------------------------------------------
-- Tabla Control de Cheques
CREATE TABLE CheckControl (
    CheckControlId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,                               -- FK a Users
    InitialLimit INT NOT NULL,
    FinalLimit INT NOT NULL,
    CurrentCounter INT NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT,
    ModifiedDate DATETIME,
    ModifiedBy INT,
	Priority BIT DEFAULT 0 NOT NULL,
    
    CONSTRAINT FK_CheckControl_User FOREIGN KEY (UserId) REFERENCES Users(UserId),
    CONSTRAINT FK_CheckControl_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_CheckControl_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId)
);

----------------------------- Tabla de estados de cheques ---------------------------------------------
CREATE TABLE CheckStatus (
    StatusId INT IDENTITY(1,1) PRIMARY KEY,
    StatusName VARCHAR(25) NOT NULL UNIQUE,            -- 'EMITIDO', 'CIRCULACION', 'COBRADO', 'PAGADO', 'ANULADO'
    Description VARCHAR(255),
    IsActive BIT DEFAULT 1
);

SELECT * FROM CheckStatus

INSERT INTO CheckStatus(StatusName, Description, IsActive) VALUES

('EMITIDO','CHEQUE GENERADO EN SECRON, AÚN NO EN CIRCULACIÓN',1),
('CIRCULACIÓN','CHEQUE QUE YA SALIÓ DE OFICINAS CENTRALES, ESTA CIRCULANDO',1),
('COBRADO','CHEQUE QUE FUE COBRADO POR EL BENEFICIARIO',1),
('PAGADO','CHEQUE DEPOSITADO AL BENEFICIARIO',1),
('ANULADO','CHEQUE ANULADO POR INFORMACIÓN ERRONEA O PAGO CANCELADO',1);