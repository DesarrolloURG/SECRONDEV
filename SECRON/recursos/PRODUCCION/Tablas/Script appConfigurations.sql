-- Tabla de parámetros
CREATE TABLE ParametersConfiguration (
    ParameterId INT IDENTITY(1,1) PRIMARY KEY,
    ParameterName VARCHAR(100) NOT NULL UNIQUE,
    ParameterValue VARCHAR(200) NOT NULL,
    Description VARCHAR(300) NULL,
    CreateDate DATETIME NOT NULL DEFAULT GETDATE(),
    ModifiedDate DATETIME NULL
);

INSERT INTO ParametersConfiguration (ParameterName, ParameterValue, Description)
VALUES ('DiasVidaContrasena', '30', 'Días de vida útil de la contraseña antes de forzar cambio');

INSERT INTO ParametersConfiguration (ParameterName, ParameterValue, Description)
VALUES ('TiempoSesionActivaMinutos', '15', 'Minutos de inactividad antes de cerrar sesión automáticamente');

update Users set passwordNeverExpires = 1
where username in ('SA','ADMIN','PHERNANDEZ', 'JTORRES')
