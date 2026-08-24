-- =====================================================================
-- SP: SP_CareerPensums_SaveTree
-- Crea o actualiza un pensum completo (cabecera + cursos + prerequisitos)
-- y genera los precios estándar por sede/modalidad SOLO para cursos NUEVOS.
--
-- Reglas de negocio aplicadas:
--   - StandardPrice de un curso YA EXISTENTE se puede editar en CareerCourses,
--     pero NUNCA se propaga a CourseLocationPricingMaster/Detail ya creados.
--   - Un curso NUEVO agregado al pensum NO genera ningún registro en LocationCareers
--     ni en CourseLocationPricingMaster/Detail. La asignación de Sede+Modalidad a una
--     carrera y la creación de precios se hace exclusivamente desde
--     Frm_AcademicProcesses_CoursesPricing.
--   - Un curso quitado del pensum (no viene en el JSON) se INACTIVA en cascada:
--     CareerCourses.IsActive = 0, y sus CourseLocationPricingMaster también
--     se inactivan (cerrando el Detail vigente), SI es que ya existían.
--   - Prerequisitos: solo pueden referenciar cursos del MISMO JSON en un
--     semestre estrictamente menor. Se valida antes de cualquier escritura.
--
-- Formato esperado de @PensumJson:
-- {
--   "careerPensumId": null,           -- null = crear pensum nuevo
--   "careerId": 5,
--   "pensumCode": "2024-A",
--   "pensumName": "PENSUM 2024",
--   "isCurrent": true,
--   "courses": [
--     {
--       "careerCourseId": null,       -- null = curso nuevo, valor = curso existente
--       "courseId": 12,
--       "semester": 1,
--       "standardPrice": 500.00,
--       "prerequisiteCourseIds": []   -- courseId de cursos del MISMO JSON
--     }
--   ]
-- }
--
-- Códigos de retorno:
--   > 0   = CareerPensumId (éxito)
--   -1    = PensumCode duplicado para esa carrera
--   -2    = Prerequisito inválido (no existe en el JSON o no es de semestre anterior)
--   0     = Error inesperado
-- =====================================================================
CREATE OR ALTER PROCEDURE SP_CareerPensums_SaveTree
    @PensumJson NVARCHAR(MAX),
    @UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UserId, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    DECLARE @CareerPensumId INT       = TRY_CAST(JSON_VALUE(@PensumJson, '$.careerPensumId') AS INT);
    DECLARE @CareerId INT             = JSON_VALUE(@PensumJson, '$.careerId');
    DECLARE @PensumCode VARCHAR(50)   = JSON_VALUE(@PensumJson, '$.pensumCode');
    DECLARE @PensumName VARCHAR(150)  = JSON_VALUE(@PensumJson, '$.pensumName');
    DECLARE @IsCurrent BIT            = TRY_CAST(JSON_VALUE(@PensumJson, '$.isCurrent') AS BIT);
    DECLARE @Now DATETIME             = GETDATE();

    -- Cursos entrantes, con su JSON crudo para poder leer prerequisiteCourseIds después
    IF OBJECT_ID('tempdb..#CoursesIncoming') IS NOT NULL DROP TABLE #CoursesIncoming;
    SELECT
        CAST(JSON_VALUE(c.value, '$.courseId') AS INT)        AS CourseId,
        TRY_CAST(JSON_VALUE(c.value, '$.careerCourseId') AS INT) AS CareerCourseId,
        CAST(JSON_VALUE(c.value, '$.semester') AS INT)        AS Semester,
        CAST(JSON_VALUE(c.value, '$.standardPrice') AS DECIMAL(10,2)) AS StandardPrice,
        c.value                                                AS CourseJson
    INTO #CoursesIncoming
    FROM OPENJSON(@PensumJson, '$.courses') c;

    -- Mapa CourseId -> CareerCourseId (se llena conforme se insertan/actualizan)
    IF OBJECT_ID('tempdb..#CourseIdMap') IS NOT NULL DROP TABLE #CourseIdMap;
    CREATE TABLE #CourseIdMap (CourseId INT PRIMARY KEY, CareerCourseId INT NOT NULL);

    BEGIN TRANSACTION
    BEGIN TRY

        -- =================================================================
        -- VALIDACIÓN: cada prerequisito debe existir en el mismo JSON
        -- y pertenecer a un semestre estrictamente menor
        -- =================================================================
        IF EXISTS (
            SELECT 1
            FROM #CoursesIncoming ci
            CROSS APPLY OPENJSON(ci.CourseJson, '$.prerequisiteCourseIds') pr
            LEFT JOIN #CoursesIncoming prereq ON prereq.CourseId = CAST(pr.value AS INT)
            WHERE prereq.CourseId IS NULL
               OR prereq.Semester >= ci.Semester
        )
        BEGIN
            ROLLBACK TRANSACTION; SELECT -2; RETURN;
        END

        -- =================================================================
        -- CABECERA: CareerPensums (insert o update)
        -- =================================================================
        IF @CareerPensumId IS NULL
        BEGIN
            IF EXISTS (SELECT 1 FROM CareerPensums WHERE CareerId = @CareerId AND PensumCode = UPPER(@PensumCode))
            BEGIN
                ROLLBACK TRANSACTION; SELECT -1; RETURN;
            END

            IF @IsCurrent = 1
                UPDATE CareerPensums SET IsCurrent = 0, ModifiedDate = @Now, ModifiedBy = @UserId
                WHERE CareerId = @CareerId AND IsCurrent = 1;

            INSERT INTO CareerPensums (CareerId, PensumCode, PensumName, IsCurrent, CreatedBy)
            VALUES (@CareerId, UPPER(@PensumCode), UPPER(@PensumName), ISNULL(@IsCurrent, 0), @UserId);

            SET @CareerPensumId = SCOPE_IDENTITY();
        END
        ELSE
        BEGIN
            IF EXISTS (
                SELECT 1 FROM CareerPensums
                WHERE CareerId = @CareerId AND PensumCode = UPPER(@PensumCode) AND CareerPensumId <> @CareerPensumId
            )
            BEGIN
                ROLLBACK TRANSACTION; SELECT -1; RETURN;
            END

            IF @IsCurrent = 1
                UPDATE CareerPensums SET IsCurrent = 0, ModifiedDate = @Now, ModifiedBy = @UserId
                WHERE CareerId = @CareerId AND IsCurrent = 1 AND CareerPensumId <> @CareerPensumId;

            UPDATE CareerPensums
            SET PensumCode = UPPER(@PensumCode),
                PensumName = UPPER(@PensumName),
                IsCurrent = ISNULL(@IsCurrent, 0),
                ModifiedDate = @Now,
                ModifiedBy = @UserId
            WHERE CareerPensumId = @CareerPensumId;
        END

        -- =================================================================
        -- CURSOS: insertar nuevos / actualizar existentes
        -- =================================================================
        DECLARE @CourseId INT, @CareerCourseId INT, @Semester INT, @StandardPrice DECIMAL(10,2);

        DECLARE course_cursor CURSOR LOCAL FAST_FORWARD FOR
            SELECT CourseId, CareerCourseId, Semester, StandardPrice FROM #CoursesIncoming;
        OPEN course_cursor;
        FETCH NEXT FROM course_cursor INTO @CourseId, @CareerCourseId, @Semester, @StandardPrice;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            IF @CareerCourseId IS NULL
            BEGIN
                -- Curso nuevo dentro del pensum. NO genera precios ni toca LocationCareers:
                -- la asignación de Sede+Modalidad y la creación de precios se hace exclusivamente
                -- desde Frm_AcademicProcesses_CoursesPricing (Btn_Save).
                INSERT INTO CareerCourses (CareerPensumId, CourseId, Semester, IsRequired, StandardPrice, CreatedBy)
                VALUES (@CareerPensumId, @CourseId, @Semester, 1, @StandardPrice, @UserId);

                SET @CareerCourseId = SCOPE_IDENTITY();
            END
            ELSE
            BEGIN
                -- Curso existente: solo actualiza CareerCourses, NUNCA toca precios ya generados
                UPDATE CareerCourses
                SET Semester = @Semester,
                    StandardPrice = @StandardPrice,
                    IsActive = 1,
                    ModifiedDate = @Now,
                    ModifiedBy = @UserId
                WHERE CareerCourseId = @CareerCourseId AND CareerPensumId = @CareerPensumId;
            END

            INSERT INTO #CourseIdMap (CourseId, CareerCourseId) VALUES (@CourseId, @CareerCourseId);

            FETCH NEXT FROM course_cursor INTO @CourseId, @CareerCourseId, @Semester, @StandardPrice;
        END
        CLOSE course_cursor; DEALLOCATE course_cursor;

        -- =================================================================
        -- CURSOS QUITADOS DEL PENSUM: inactivar en cascada (nunca eliminar)
        -- =================================================================
        UPDATE CareerCourses
        SET IsActive = 0, ModifiedDate = @Now, ModifiedBy = @UserId
        WHERE CareerPensumId = @CareerPensumId
          AND IsActive = 1
          AND CareerCourseId NOT IN (SELECT CareerCourseId FROM #CourseIdMap);

        UPDATE m
        SET m.IsActive = 0, m.ModifiedDate = @Now, m.ModifiedBy = @UserId
        FROM CourseLocationPricingMaster m
        INNER JOIN CareerCourses cc ON cc.CareerCourseId = m.CareerCourseId
        WHERE cc.CareerPensumId = @CareerPensumId
          AND cc.IsActive = 0
          AND m.IsActive = 1;

        UPDATE d
        SET d.EffectiveTo = @Now, d.ModifiedDate = @Now, d.ModifiedBy = @UserId
        FROM CourseLocationPricingDetail d
        INNER JOIN CourseLocationPricingMaster m ON m.CourseLocationPricingId = d.CourseLocationPricingId
        INNER JOIN CareerCourses cc ON cc.CareerCourseId = m.CareerCourseId
        WHERE cc.CareerPensumId = @CareerPensumId
          AND cc.IsActive = 0
          AND d.EffectiveTo IS NULL
          AND d.IsActive = 1;

        -- =================================================================
        -- PREREQUISITOS: diff real, solo se toca lo que cambió
        -- =================================================================
        IF OBJECT_ID('tempdb..#PrereqIncoming') IS NOT NULL DROP TABLE #PrereqIncoming;
        SELECT DISTINCT map.CareerCourseId, prereqMap.CareerCourseId AS PrerequisiteCareerCourseId
        INTO #PrereqIncoming
        FROM #CoursesIncoming ci
        CROSS APPLY OPENJSON(ci.CourseJson, '$.prerequisiteCourseIds') pr
        INNER JOIN #CourseIdMap map ON map.CourseId = ci.CourseId
        INNER JOIN #CourseIdMap prereqMap ON prereqMap.CourseId = CAST(pr.value AS INT);

        -- 1) Quitados: estaban activos pero ya no vienen en el JSON -> inactivar
        UPDATE existing
        SET existing.IsActive = 0, existing.ModifiedDate = @Now, existing.ModifiedBy = @UserId
        FROM CareerCoursePrerequisites existing
        WHERE existing.CareerCourseId IN (SELECT CareerCourseId FROM #CourseIdMap)
          AND existing.IsActive = 1
          AND NOT EXISTS (
              SELECT 1 FROM #PrereqIncoming pi
              WHERE pi.CareerCourseId = existing.CareerCourseId
                AND pi.PrerequisiteCareerCourseId = existing.PrerequisiteCareerCourseId
          );

        -- 2) Reactivados: existían inactivos y vuelven a aparecer en el JSON
        UPDATE existing
        SET existing.IsActive = 1, existing.ModifiedDate = @Now, existing.ModifiedBy = @UserId
        FROM CareerCoursePrerequisites existing
        INNER JOIN #PrereqIncoming pi
            ON pi.CareerCourseId = existing.CareerCourseId
           AND pi.PrerequisiteCareerCourseId = existing.PrerequisiteCareerCourseId
        WHERE existing.IsActive = 0;

        -- 3) Nuevos: no existen ni activos ni inactivos todavía
        INSERT INTO CareerCoursePrerequisites (CareerCourseId, PrerequisiteCareerCourseId, CreatedBy)
        SELECT pi.CareerCourseId, pi.PrerequisiteCareerCourseId, @UserId
        FROM #PrereqIncoming pi
        WHERE NOT EXISTS (
            SELECT 1 FROM CareerCoursePrerequisites existing
            WHERE existing.CareerCourseId = pi.CareerCourseId
              AND existing.PrerequisiteCareerCourseId = pi.PrerequisiteCareerCourseId
        );

        COMMIT TRANSACTION;
        SELECT @CareerPensumId;
    END TRY
    BEGIN CATCH
        IF CURSOR_STATUS('local', 'course_cursor') >= 0
        BEGIN
            CLOSE course_cursor; DEALLOCATE course_cursor;
        END
        ROLLBACK TRANSACTION;
        SELECT 0;
    END CATCH
END
GO