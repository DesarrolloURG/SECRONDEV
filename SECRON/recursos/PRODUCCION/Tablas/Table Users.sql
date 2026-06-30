-- TABLA: Users (Usuarios del Sistema)
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    FullName VARCHAR(200) NOT NULL,
    RoleId INT NOT NULL,
    StatusId INT NOT NULL,
    NotificationsEnabled BIT DEFAULT 1,
    LastConnectionDate DATETIME,
    IsTemporaryPassword BIT DEFAULT 0,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT,
    ModifiedDate DATETIME,
    ModifiedBy INT,
    InstitutionalEmail VARCHAR(255),
    EmployeeId INT, -- Relación con empleados (puede ser NULL para usuarios externos)
    PasswordExpiryDate DATETIME,
    FailedLoginAttempts INT DEFAULT 0,
    IsLocked BIT DEFAULT 0,
    LastLoginDate DATETIME,
    
    CONSTRAINT FK_Users_Role FOREIGN KEY (RoleId) REFERENCES Roles(RoleId),
    CONSTRAINT FK_Users_Status FOREIGN KEY (StatusId) REFERENCES UserStatus(StatusId),
    CONSTRAINT FK_Users_Employee FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId),
    CONSTRAINT FK_Users_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Users_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId)
);

INSERT INTO Users (Username, PasswordHash, FullName, RoleId, StatusId, IsTemporaryPassword, FailedLoginAttempts, IsLocked)
VALUES ('SA', '$2a$12$/EgBGkJshiVQ2XPyUnRppuJNeDTMHU3K.TeZdq1sqUwvhZeXrJhRa', 'SUPER ADMINISTRATOR', 1, 1, 0, 0, 0); --Password test

INSERT INTO Users (Username, PasswordHash, FullName, RoleId, StatusId, IsTemporaryPassword, FailedLoginAttempts, IsLocked)
VALUES ('ADMIN', '$2a$12$/EgBGkJshiVQ2XPyUnRppuJNeDTMHU3K.TeZdq1sqUwvhZeXrJhRa', 'ADMINISTRADOR', 2, 1, 0, 0, 0); --Password test

INSERT INTO Users (Username, PasswordHash, FullName, RoleId, StatusId, IsTemporaryPassword, FailedLoginAttempts, IsLocked)
VALUES ('SYS_CHECKS_SHARE', '$2a$12$/EgBGkJshiVQ2XPyUnRppuJNeDTMHU3K.TeZdq1sqUwvhZeXrJhRa', 'SYS_CHECKS_SHARE', 1, 1, 0, 0, 0); --Password test

SELECT * FROM Users