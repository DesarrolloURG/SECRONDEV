-- TABLA: Roles (Roles del Sistema)
CREATE TABLE Roles (
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    RoleName VARCHAR(50) NOT NULL UNIQUE,
    Description VARCHAR(255),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT NULL,
    
    CONSTRAINT FK_Roles_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId)
);

-- Si se desea botar la de usuarios, primero se crean los roles o el SA. para poder realizar la llave foranea
-- ALTER TABLE Roles
-- ADD CONSTRAINT FK_Roles_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId);

SELECT * from Roles