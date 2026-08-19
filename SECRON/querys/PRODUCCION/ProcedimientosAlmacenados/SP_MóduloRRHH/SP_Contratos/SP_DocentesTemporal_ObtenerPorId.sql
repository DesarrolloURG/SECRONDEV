CREATE OR ALTER PROCEDURE SP_DocentesTemporal_ObtenerPorId
    @TeacherTempId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TeacherTempId, ContractCode, DPI, FirstName, LastName, BirthDate,
           MaritalStatus, Gender, Address, Nationality, CollegiateNumber, NIT,
           Cycle, ContractYear, IssueDate
    FROM DocentesTemporal
    WHERE TeacherTempId = @TeacherTempId;
END
GO

GRANT EXECUTE ON dbo.SP_DocentesTemporal_ObtenerPorId TO admin_api;
GO