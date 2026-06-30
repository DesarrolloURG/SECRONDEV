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

SELECT * FROM CheckControl