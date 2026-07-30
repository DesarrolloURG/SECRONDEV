-- 1. Login a nivel de servidor
CREATE LOGIN admin_api WITH PASSWORD = 'Dante2026*', CHECK_POLICY = OFF;
GO

-- 2. Usuario dentro de la base de datos SECRONDEV
USE SECRONDEV;
GO
CREATE USER admin_api FOR LOGIN admin_api;
GO

-- 3. Rol dedicado para la API (así, cuando agreguemos SPs nuevos más adelante,
--    solo hay que dar GRANT EXECUTE al rol, no tocar el usuario cada vez)
CREATE ROLE db_api_contratos;
GO
ALTER ROLE db_api_contratos ADD MEMBER admin_api;
GO

-- 4. Permisos SOLO de ejecución sobre los SPs que la API ya necesita hoy
GRANT EXECUTE ON dbo.SP_Portal_Contratos_Vigencia_ObtenerVigente TO db_api_contratos;
GRANT EXECUTE ON dbo.SP_DocentesTemporal_ObtenerPorDPI TO db_api_contratos;
GRANT EXECUTE ON dbo.SP_DocentesTemporal_Cursos_SelectByTeacherTempId TO db_api_contratos;
GRANT EXECUTE ON dbo.SP_Teachers_UpdateFilePath TO db_api_contratos;
GO

-- 5. Permisos sobre los SP al usuario admin_api
GRANT EXECUTE ON dbo.SP_Teachers_UpdateFilePath TO db_api_contratos;
GO