CREATE TABLE AccountingEntryChecks (
    EntryMasterId INT NOT NULL PRIMARY KEY,
    CheckId INT NOT NULL,
    CONSTRAINT FK_AEChecks_Master   FOREIGN KEY (EntryMasterId) REFERENCES AccountingEntryMaster(EntryMasterId) ON DELETE CASCADE,
    CONSTRAINT FK_AEChecks_Check    FOREIGN KEY (CheckId)        REFERENCES Checks(CheckId)
);

SELECT * FROM AccountingEntryChecks