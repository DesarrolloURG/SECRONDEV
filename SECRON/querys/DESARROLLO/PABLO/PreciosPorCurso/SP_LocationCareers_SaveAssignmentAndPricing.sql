CREATE OR ALTER PROCEDURE SP_LocationCareers_SaveAssignmentAndPricing
    @LocationId INT,
    @CareerId INT,
    @ModalityId INT,
    @PricesJson NVARCHAR(MAX),
    @UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UserId, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;
    DECLARE @Now DATETIME = GETDATE();
    DECLARE @Procesados INT = 0;

    BEGIN TRANSACTION
    BEGIN TRY

        -- 1) Asignación Sede+Carrera+Modalidad: crear solo si no existe activa
        IF NOT EXISTS (
            SELECT 1 FROM LocationCareers
            WHERE LocationId = @LocationId AND CareerId = @CareerId AND ModalityId = @ModalityId AND IsActive = 1
        )
        BEGIN
            INSERT INTO LocationCareers (LocationId, CareerId, ModalityId, CreatedBy)
            VALUES (@LocationId, @CareerId, @ModalityId, @UserId);
        END

        -- 2) Precios de cada curso
        DECLARE @CareerCourseId INT, @Price DECIMAL(10,2);

        DECLARE price_cursor CURSOR LOCAL FAST_FORWARD FOR
            SELECT CareerCourseId, Price
            FROM OPENJSON(@PricesJson)
            WITH (CareerCourseId INT '$.careerCourseId', Price DECIMAL(10,2) '$.price');

        OPEN price_cursor;
        FETCH NEXT FROM price_cursor INTO @CareerCourseId, @Price;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            DECLARE @CourseLocationPricingId INT = NULL;
            DECLARE @PrecioVigente DECIMAL(10,2) = NULL;

            SELECT @CourseLocationPricingId = CourseLocationPricingId
            FROM CourseLocationPricingMaster
            WHERE CareerCourseId = @CareerCourseId
              AND LocationId = @LocationId
              AND ModalityId = @ModalityId
              AND IsActive = 1;

            IF @CourseLocationPricingId IS NULL
            BEGIN
                -- No existe combinación todavía: crear Master + primer precio
                INSERT INTO CourseLocationPricingMaster (CareerCourseId, LocationId, ModalityId, CreatedBy)
                VALUES (@CareerCourseId, @LocationId, @ModalityId, @UserId);

                SET @CourseLocationPricingId = SCOPE_IDENTITY();

                INSERT INTO CourseLocationPricingDetail (CourseLocationPricingId, Price, EffectiveFrom, EffectiveTo, CreatedBy)
                VALUES (@CourseLocationPricingId, @Price, @Now, NULL, @UserId);

                SET @Procesados = @Procesados + 1;
            END
            ELSE
            BEGIN
                -- Ya existe: solo se toca el historial si el precio realmente cambió
                SELECT @PrecioVigente = Price
                FROM CourseLocationPricingDetail
                WHERE CourseLocationPricingId = @CourseLocationPricingId
                  AND EffectiveTo IS NULL
                  AND IsActive = 1;

                IF @PrecioVigente IS NULL OR @PrecioVigente <> @Price
                BEGIN
                    UPDATE CourseLocationPricingDetail
                    SET EffectiveTo = @Now, ModifiedDate = @Now, ModifiedBy = @UserId
                    WHERE CourseLocationPricingId = @CourseLocationPricingId
                      AND EffectiveTo IS NULL
                      AND IsActive = 1;

                    INSERT INTO CourseLocationPricingDetail (CourseLocationPricingId, Price, EffectiveFrom, EffectiveTo, CreatedBy)
                    VALUES (@CourseLocationPricingId, @Price, @Now, NULL, @UserId);

                    SET @Procesados = @Procesados + 1;
                END
            END

            FETCH NEXT FROM price_cursor INTO @CareerCourseId, @Price;
        END
        CLOSE price_cursor; DEALLOCATE price_cursor;

        COMMIT TRANSACTION;
        SELECT @Procesados;
    END TRY
    BEGIN CATCH
        IF CURSOR_STATUS('local', 'price_cursor') >= 0
        BEGIN
            CLOSE price_cursor; DEALLOCATE price_cursor;
        END
        ROLLBACK TRANSACTION;
        SELECT 0;
    END CATCH
END
GO