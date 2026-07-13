-- =========================================================
-- Catálogo de roles para asignación de personal a sedes
-- =========================================================
CREATE TABLE LocationStaffRoles
(
    RoleTypeId  TINYINT IDENTITY(1,1) PRIMARY KEY,
    RoleName    VARCHAR(50) NOT NULL,
    IsActive    BIT NOT NULL DEFAULT 1,
    CONSTRAINT UX_LocationStaffRoles_RoleName UNIQUE (RoleName)
);
GO

INSERT INTO LocationStaffRoles (RoleName, IsActive)
VALUES
    ('COORDINADOR', 1),
    ('ASISTENTE', 1);
GO

-- =========================================================
-- Asignación de personal (coordinadores/asistentes) a sedes
-- =========================================================
CREATE TABLE LocationStaffAssignments
(
    AssignmentId    INT IDENTITY(1,1) PRIMARY KEY,
    LocationId      INT NOT NULL,
    UserId          INT NOT NULL,
    RoleTypeId      TINYINT NOT NULL,
    IsActive        BIT NOT NULL DEFAULT 1,
    CreatedBy       INT NOT NULL,
    CreatedDate     DATETIME NOT NULL DEFAULT GETDATE(),
    ModifiedBy      INT NULL,
    ModifiedDate    DATETIME NULL,

    CONSTRAINT FK_LocationStaffAssignments_Location
        FOREIGN KEY (LocationId) REFERENCES Locations(LocationId),

    CONSTRAINT FK_LocationStaffAssignments_User
        FOREIGN KEY (UserId) REFERENCES Users(UserId),

    CONSTRAINT FK_LocationStaffAssignments_RoleType
        FOREIGN KEY (RoleTypeId) REFERENCES LocationStaffRoles(RoleTypeId),

    CONSTRAINT FK_LocationStaffAssignments_CreatedBy
        FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),

    CONSTRAINT FK_LocationStaffAssignments_ModifiedBy
        FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId)
);
GO

-- Rol excluyente por sede: un usuario solo puede tener 1 rol activo por sede
CREATE UNIQUE INDEX UX_LocationStaffAssignments_Active
ON LocationStaffAssignments (LocationId, UserId)
WHERE IsActive = 1;
GO

CREATE INDEX IX_LocationStaffAssignments_UserId
ON LocationStaffAssignments (UserId)
WHERE IsActive = 1;
GO