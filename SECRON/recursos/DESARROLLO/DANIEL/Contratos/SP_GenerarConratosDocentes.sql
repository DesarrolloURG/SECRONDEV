-- =========================================================
-- SECRON - SPs adicionales para Frm_RRHH_Teachers_TempGenerarContratos
-- =========================================================

-- ---------------------------------------------------------
-- SP_DocentesTemporal_Cursos_SelectSedes
-- Lista las sedes distintas que existen en DocentesTemporal_Cursos
-- (para llenar el ComboBox_Sede). No depende del catálogo real de
-- Locations -- es texto libre tal como se cargó en el Excel.
-- ---------------------------------------------------------
CREATE OR ALTER PROCEDURE SP_DocentesTemporal_Cursos_SelectSedes
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT AcademicLocation
    FROM DocentesTemporal_Cursos
    ORDER BY AcademicLocation;
END
GO

-- ---------------------------------------------------------
-- SP_DocentesTemporal_SelectPorSede
-- Lista los DOCENTES (una fila por docente, no por curso) que
-- tienen al menos un curso en la sede indicada. El documento que
-- se genera para cada docente incluye TODOS sus cursos, sin
-- importar en qué sede -- este SP solo sirve para UBICARLO en
-- el grid filtrado por sede.
-- ---------------------------------------------------------
CREATE OR ALTER PROCEDURE SP_DocentesTemporal_SelectPorSede
    @AcademicLocation VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT
        d.TeacherTempId, d.ContractCode, d.DPI, d.FirstName, d.LastName,
        d.BirthDate, d.MaritalStatus, d.Gender, d.Address, d.Nationality,
        d.CollegiateNumber, d.NIT, d.Cycle, d.ContractYear, d.IssueDate,
        d.CreatedBy, d.CreatedDate,
        (SELECT COUNT(*) FROM DocentesTemporal_Cursos c2 WHERE c2.TeacherTempId = d.TeacherTempId) AS TotalCursos
    FROM DocentesTemporal d
    INNER JOIN DocentesTemporal_Cursos c ON c.TeacherTempId = d.TeacherTempId
    WHERE c.AcademicLocation = @AcademicLocation
    ORDER BY d.LastName, d.FirstName;
END
GO