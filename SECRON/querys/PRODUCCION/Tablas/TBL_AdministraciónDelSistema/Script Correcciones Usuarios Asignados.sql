
------ Paso1. En Tabla Employees Crear la Columna de UserId y vincularla con FK de Usuarios -----
ALTER TABLE Employees ADD UserId INT NULL;
ALTER TABLE Employees ADD CONSTRAINT FK_Employees_User FOREIGN KEY (UserId) REFERENCES Users(UserId);
ALTER TABLE Employees ADD CONSTRAINT UQ_Employees_User UNIQUE (UserId);

------ Paso2. En Tabla Suppliers Crear la Columna de UserId y vincularla con FK de Usuarios -----
ALTER TABLE Suppliers ADD UserId INT NULL;
ALTER TABLE Suppliers ADD CONSTRAINT FK_Suppliers_User FOREIGN KEY (UserId) REFERENCES Users(UserId);
ALTER TABLE Suppliers ADD CONSTRAINT UQ_Suppliers_User UNIQUE (UserId);

------ Paso3. Para mantener las vinculaciones que se tienen de usuarios se pasa a las tablas de empleados -----
UPDATE Employees SET 
UserId = u.UserId 
FROM Users u 
WHERE u.EmployeeId = Employees.EmployeeId;

------ Paso4. Para mantener las vinculaciones que se tienen de usuarios se pasa a las tablas de empleados -----
SELECT DISTINCT o.name AS ObjectName, o.type_desc
FROM sys.sql_modules m
JOIN sys.objects o ON m.object_id = o.object_id
WHERE m.definition LIKE '%Users%EmployeeId%'
   OR m.definition LIKE '%EmployeeId%' AND o.name LIKE '%User%';

SELECT * FROM AcademicProcesses_Revisers