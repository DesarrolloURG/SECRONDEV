---- TABLA MAESTRA: PartidaContableMaster AccountingEntryMaster
------------------------- ESTRUCTURA LEGACY -----------------------
--CREATE TABLE AccountingEntryMaster (
--    EntryMasterId INT IDENTITY(1,1) PRIMARY KEY,
--    CheckId INT NOT NULL,                              -- FK a Checks (siempre asociado)
--    EntryDate DATE NOT NULL,
--    Concept VARCHAR(300) NOT NULL,
--    StatusId INT NOT NULL,                             -- FK a AccountingEntryStatus
--    TotalAmount DECIMAL(18,2) DEFAULT 0,               -- Se calcula desde detalles
    
--    -- Auditoría
--    CreatedDate DATETIME DEFAULT GETDATE(),
--    CreatedBy INT NOT NULL,
--    ApprovedDate DATETIME,
--    ApprovedBy INT,
--    ModifiedDate DATETIME,
--    ModifiedBy INT,
--    IsActive BIT DEFAULT 1,
    
--    CONSTRAINT FK_AccountingEntryMaster_Check FOREIGN KEY (CheckId) REFERENCES Checks(CheckId),
--    CONSTRAINT FK_AccountingEntryMaster_Status FOREIGN KEY (StatusId) REFERENCES AccountingEntryStatus(StatusId),
--    CONSTRAINT FK_AccountingEntryMaster_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
--    CONSTRAINT FK_AccountingEntryMaster_ApprovedBy FOREIGN KEY (ApprovedBy) REFERENCES Users(UserId),
--    CONSTRAINT FK_AccountingEntryMaster_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId)
--);


----1. Creación de partida maestra a la que se vincularán los módulo -----------------------
----------------------- ESTRUCTURA NUEVA -----------------------
CREATE TABLE AccountingEntryMaster (
    EntryMasterId INT IDENTITY(1,1) PRIMARY KEY,	-- Id de la partida maestra 
    EntryDate DATE NOT NULL,						-- Fecha de la partida
    Concept VARCHAR(300) NOT NULL,					-- Concepto 
    StatusId INT NOT NULL,							-- FK del estado de la partida
    TotalAmount DECIMAL(18,2) DEFAULT 0,			-- Se calcula desde los detalles
    
	-- Auditoría
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT NOT NULL,
    ApprovedDate DATETIME,
    ApprovedBy INT,
    ModifiedDate DATETIME,
    ModifiedBy INT,
    IsActive BIT DEFAULT 1,

    -- FKs a Status y Users (se quedan igual)
    CONSTRAINT FK_AccountingEntryMaster_Status FOREIGN KEY (StatusId) REFERENCES AccountingEntryStatus(StatusId),
    CONSTRAINT FK_AccountingEntryMaster_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_AccountingEntryMaster_ApprovedBy FOREIGN KEY (ApprovedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_AccountingEntryMaster_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId)
);

----2. Estados de partidas contables -----------------------

-- TABLA CATÁLOGO: de Estados AccountingEntryStatus
CREATE TABLE AccountingEntryStatus (
    StatusId INT IDENTITY(1,1) PRIMARY KEY,
    StatusCode VARCHAR(20) NOT NULL UNIQUE,            -- 'APROBADO', 'ANULADO', 'REVERSION'
    StatusName VARCHAR(50) NOT NULL,
    IsActive BIT DEFAULT 1
);

-- DATOS INICIALES
INSERT INTO AccountingEntryStatus (StatusCode, StatusName) VALUES
('APROBADO', 'APROBADO'),
('ANULADO', 'ANULADO'),
('REVERSION', 'REVERSIÓN');

----3. Creación de la tabla de detalles para la partida contable master -----------------------

-- TABLA DETALLE: PartidaContableDetalles AccountingEntryDetails 
CREATE TABLE AccountingEntryDetails (
    EntryDetailId INT IDENTITY(1,1) PRIMARY KEY,
    EntryMasterId INT NOT NULL,
    AccountId INT NOT NULL,                            -- FK a Accounts
    
    Debit DECIMAL(18,2) DEFAULT 0,                     -- Cargo (debe)
    Credit DECIMAL(18,2) DEFAULT 0,                    -- Abono (haber)
    
    Remarks VARCHAR(300),                              -- Observaciones del detalle
    
    CONSTRAINT FK_AccountingEntryDetails_Master FOREIGN KEY (EntryMasterId) REFERENCES AccountingEntryMaster(EntryMasterId) ON DELETE CASCADE,
    CONSTRAINT FK_AccountingEntryDetails_Account FOREIGN KEY (AccountId) REFERENCES Accounts(AccountId),
    CONSTRAINT CK_DebitOrCredit CHECK (Debit > 0 OR Credit > 0)  -- Al menos uno debe tener valor
);

----4. Creación de partida contable - de encabezado de módulo de cheques -----------------------
----------------------- Partidas de Cheques  -----------------------
CREATE TABLE AccountingEntryChecks (
    EntryMasterId INT NOT NULL PRIMARY KEY,
    CheckId INT NOT NULL,
    CONSTRAINT FK_AEChecks_Master   FOREIGN KEY (EntryMasterId) REFERENCES AccountingEntryMaster(EntryMasterId) ON DELETE CASCADE,
    CONSTRAINT FK_AEChecks_Check    FOREIGN KEY (CheckId)        REFERENCES Checks(CheckId)
);

----5. Creación de partida contable - de encabezado de módulo de transferencias -----------------------
----------------------- Partidas de Transferencias  -----------------------

CREATE TABLE AccountingEntryTransfers (
    EntryMasterId INT NOT NULL PRIMARY KEY,
    TransferId INT NOT NULL,
    CONSTRAINT FK_AETransfers_Master   FOREIGN KEY (EntryMasterId) REFERENCES AccountingEntryMaster(EntryMasterId) ON DELETE CASCADE,
    CONSTRAINT FK_AETransfers_Transfer FOREIGN KEY (TransferId)    REFERENCES Transfers(TransferId)
);