CREATE OR ALTER PROCEDURE sp_ObtenerReservaPorID
    @ID_Reserva INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        -- IDs
        r.ID_Reserva AS Id,
        r.ID_Usuario AS UserId,
        r.ID_Fecha AS FechaId,
        f.ID_Experiencia AS ExperienceId,

        -- Datos enriquecidos
        u.Nombre AS UserName,
        e.Titulo AS ExperienceName,

        -- Fechas
        CAST(r.Fecha_Reserva AS datetime2) AS DateFrom,
        CAST(r.Fecha_Reserva AS datetime2) AS DateTo,

        -- Estado
        CASE 
            WHEN r.Estado = 1 THEN 'Confirmed' 
            ELSE 'Pending' 
        END AS Status,

        GETUTCDATE() AS CreatedAt

    FROM Reserva r

    INNER JOIN Usuario u 
        ON r.ID_Usuario = u.ID_Usuario

    INNER JOIN Fecha_Disponibilidad f 
        ON r.ID_Fecha = f.ID_Fecha

    INNER JOIN Experiencia e 
        ON f.ID_Experiencia = e.ID_Experiencia

    WHERE r.ID_Reserva = @ID_Reserva;
END;
GO

CREATE OR ALTER PROCEDURE sp_CrearReserva
    @ID_Usuario INT,
    @ID_Fecha INT,
    @Cantidad_Personas INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Reserva (
        Fecha_Reserva,
        Cantidad_Personas,
        Estado,
        ID_Usuario,
        ID_Fecha
    )
    VALUES (
        GETDATE(),         -- fecha automática
        @Cantidad_Personas,
        1,                 -- 1 = Confirmed
        @ID_Usuario,
        @ID_Fecha
    );

    -- opcional: devolver el ID creado
    SELECT SCOPE_IDENTITY() AS IdGenerado;
END;
GO

CREATE OR ALTER PROCEDURE sp_ObtenerReservas
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        -- 🔑 IDs (backend)
        r.ID_Reserva AS Id,
        r.ID_Fecha AS FechaId,
        r.ID_Usuario AS UserId,
        f.ID_Experiencia AS ExperienceId,

        -- 🧾 Datos enriquecidos (frontend)
        u.Nombre AS UserName,
        e.Titulo AS ExperienceName,
        c.Nombre_Comunidad AS ComunidadName,

        -- 📅 Datos de reserva
        CAST(r.Fecha_Reserva AS datetime2) AS DateFrom,
        CAST(r.Fecha_Reserva AS datetime2) AS DateTo,

        CASE 
            WHEN r.Estado = 1 THEN 'Confirmed' 
            ELSE 'Pending' 
        END AS Status,

        GETUTCDATE() AS CreatedAt

    FROM Reserva r

    INNER JOIN Usuario u 
        ON r.ID_Usuario = u.ID_Usuario

    INNER JOIN Fecha_Disponibilidad f 
        ON r.ID_Fecha = f.ID_Fecha

    INNER JOIN Experiencia e 
        ON f.ID_Experiencia = e.ID_Experiencia

    INNER JOIN Comunidad c 
        ON e.ID_Comunidad = c.ID_Comunidad;
END;
GO