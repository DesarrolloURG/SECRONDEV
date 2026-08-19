-- TABLA MAESTRA: PartidaContableMaster AccountingEntryMaster
----------------------- ESTRUCTURA LEGACY -----------------------
CREATE TABLE AccountingEntryMaster (
    EntryMasterId INT IDENTITY(1,1) PRIMARY KEY,
    CheckId INT NOT NULL,                              -- FK a Checks (siempre asociado)
    EntryDate DATE NOT NULL,
    Concept VARCHAR(300) NOT NULL,
    StatusId INT NOT NULL,                             -- FK a AccountingEntryStatus
    TotalAmount DECIMAL(18,2) DEFAULT 0,               -- Se calcula desde detalles
    
    -- Auditoría
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT NOT NULL,
    ApprovedDate DATETIME,
    ApprovedBy INT,
    ModifiedDate DATETIME,
    ModifiedBy INT,
    IsActive BIT DEFAULT 1,
    
    CONSTRAINT FK_AccountingEntryMaster_Check FOREIGN KEY (CheckId) REFERENCES Checks(CheckId),
    CONSTRAINT FK_AccountingEntryMaster_Status FOREIGN KEY (StatusId) REFERENCES AccountingEntryStatus(StatusId),
    CONSTRAINT FK_AccountingEntryMaster_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_AccountingEntryMaster_ApprovedBy FOREIGN KEY (ApprovedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_AccountingEntryMaster_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId)
);
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
SELECT * FROM AccountingEntryMaster
