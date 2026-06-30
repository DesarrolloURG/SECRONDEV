CREATE TABLE AccountingEntryTransfers (
    EntryMasterId INT NOT NULL PRIMARY KEY,
    TransferId INT NOT NULL,
    CONSTRAINT FK_AETransfers_Master   FOREIGN KEY (EntryMasterId) REFERENCES AccountingEntryMaster(EntryMasterId) ON DELETE CASCADE,
    CONSTRAINT FK_AETransfers_Transfer FOREIGN KEY (TransferId)    REFERENCES Transfers(TransferId)
);

SELECT * FROM AccountingEntryTransfers