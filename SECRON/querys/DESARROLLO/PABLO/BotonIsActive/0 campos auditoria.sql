ALTER TABLE ItemCategories ADD ModifiedDate DATETIME NULL;
ALTER TABLE ItemCategories ADD ModifiedBy INT NULL;

ALTER TABLE MeasurementUnits ADD CreatedBy INT NULL;
ALTER TABLE MeasurementUnits ADD CreatedDate DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE MeasurementUnits ADD ModifiedBy INT NULL;
ALTER TABLE MeasurementUnits ADD ModifiedDate DATETIME NULL;