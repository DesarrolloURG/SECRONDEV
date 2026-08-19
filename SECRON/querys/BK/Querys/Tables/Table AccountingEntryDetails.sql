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

SELECT * FROM AccountingEntryDetails ORDER BY EntryMasterId
