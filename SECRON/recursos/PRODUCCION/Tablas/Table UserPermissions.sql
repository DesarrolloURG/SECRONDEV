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