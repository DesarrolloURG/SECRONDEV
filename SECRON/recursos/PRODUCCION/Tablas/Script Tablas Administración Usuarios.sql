

------------------------- Tabla de Roles de Usuarios --------------------------------------------------------------------
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

------------------------- Tabla de Roles Permissions --------------------------------------------------------------------

-- TABLA: RolePermissions (Permisos por Rol)
CREATE TABLE RolePermissions (
    RolePermissionId INT IDENTITY(1,1) PRIMARY KEY,
    RoleId INT NOT NULL,
    PermissionId INT NOT NULL,
    IsGranted BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT,
    
    CONSTRAINT FK_RolePermissions_Role FOREIGN KEY (RoleId) REFERENCES Roles(RoleId),
    CONSTRAINT FK_RolePermissions_Permission FOREIGN KEY (PermissionId) REFERENCES Permissions(PermissionId),
    CONSTRAINT FK_RolePermissions_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT UQ_RolePermissions UNIQUE (RoleId, PermissionId)
);

------------------------- Tabla de Estados de Usuarios --------------------------------------------------------------------

-- Tabla de Estados de Usuario
CREATE TABLE UserStatus (
    StatusId INT IDENTITY(1,1) PRIMARY KEY,
    StatusName VARCHAR(25) NOT NULL UNIQUE,  -- 'Activo', 'Inactivo', 'Suspendido', 'Temporal'
    Description VARCHAR(255),
    IsActive BIT DEFAULT 1
);
------------------------- Tabla de Usuarios --------------------------------------------------------------------

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

------------------------- DATOS INICIALES - VALORES PREDETERMINADOS --------------------------------------------------------------------

INSERT INTO Users (Username, PasswordHash, FullName, RoleId, StatusId, IsTemporaryPassword, FailedLoginAttempts, IsLocked)
VALUES ('SA', '$2a$12$/EgBGkJshiVQ2XPyUnRppuJNeDTMHU3K.TeZdq1sqUwvhZeXrJhRa', 'SUPER ADMINISTRATOR', 1, 1, 0, 0, 0); --Password test

INSERT INTO Users (Username, PasswordHash, FullName, RoleId, StatusId, IsTemporaryPassword, FailedLoginAttempts, IsLocked)
VALUES ('ADMIN', '$2a$12$/EgBGkJshiVQ2XPyUnRppuJNeDTMHU3K.TeZdq1sqUwvhZeXrJhRa', 'ADMINISTRADOR', 2, 1, 0, 0, 0); --Password test

INSERT INTO Users (Username, PasswordHash, FullName, RoleId, StatusId, IsTemporaryPassword, FailedLoginAttempts, IsLocked)
VALUES ('SYS_CHECKS_SHARE', '$2a$12$/EgBGkJshiVQ2XPyUnRppuJNeDTMHU3K.TeZdq1sqUwvhZeXrJhRa', 'SYS_CHECKS_SHARE', 1, 1, 0, 0, 0); --Password test

------------------------- Tabla de Permisos por Usuarios  --------------------------------------------------------------------

-- TABLA: UserPermissions (Permisos Específicos por Usuario - Sobrescribir rol)
CREATE TABLE UserPermissions (
    UserPermissionId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    PermissionId INT NOT NULL,
    IsGranted BIT NOT NULL, -- True=Concede, False=Deniega (sobrescribe rol)
    GrantedDate DATETIME DEFAULT GETDATE(),
    GrantedBy INT NOT NULL
    
    CONSTRAINT FK_UserPermissions_User FOREIGN KEY (UserId) REFERENCES Users(UserId),
    CONSTRAINT FK_UserPermissions_Permission FOREIGN KEY (PermissionId) REFERENCES Permissions(PermissionId),
    CONSTRAINT FK_UserPermissions_GrantedBy FOREIGN KEY (GrantedBy) REFERENCES Users(UserId),
    CONSTRAINT UQ_UserPermissions UNIQUE (UserId, PermissionId)
);