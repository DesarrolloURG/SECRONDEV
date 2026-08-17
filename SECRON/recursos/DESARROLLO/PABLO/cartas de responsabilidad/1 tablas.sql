CREATE TABLE ResponsibilityLetterMaster (
    ResponsibilityLetterId INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeId INT NOT NULL,
    FilePath NVARCHAR(400) NOT NULL,
    FileName NVARCHAR(200) NOT NULL,
    UploadDate DATETIME NOT NULL DEFAULT GETDATE(),
    UploadedByUserId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_ResponsibilityLetterMaster_Employees FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId),
    CONSTRAINT FK_ResponsibilityLetterMaster_Users FOREIGN KEY (UploadedByUserId) REFERENCES Users(UserId)
);

CREATE TABLE ResponsibilityLetterDetail (
    ResponsibilityLetterDetailId INT IDENTITY(1,1) PRIMARY KEY,
    ResponsibilityLetterId INT NOT NULL,
    AssetId INT NOT NULL,
    IsCurrent BIT NOT NULL DEFAULT 1,
    IsActive BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_ResponsibilityLetterDetail_Master FOREIGN KEY (ResponsibilityLetterId)
        REFERENCES ResponsibilityLetterMaster(ResponsibilityLetterId),
    CONSTRAINT FK_ResponsibilityLetterDetail_Asset FOREIGN KEY (AssetId) REFERENCES FixedAssets(AssetId)
);

CREATE INDEX IX_ResponsibilityLetterDetail_Asset ON ResponsibilityLetterDetail (AssetId, IsCurrent);

INSERT INTO ParametersConfiguration (ParameterName, ParameterValue, Description)
VALUES (
    'ResponsibilityLettersFolderPath',
    'C:\Users\Pablo Hernandez\Documents\pruebasSECRON\ActivosFijos',
    'Ruta de red donde se almacenan los PDF de cartas de responsabilidad firmadas'
);