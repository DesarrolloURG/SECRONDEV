-- =====================================================
-- ÍNDICES PARA OPTIMIZACIÓN DE BÚSQUEDAS
-- =====================================================

-- Índices en códigos (búsquedas frecuentes por código público)
CREATE INDEX IX_Coordinators_Code ON Coordinators(CoordinatorCode) WHERE IsActive = 1;
CREATE INDEX IX_Teachers_Code ON Teachers(TeacherCode) WHERE IsActive = 1;
CREATE INDEX IX_Courses_Code ON Courses(CourseCode) WHERE IsActive = 1;
CREATE INDEX IX_Careers_Code ON Careers(CareerCode) WHERE IsActive = 1;
CREATE INDEX IX_Sections_Code ON Sections(SectionCode) WHERE IsActive = 1;
CREATE INDEX IX_Schedules_Code ON Schedules(ScheduleCode) WHERE IsActive = 1;
CREATE INDEX IX_ScheduleTypes_Code ON ScheduleTypes(ScheduleTypeCode) WHERE IsActive = 1;

-- Índices en relaciones frecuentes
CREATE INDEX IX_Coordinators_Location ON Coordinators(LocationId) WHERE IsActive = 1;
CREATE INDEX IX_Teachers_Location ON Teachers(LocationId) WHERE IsActive = 1;
CREATE INDEX IX_Teachers_Coordinator ON Teachers(RegisteredByCoordinatorId) WHERE IsActive = 1;
CREATE INDEX IX_Sections_Location ON Sections(LocationId) WHERE IsActive = 1;
CREATE INDEX IX_Sections_Career ON Sections(CareerId) WHERE IsActive = 1;
CREATE INDEX IX_Sections_Coordinator ON Sections(CoordinatorId) WHERE IsActive = 1;
CREATE INDEX IX_Schedules_Section ON Schedules(SectionId) WHERE IsActive = 1;

-- Índices en tablas detalle
CREATE INDEX IX_CareerCourses_Career ON CareerCourses(CareerId) WHERE IsActive = 1;
CREATE INDEX IX_CareerCourses_Course ON CareerCourses(CourseId) WHERE IsActive = 1;
CREATE INDEX IX_SectionCourses_Section ON SectionCourses(SectionId) WHERE IsActive = 1;
CREATE INDEX IX_TeacherCourses_Teacher ON TeacherCourses(TeacherId) WHERE IsActive = 1;
CREATE INDEX IX_ScheduleDetails_Schedule ON ScheduleDetails(ScheduleId) WHERE IsActive = 1;
CREATE INDEX IX_ScheduleDetails_Teacher ON ScheduleDetails(TeacherId) WHERE IsActive = 1;

