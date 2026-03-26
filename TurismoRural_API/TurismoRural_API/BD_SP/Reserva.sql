CREATE  or alter PROCEDURE sp_ObtenerReservaPorId
    @ID_Reserva INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        r.ID_Reserva AS iD_Reserva,
        r.ID_Usuario AS ID_Usuario,
        r.ID_Concurrencia AS ID_Fecha,
        ec.ID_Experiencia AS ID_Experiencia,
		R.Cantidad_Personas AS Cantidad_Personas,
		CAST(r.Fecha_Reserva AS datetime2) AS Fecha_Reserva,
        u.Nombre AS NombreUsuario,
        e.Titulo AS nombreConcurrencia,
		r.Estado AS Estado,
		ec.ID_Concurrencia as iD_Concurrencia,
				Ct.Descripcion as EstadoNombre


    FROM Reserva r

    INNER JOIN Usuario u 
        ON r.ID_Usuario = u.ID_Usuario
    INNER JOIN ExperienciaConcurrencia ec 
        ON r.iD_Concurrencia = ec.ID_Concurrencia
    INNER JOIN Experiencia e 
        ON ec.ID_Experiencia = e.ID_Experiencia
	INNER JOIN CatalogoEstado Ct 
		on Ct.ID_estado = r.Estado

    WHERE r.ID_Reserva = @ID_Reserva;
END;
GO
CREATE OR ALTER PROCEDURE SP_ConsultarReservas
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        R.ID_Reserva AS iD_Reserva,
        U.Nombre AS NombreUsuario,
        E.Titulo AS nombreConcurrencia,
        CAST(FD.Fecha AS datetime2) AS Fecha_Reserva,
        R.Cantidad_Personas AS Cantidad_Personas,
        R.ID_Usuario AS ID_Usuario,
        R.ID_Concurrencia AS iD_Concurrencia,
        R.Estado AS Estado,
		Ct.Descripcion as EstadoNombre


    FROM Reserva R
    INNER JOIN Usuario U
        ON R.ID_Usuario = U.ID_Usuario
    INNER JOIN ExperienciaConcurrencia FD
        ON R.ID_Concurrencia = FD.ID_Concurrencia
    INNER JOIN Experiencia E
        ON FD.ID_Experiencia = E.ID_Experiencia
	INNER JOIN CatalogoEstado Ct 
		on Ct.ID_estado = r.Estado
END;
GO
CREATE OR ALTER PROCEDURE SP_ObtenerReservasPorUsuario
    @ID_Usuario INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        R.ID_Reserva AS iD_Reserva,
        R.ID_Concurrencia AS ID_Fecha,
        R.ID_Usuario AS ID_Usuario,
        R.Cantidad_Personas,
        CAST(R.Fecha_Reserva AS datetime2) AS fecha_Reserva,
		e.Titulo AS nombreConcurrencia,
        R.Estado,
		R.ID_Concurrencia AS iD_Concurrencia,

        Ct.Descripcion AS EstadoNombre
    FROM Reserva R

    INNER JOIN CatalogoEstado Ct
        ON Ct.ID_estado = R.Estado
		INNER JOIN ExperienciaConcurrencia FD
        ON R.ID_Concurrencia = FD.ID_Concurrencia
		 INNER JOIN Experiencia E
        ON FD.ID_Experiencia = E.ID_Experiencia
    WHERE R.ID_Usuario = @ID_Usuario;

END;
GO
CREATE OR ALTER PROCEDURE sp_CrearReserva
    @ID_Usuario INT,
    @ID_Concurrencia INT,
    @Cantidad_Personas INT,
    @Estado INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validación básica
    IF @Cantidad_Personas <= 0
    BEGIN
        RAISERROR('Cantidad de personas inválida', 16, 1);
        RETURN;
    END

    INSERT INTO Reserva (
        Fecha_Reserva,
        Cantidad_Personas,
        Estado,
        ID_Usuario,
        ID_Concurrencia
    )
    VALUES (
        GETDATE(),
        @Cantidad_Personas,
        @Estado,
        @ID_Usuario,
        @ID_Concurrencia
    );

    -- Retornar ID creado
    SELECT SCOPE_IDENTITY() AS ID_Reserva;
END;
GO

    -- 👇 opcional pero PRO: devolver el ID creado
    SELECT SCOPE_IDENTITY() AS ID_Reserva;

END;
GO
CREATE OR ALTER PROCEDURE sp_ActualizarReserva
    @ID_Reserva INT,
    @ID_Usuario INT,
    @ID_Concurrencia INT,
    @Cantidad_Personas INT,
    @Estado INT
AS
BEGIN
    SET NOCOUNT OFF; -- IMPORTANTE para ExecuteAsync

    UPDATE Reserva
    SET 
        ID_Usuario = @ID_Usuario,
        ID_Concurrencia = @ID_Concurrencia,
        Cantidad_Personas = @Cantidad_Personas,
        Estado = @Estado
    WHERE ID_Reserva = @ID_Reserva;
END
GO
CREATE OR ALTER PROCEDURE sp_EliminarReserva
    @ID_Reserva INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Reserva WHERE ID_Reserva = @ID_Reserva)
    BEGIN
        SELECT 0 AS Resultado;
        RETURN;
    END

    DELETE FROM Reserva
    WHERE ID_Reserva = @ID_Reserva;

    SELECT 1 AS Resultado;
END;
go








CREATE TABLE Reserva ( 
    ID_Reserva INT PRIMARY KEY IDENTITY(1,1),
    Fecha_Reserva DATE,
    Cantidad_Personas INT,
    Estado INT,
    ID_Usuario INT,
    ID_Concurrencia INT,
    FOREIGN KEY (ID_Usuario) REFERENCES Usuario(ID_Usuario),
	FOREIGN KEY (ID_Concurrencia) REFERENCES ExperienciaConcurrencia(ID_Concurrencia)

);
GO
create table CatalogoEstado(
    ID_estado INT PRIMARY KEY,
    Descripcion VARCHAR(50)
);          
go


--inserts nescesarios para reservas 

INSERT INTO CatalogoEstado (ID_estado, Descripcion) VALUES (1, 'Pendiente');
INSERT INTO CatalogoEstado (ID_estado, Descripcion) VALUES (2, 'Confirmada');
INSERT INTO CatalogoEstado (ID_estado, Descripcion) VALUES (3, 'Cancelada');
INSERT INTO CatalogoEstado (ID_estado, Descripcion) VALUES (4, 'Completada');

INSERT INTO ExperienciaConcurrencia (Fecha, Detalle, Precio, Cupos_Disponibles, ID_Experiencia)
VALUES
('2026-04-10', 'Tour en canopy por la mañana', 50.00, 10, 1),
('2026-04-11', 'Tour en canopy por la tarde', 55.00, 8, 1),
('2026-04-15', 'Caminata guiada al volcán', 40.00, 15, 2),
('2026-04-20', 'Clase de surf para principiantes', 30.00, 12, 3),
('2026-04-21', 'Clase de surf intermedio', 35.00, 10, 3);

INSERT INTO Experiencia (Titulo, Descripcion, Categoria, UsuarioIdRegistrador, ID_Comunidad)
VALUES
('Tour en canopy', 'Recorrido en tirolesa por el bosque', 'Aventura', 1, 1),
('Caminata volcán Arenal', 'Exploración guiada cerca del volcán', 'Naturaleza', 2, 2),
('Clase de surf', 'Aprende a surfear en el Caribe', 'Deportes', 1, 3);

INSERT INTO Comunidad (Nombre_Comunidad, Pais, Provincia, Descripcion)
VALUES
('Monteverde', 'Costa Rica', 'Puntarenas', 'Zona turística famosa por su biodiversidad'),
('La Fortuna', 'Costa Rica', 'Alajuela', 'Destino popular por el volcán Arenal'),
('Puerto Viejo', 'Costa Rica', 'Limón', 'Comunidad costera con cultura caribeña');






