CREATE  or alter PROCEDURE sp_ObtenerReservaPorId
    @ID_Reserva INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        -- IDs
        r.ID_Reserva AS iD_Reserva,
        r.ID_Usuario AS ID_Usuario,
        r.ID_Fecha AS ID_Fecha,
        ec.ID_Experiencia AS ID_Experiencia,

        -- Datos enriquecidos
        u.Nombre AS NombreUsuario,
        e.Titulo AS TituloExperiencia,

        -- Fechas
        CAST(r.Fecha_Reserva AS datetime2) AS Fecha_Reserva,
        CAST(ec.Fecha AS datetime2) AS Fecha_Experiencia,

        -- Estado
        r.Estado AS Estado,

        -- Extras útiles
        ec.Precio,
        ec.Cupos_Disponibles

    FROM Reserva r

    INNER JOIN Usuario u 
        ON r.ID_Usuario = u.ID_Usuario

    INNER JOIN ExperienciaConcurrencia ec 
        ON r.ID_Fecha = ec.ID_Concurrencia

    INNER JOIN Experiencia e 
        ON ec.ID_Experiencia = e.ID_Experiencia

    WHERE r.ID_Reserva = @ID_Reserva;
END;
Go
CREATE OR ALTER PROCEDURE SP_ConsultarReservas
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        R.ID_Reserva AS iD_Reserva,
        U.Nombre AS NombreUsuario,
        E.Titulo AS TituloExperiencia,
        CAST(FD.Fecha AS datetime2) AS Fecha_Reserva,
        R.Cantidad_Personas AS Cantidad_Personas,
        R.ID_Usuario AS ID_Usuario,
        R.ID_Fecha AS ID_Fecha,
		R.Estado AS Estado

    FROM Reserva R
    INNER JOIN Usuario U
        ON R.ID_Usuario = U.ID_Usuario
    INNER JOIN ExperienciaConcurrencia FD
        ON R.ID_Fecha = FD.ID_Concurrencia
    INNER JOIN Experiencia E
        ON FD.ID_Experiencia = E.ID_Experiencia;
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
CREATE OR ALTER PROCEDURE SP_ObtenerReservasPorUsuario
    @ID_Usuario INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ID_Reserva AS iD_Reserva,
        ID_Fecha AS ID_Fecha,
        ID_Usuario AS ID_Usuario,
		Cantidad_Personas,
        CAST(Fecha_Reserva AS datetime2) AS fecha_Reserva,
        CASE 
            WHEN Estado = 1 THEN 'Confirmed' 
            ELSE 'Pending' 
        END AS Status,
        GETUTCDATE() AS CreatedAt
    FROM Reserva
    WHERE ID_Usuario = @ID_Usuario;
END;
GO
