-- =========================================
-- COMPLETE DATABASE SETUP FOR TURISMORURAL
-- =========================================
-- Complete setup: database, tables, seed data, and stored procedures
-- Run this single script to set up the entire database from scratch

-- =========================================
-- CREATE DATABASE
-- =========================================
IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'TurismoRural')
BEGIN
    CREATE DATABASE TurismoRural;
END
GO

-- TURISMORURAL DATABASE - SETUP WITH JWT & PASSWORD ENCRYPTION
-- Las contraseñas se encriptan con AES-256 en la API
-- Los tokens JWT tienen validez de 10 minutos

--CREATE DATABASE TurismoRural;

USE TurismoRural;
GO

-- =========================================
-- CREATE TABLES (IF NOT EXISTS)
-- =========================================

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Rol')
CREATE TABLE Rol (
    ID_Rol INT PRIMARY KEY IDENTITY(1,1),
    Rol_Nombre VARCHAR(50) NOT NULL
);
GO

CREATE TABLE Usuario (
    ID_Usuario INT PRIMARY KEY IDENTITY(1,1),
    Nombre VARCHAR(100) NOT NULL,
    Correo VARCHAR(100) UNIQUE NOT NULL,
    Telefono VARCHAR(20),
    Contrasena VARCHAR(255) NOT NULL,
    ID_Rol INT NOT NULL,
    Fecha_Registro DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ID_Rol) REFERENCES Rol(ID_Rol)
);
GO

CREATE TABLE Comunidad (
    ID_Comunidad INT PRIMARY KEY IDENTITY(1,1),
    Nombre_Comunidad VARCHAR(50) NOT NULL,
	Pais VARCHAR(50) NOT NULL,
	Provincia VARCHAR(30),
    Descripcion VARCHAR(255),
);
GO

CREATE TABLE Experiencia (
    ID_Experiencia INT PRIMARY KEY IDENTITY(1,1),
    Titulo VARCHAR(50) NOT NULL,
    Descripcion VARCHAR(150),
    Categoria VARCHAR(150),
	UsuarioIdRegistrador  INT,
    ID_Comunidad INT,
    FOREIGN KEY (ID_Comunidad) REFERENCES Comunidad(ID_Comunidad),
	FOREIGN KEY (UsuarioIdRegistrador) REFERENCES Usuario(ID_Usuario)
);
GO

CREATE TABLE ExperienciaConcurrencia (
    ID_Concurrencia INT PRIMARY KEY IDENTITY(1,1),
    Fecha DATE,
	Detalle NVARCHAR(255),
	Precio DECIMAL,
    Cupos_Disponibles INT,
    ID_Experiencia INT,
    FOREIGN KEY (ID_Experiencia) REFERENCES Experiencia(ID_Experiencia)
);
GO

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

CREATE TABLE ErrorSistema (
    ID_Error INT PRIMARY KEY IDENTITY(1,1),
    MensajeError VARCHAR(500),
    StackTrace VARCHAR(MAX),
    Metodo VARCHAR(100),
    Fecha DATETIME DEFAULT GETDATE()
);
GO

---------------------- Procedimientos almacenados -------------------------------------
-- Las contraseñas recibidas están encriptadas por la API
-- Clave AES: G7kP2mX9Qa4ZtL8wR1bY6HcD3sN5uFjV

-- Registrar usuario
CREATE PROCEDURE SP_RegistrarUsuario
	@Nombre VARCHAR(100),
	@Correo VARCHAR(100),
	@Telefono VARCHAR(20),
	@Contrasena VARCHAR(255),
	@ID_Rol INT
AS
BEGIN
    CREATE TABLE Rol (
        ID_Rol INT PRIMARY KEY IDENTITY(1,1),
        Rol_Nombre VARCHAR(50) NOT NULL
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Usuario')
BEGIN
    CREATE TABLE Usuario (
        ID_Usuario INT PRIMARY KEY IDENTITY(1,1),
        Nombre VARCHAR(100) NOT NULL,
        Correo VARCHAR(100) UNIQUE NOT NULL,
        Telefono VARCHAR(20),
        Contrasena VARCHAR(255) NOT NULL,
        ID_Rol INT NOT NULL,
        Fecha_Registro DATETIME DEFAULT GETDATE(),
        FOREIGN KEY (ID_Rol) REFERENCES Rol(ID_Rol)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Comunidad')
BEGIN
    CREATE TABLE Comunidad (
        ID_Comunidad INT PRIMARY KEY IDENTITY(1,1),
        Nombre_Comunidad VARCHAR(50) NOT NULL,
        Pais VARCHAR(50) NOT NULL,
        Provincia VARCHAR(30),
        Descripcion VARCHAR(255)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Experiencia')
BEGIN
    CREATE TABLE Experiencia (
        ID_Experiencia INT PRIMARY KEY IDENTITY(1,1),
        Titulo VARCHAR(50) NOT NULL,
        Descripcion VARCHAR(150),
        Categoria VARCHAR(150),
        UsuarioIdRegistrador INT,
        ID_Comunidad INT,
        FOREIGN KEY (UsuarioIdRegistrador) REFERENCES Usuario(ID_Usuario),
        FOREIGN KEY (ID_Comunidad) REFERENCES Comunidad(ID_Comunidad)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ExperienciaConcurrencia')
BEGIN
    CREATE TABLE ExperienciaConcurrencia (
        ID_Concurrencia INT PRIMARY KEY IDENTITY(1,1),
        Fecha DATE,
        Detalle NVARCHAR(255),
        Precio DECIMAL,
        Cupos_Disponibles INT,
        ID_Experiencia INT,
        FOREIGN KEY (ID_Experiencia) REFERENCES Experiencia(ID_Experiencia)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CatalogoEstado')
BEGIN
    CREATE TABLE CatalogoEstado (
        ID_estado INT PRIMARY KEY,
        Descripcion VARCHAR(50)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Reserva')
BEGIN
    CREATE TABLE Reserva ( 
        ID_Reserva INT PRIMARY KEY IDENTITY(1,1),
        Fecha_Reserva DATETIME2,
        Cantidad_Personas INT,
        Estado INT,
        ID_Usuario INT,
        ID_Concurrencia INT,
        FOREIGN KEY (ID_Usuario) REFERENCES Usuario(ID_Usuario),
        FOREIGN KEY (ID_Concurrencia) REFERENCES ExperienciaConcurrencia(ID_Concurrencia)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ErrorSistema')
BEGIN
    CREATE TABLE ErrorSistema (
        ID_Error INT PRIMARY KEY IDENTITY(1,1),
        MensajeError VARCHAR(500),
        StackTrace VARCHAR(MAX),
        Metodo VARCHAR(100),
        Fecha DATETIME DEFAULT GETDATE()
    );
END
GO

-- =========================================
-- SEED DATA
-- =========================================

-- Insert Roles (if not exists)
IF NOT EXISTS (SELECT 1 FROM Rol WHERE Rol_Nombre = 'Administrador')
BEGIN
    INSERT INTO Rol (Rol_Nombre) VALUES ('Administrador');
END
GO

IF NOT EXISTS (SELECT 1 FROM Rol WHERE Rol_Nombre = 'Usuario')
BEGIN
    INSERT INTO Rol (Rol_Nombre) VALUES ('Usuario');
END
GO

-- Insert Users (if not exists)
IF NOT EXISTS (SELECT 1 FROM Usuario WHERE Correo = 'jonathan@correo.com')
BEGIN
    INSERT INTO Usuario (Nombre, Correo, Telefono, Contrasena, ID_Rol)
    VALUES ('Jonathan', 'jonathan@correo.com', '88881111', 'DkF5eJ1UhQwmEXbYNJDqmQ==', 1);
END
GO

-- Insert Communities (if not exists)
IF NOT EXISTS (SELECT 1 FROM Comunidad WHERE Nombre_Comunidad = 'Mercedes Norte')
BEGIN
    INSERT INTO Comunidad (Nombre_Comunidad, Pais, Provincia, Descripcion)
    VALUES
    ('Mercedes Norte', 'Costa Rica', 'Heredia', 'Alegre y urbana comunidad'),
    ('San Rafael', 'Costa Rica', 'Heredia', 'Zona monta�osa'),
    ('Monteverde', 'Costa Rica', 'Puntarenas', 'Alta biodiversidad'),
    ('La Fortuna', 'Costa Rica', 'Alajuela', 'Volc�n Arenal'),
    ('Puerto Viejo', 'Costa Rica', 'Lim�n', 'Cultura caribe�a');
END
GO

-- Insert Experiences (if not exists)
IF NOT EXISTS (SELECT 1 FROM Experiencia WHERE Titulo = 'Tour en canopy')
BEGIN
    INSERT INTO Experiencia (Titulo, Descripcion, Categoria, UsuarioIdRegistrador, ID_Comunidad)
    VALUES
    ('Tour en canopy', 'Tirolesa en bosque', 'Aventura', 1, 1),
    ('Caminata volc�n Arenal', 'Tour guiado', 'Naturaleza', 1, 4),
    ('Clase de surf', 'Surf b�sico', 'Deportes', 1, 5);
END
GO

-- Insert Experience Concurrences (if not exists)
IF NOT EXISTS (SELECT 1 FROM ExperienciaConcurrencia WHERE Detalle = 'Canopy ma�ana')
BEGIN
    INSERT INTO ExperienciaConcurrencia (Fecha, Detalle, Precio, Cupos_Disponibles, ID_Experiencia)
    VALUES
    ('2026-04-10', 'Canopy ma�ana', 50.00, 10, 1),
    ('2026-04-11', 'Canopy tarde', 55.00, 8, 1),
    ('2026-04-15', 'Caminata volc�n', 40.00, 15, 2),
    ('2026-04-20', 'Surf b�sico', 30.00, 12, 3),
    ('2026-04-21', 'Surf intermedio', 35.00, 10, 3);
END
GO

-- Insert Catalog States (if not exists)
IF NOT EXISTS (SELECT 1 FROM CatalogoEstado WHERE ID_estado = 1)
BEGIN
    INSERT INTO CatalogoEstado (ID_estado, Descripcion) VALUES
    (1, 'Pendiente'),
    (2, 'Confirmada'),
    (3, 'Cancelada'),
    (4, 'Completada');
END
GO

-- Insert Sample Reservations (if not exists)
IF NOT EXISTS (SELECT 1 FROM Reserva WHERE ID_Usuario = 1 AND ID_Concurrencia = 1)
BEGIN
    INSERT INTO Reserva (Fecha_Reserva, Cantidad_Personas, Estado, ID_Usuario, ID_Concurrencia)
    VALUES
    (GETDATE(), 2, 1, 1, 1),
    (GETDATE(), 4, 2, 1, 3);
END
GO

-- =========================================
-- STORED PROCEDURES - CATALOG
-- =========================================

-- Procedure: Get all catalog states (status values)
CREATE PROCEDURE SP_ConsultarCatalogoEstado
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ID_estado,
        Descripcion
    FROM CatalogoEstado
    ORDER BY ID_estado;
END;
GO

-- =========================================
-- STORED PROCEDURES - EXPERIENCIAS
-- =========================================

-- Drop existing procedures (if needed for clean slate)
IF OBJECT_ID('SP_ConsultarExperiencias', 'P') IS NOT NULL DROP PROCEDURE SP_ConsultarExperiencias;
IF OBJECT_ID('SP_ConsultarExperienciasPorId', 'P') IS NOT NULL DROP PROCEDURE SP_ConsultarExperienciasPorId;
IF OBJECT_ID('SP_InsertarExperiencia', 'P') IS NOT NULL DROP PROCEDURE SP_InsertarExperiencia;
IF OBJECT_ID('SP_ActualizarExperiencia', 'P') IS NOT NULL DROP PROCEDURE SP_ActualizarExperiencia;
IF OBJECT_ID('SP_EliminarExperiencia', 'P') IS NOT NULL DROP PROCEDURE SP_EliminarExperiencia;
IF OBJECT_ID('SP_ConsultarExperienciasConcurrencia', 'P') IS NOT NULL DROP PROCEDURE SP_ConsultarExperienciasConcurrencia;
IF OBJECT_ID('SP_ConsultarExperienciaConcurrenciaPorId', 'P') IS NOT NULL DROP PROCEDURE SP_ConsultarExperienciaConcurrenciaPorId;
IF OBJECT_ID('SP_InsertarExperienciaConcurrencia', 'P') IS NOT NULL DROP PROCEDURE SP_InsertarExperienciaConcurrencia;
IF OBJECT_ID('SP_ActualizarExperienciaConcurrencia', 'P') IS NOT NULL DROP PROCEDURE SP_ActualizarExperienciaConcurrencia;
IF OBJECT_ID('SP_EliminarExperienciaConcurrencia', 'P') IS NOT NULL DROP PROCEDURE SP_EliminarExperienciaConcurrencia;
IF OBJECT_ID('SP_ConsultarCatalogoEstado', 'P') IS NOT NULL DROP PROCEDURE SP_ConsultarCatalogoEstado;
GO

-- Procedure: Get all experiences
CREATE PROCEDURE SP_ConsultarExperiencias
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        e.ID_Experiencia,
        e.Titulo,
        e.Descripcion,
        e.Categoria,
        e.UsuarioIdRegistrador,
        e.ID_Comunidad
    FROM Experiencia e;
END;
GO

-- Procedure: Get experience by ID
CREATE PROCEDURE SP_ConsultarExperienciasPorId
    @ID_Experiencia INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        e.ID_Experiencia,
        e.Titulo,
        e.Descripcion,
        e.Categoria,
        e.UsuarioIdRegistrador,
        e.ID_Comunidad
    FROM Experiencia e
    WHERE e.ID_Experiencia = @ID_Experiencia;
END;
GO

-- Procedure: Insert a new experience
CREATE PROCEDURE SP_InsertarExperiencia
    @Titulo VARCHAR(50),
    @Descripcion VARCHAR(150),
    @Categoria VARCHAR(150),
    @UsuarioIdRegistrador INT,
    @ID_Comunidad INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Experiencia (
        Titulo,
        Descripcion,
        Categoria,
        UsuarioIdRegistrador,
        ID_Comunidad
    INSERT INTO Reserva (
        Fecha_Reserva,
        Cantidad_Personas,  
		ID_Usuario,
        Estado,
        ID_Concurrencia
    )
    VALUES (
        @Titulo,
        @Descripcion,
        @Categoria,
        @UsuarioIdRegistrador,
        @ID_Comunidad
    );

    SELECT SCOPE_IDENTITY() AS ID_Experiencia;
END;
GO

-- Procedure: Update an experience
CREATE PROCEDURE SP_ActualizarExperiencia
    @ID_Experiencia INT,
    @Titulo VARCHAR(50),
    @Descripcion VARCHAR(150),
    @Categoria VARCHAR(150),
    @UsuarioIdRegistrador INT,
    @ID_Comunidad INT
AS
BEGIN
    SET NOCOUNT OFF;

    UPDATE Experiencia
    SET
        Titulo = @Titulo,
        Descripcion = @Descripcion,
        Categoria = @Categoria,
        UsuarioIdRegistrador = @UsuarioIdRegistrador,
        ID_Comunidad = @ID_Comunidad
    WHERE ID_Experiencia = @ID_Experiencia;
END;
GO

-- Procedure: Delete an experience
CREATE PROCEDURE SP_EliminarExperiencia
    @ID_Experiencia INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Experiencia WHERE ID_Experiencia = @ID_Experiencia)
    BEGIN
        SELECT 0 AS Resultado;
        RETURN;
    END

    DELETE FROM Experiencia
    WHERE ID_Experiencia = @ID_Experiencia;

    SELECT 1 AS Resultado;
END;
GO

-- =========================================
-- STORED PROCEDURES - EXPERIENCIAS CONCURRENCIA
-- =========================================

-- Procedure: Get all experience occurrences
CREATE PROCEDURE SP_ConsultarExperienciasConcurrencia
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ec.ID_Concurrencia,
        ec.Fecha,
        ec.Detalle,
        ec.Precio,
        ec.Cupos_Disponibles,
        ec.ID_Experiencia
    FROM ExperienciaConcurrencia ec;
END;
GO

-- Procedure: Get experience occurrence by ID
CREATE PROCEDURE SP_ConsultarExperienciaConcurrenciaPorId
    @ID_Concurrencia INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ec.ID_Concurrencia,
        ec.Fecha,
        ec.Detalle,
        ec.Precio,
        ec.Cupos_Disponibles,
        ec.ID_Experiencia
    FROM ExperienciaConcurrencia ec
    WHERE ec.ID_Concurrencia = @ID_Concurrencia;
END;
GO

-- Procedure: Insert a new experience occurrence
CREATE PROCEDURE SP_InsertarExperienciaConcurrencia
    @Fecha DATE,
    @Detalle NVARCHAR(255),
    @Precio DECIMAL,
    @Cupos_Disponibles INT,
    @ID_Experiencia INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO ExperienciaConcurrencia (
        Fecha,
        Detalle,
        Precio,
        Cupos_Disponibles,
        ID_Experiencia
    )
    VALUES (
        @Fecha,
        @Detalle,
        @Precio,
        @Cupos_Disponibles,
        @ID_Experiencia
    );

    SELECT SCOPE_IDENTITY() AS ID_Concurrencia;
END;
GO

-- Procedure: Update an experience occurrence
CREATE PROCEDURE SP_ActualizarExperienciaConcurrencia
    @ID_Concurrencia INT,
    @Fecha DATE,
    @Detalle NVARCHAR(255),
    @Precio DECIMAL,
    @Cupos_Disponibles INT,
    @ID_Experiencia INT
AS
BEGIN
    SET NOCOUNT OFF;

    UPDATE ExperienciaConcurrencia
    SET
        Fecha = @Fecha,
        Detalle = @Detalle,
        Precio = @Precio,
        Cupos_Disponibles = @Cupos_Disponibles,
        ID_Experiencia = @ID_Experiencia
    WHERE ID_Concurrencia = @ID_Concurrencia;
	UPDATE Comunidad
	SET Nombre_Comunidad = @Nombre_Comunidad,
		Pais = @Pais,
		Provincia = @Provincia,
		Descripcion = @Descripcion
	WHERE ID_Comunidad = @ID_Comunidad;

	IF @@ROWCOUNT = 0
		PRINT 'No se encontró ninguna comunidad con el ID proporcionado.';
	ELSE
		PRINT 'Comunidad actualizada con éxito.';
END;
GO

-- Procedure: Delete an experience occurrence
CREATE PROCEDURE SP_EliminarExperienciaConcurrencia
    @ID_Concurrencia INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM ExperienciaConcurrencia WHERE ID_Concurrencia = @ID_Concurrencia)
    BEGIN
        SELECT 0 AS Resultado;
        RETURN;
    END

    DELETE FROM ExperienciaConcurrencia
    WHERE ID_Concurrencia = @ID_Concurrencia;

    SELECT 1 AS Resultado;
END;
GO

-- =========================================
-- STORED PROCEDURES - COMUNIDADES
-- =========================================

-- Drop existing procedures (if needed for clean slate)
IF OBJECT_ID('SP_ConsultarComunidades', 'P') IS NOT NULL DROP PROCEDURE SP_ConsultarComunidades;
IF OBJECT_ID('SP_InsertarComunidad', 'P') IS NOT NULL DROP PROCEDURE SP_InsertarComunidad;
IF OBJECT_ID('SP_ActualizarComunidad', 'P') IS NOT NULL DROP PROCEDURE SP_ActualizarComunidad;
GO

-- Procedure: Get all communities
CREATE PROCEDURE SP_ConsultarComunidades
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ID_Comunidad,
        Nombre_Comunidad,
        Pais,
        Provincia,
        Descripcion
    FROM Comunidad;
END;
GO

-- Procedure: Insert a new community
CREATE PROCEDURE SP_InsertarComunidad
    @Nombre_Comunidad VARCHAR(50),
    @Pais VARCHAR(50),
    @Provincia VARCHAR(30),
    @Descripcion VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Comunidad (
        Nombre_Comunidad,
        Pais,
        Provincia,
        Descripcion
    )
    VALUES (
        @Nombre_Comunidad,
        @Pais,
        @Provincia,
        @Descripcion
    );

    SELECT SCOPE_IDENTITY() AS ID_Comunidad;
END;
GO

-- Procedure: Update a community
CREATE PROCEDURE SP_ActualizarComunidad
    @ID_Comunidad INT,
    @Nombre_Comunidad VARCHAR(50),
    @Pais VARCHAR(50),
    @Provincia VARCHAR(30),
    @Descripcion VARCHAR(255)
AS
BEGIN
    SET NOCOUNT OFF;

    UPDATE Comunidad
    SET
        Nombre_Comunidad = @Nombre_Comunidad,
        Pais = @Pais,
        Provincia = @Provincia,
        Descripcion = @Descripcion
    WHERE ID_Comunidad = @ID_Comunidad;
END;
GO

-- =========================================
-- STORED PROCEDURES - USUARIOS (USERS)
-- =========================================

-- Drop existing procedures (if needed for clean slate)
IF OBJECT_ID('SP_RegistrarUsuario', 'P') IS NOT NULL DROP PROCEDURE SP_RegistrarUsuario;
GO

-- Procedure: Register a new user
CREATE PROCEDURE SP_RegistrarUsuario
    @Nombre VARCHAR(100),
    @Correo VARCHAR(100),
    @Telefono VARCHAR(20),
    @Contrasena VARCHAR(255),
    @ID_Rol INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Usuario (
        Nombre,
        Correo,
        Telefono,
        Contrasena,
        ID_Rol,
        Fecha_Registro
    )
    VALUES (
        @Nombre,
        @Correo,
        @Telefono,
        @Contrasena,
        @ID_Rol,
        GETDATE()
    );

    SELECT SCOPE_IDENTITY() AS ID_Usuario;
END;
GO

-- =========================================
-- STORED PROCEDURES - RESERVAS
-- =========================================

-- Drop existing procedures (if needed for clean slate)
IF OBJECT_ID('sp_ObtenerReservaPorId', 'P') IS NOT NULL DROP PROCEDURE sp_ObtenerReservaPorId;
IF OBJECT_ID('SP_ConsultarReservas', 'P') IS NOT NULL DROP PROCEDURE SP_ConsultarReservas;
IF OBJECT_ID('SP_ObtenerReservasPorUsuario', 'P') IS NOT NULL DROP PROCEDURE SP_ObtenerReservasPorUsuario;
IF OBJECT_ID('sp_CrearReserva', 'P') IS NOT NULL DROP PROCEDURE sp_CrearReserva;
IF OBJECT_ID('sp_ActualizarReserva', 'P') IS NOT NULL DROP PROCEDURE sp_ActualizarReserva;
IF OBJECT_ID('sp_EliminarReserva', 'P') IS NOT NULL DROP PROCEDURE sp_EliminarReserva;
GO

-- Procedure: Get reservation by ID
CREATE PROCEDURE sp_ObtenerReservaPorId
    @ID_Reserva INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        r.ID_Reserva,
        r.ID_Usuario,
        r.ID_Concurrencia,
        r.Cantidad_Personas,
        CAST(r.Fecha_Reserva AS datetime2) AS Fecha_Reserva,
        u.Nombre AS NombreUsuario,
        e.Titulo AS NombreConcurrencia,
        r.Estado,
        Ct.Descripcion AS EstadoNombre
    FROM Reserva r
    INNER JOIN Usuario u ON r.ID_Usuario = u.ID_Usuario
    INNER JOIN ExperienciaConcurrencia ec ON r.ID_Concurrencia = ec.ID_Concurrencia
    INNER JOIN Experiencia e ON ec.ID_Experiencia = e.ID_Experiencia
    INNER JOIN CatalogoEstado Ct ON Ct.ID_estado = r.Estado
    WHERE r.ID_Reserva = @ID_Reserva;
END;
GO

-- Procedure: Get all reservations
CREATE PROCEDURE SP_ConsultarReservas
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        R.ID_Reserva,
        U.Nombre AS NombreUsuario,
        E.Titulo AS NombreConcurrencia,
        CAST(FD.Fecha AS datetime2) AS Fecha_Reserva,
        R.Cantidad_Personas,
        R.ID_Usuario,
        R.ID_Concurrencia,
        R.Estado,
        Ct.Descripcion AS EstadoNombre
    FROM Reserva R
    INNER JOIN Usuario U ON R.ID_Usuario = U.ID_Usuario
    INNER JOIN ExperienciaConcurrencia FD ON R.ID_Concurrencia = FD.ID_Concurrencia
    INNER JOIN Experiencia E ON FD.ID_Experiencia = E.ID_Experiencia
    INNER JOIN CatalogoEstado Ct ON Ct.ID_estado = R.Estado;
END;
GO

-- Procedure: Get reservations by user ID
CREATE PROCEDURE SP_ObtenerReservasPorUsuario
    @ID_Usuario INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        R.ID_Reserva,
        R.ID_Usuario,
        R.ID_Concurrencia,
        R.Cantidad_Personas,
        CAST(R.Fecha_Reserva AS datetime2) AS Fecha_Reserva,
        E.Titulo AS NombreConcurrencia,
        R.Estado,
        Ct.Descripcion AS EstadoNombre
    FROM Reserva R
    INNER JOIN CatalogoEstado Ct ON Ct.ID_estado = R.Estado
    INNER JOIN ExperienciaConcurrencia FD ON R.ID_Concurrencia = FD.ID_Concurrencia
    INNER JOIN Experiencia E ON FD.ID_Experiencia = E.ID_Experiencia
    WHERE R.ID_Usuario = @ID_Usuario;
END;
GO

-- Procedure: Create a new reservation
CREATE PROCEDURE sp_CrearReserva
    @ID_Usuario INT,
    @ID_Concurrencia INT,
    @Cantidad_Personas INT,
    @Estado INT
AS
BEGIN
    SET NOCOUNT ON;
	INSERT INTO Rol (Rol_Nombre) VALUES ('Usuario');

-- Datos de prueba (comentados)
/*
INSERT INTO Usuario (Nombre, Correo, Telefono, Contrasena, ID_Rol)
VALUES 
('Carlos Méndez', 'carlos.mendez@gmail.com', '88881111', 'encrypted_password', 1),
('Ana Rodríguez', 'ana.rodriguez@gmail.com', '88882222', 'encrypted_password', 2),
('Luis Fernández', 'luis.fernandez@gmail.com', '88883333', 'encrypted_password', 2),
('María Gómez', 'maria.gomez@gmail.com', '88884444', 'encrypted_password', 2),
('José Vargas', 'jose.vargas@gmail.com', '88885555', 'encrypted_password', 1);

INSERT INTO Comunidad (Nombre_Comunidad, Pais, Provincia, Descripcion)
VALUES
('Mercedes Norte' , 'Costa Rica', 'Heredia', 'Alegre y urbana comunidad de Mercedes Norte.'),
('San Rafael' , 'Costa Rica', 'Heredia', 'Ciudad en los altos de Heredia, bastante fria con mucho verde.');




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


    IF @Cantidad_Personas <= 0
    BEGIN
        RAISERROR('Cantidad de personas inv�lida', 16, 1);
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

    SELECT SCOPE_IDENTITY() AS ID_Reserva;
END;
GO

-- Procedure: Update an existing reservation
CREATE PROCEDURE sp_ActualizarReserva
    @ID_Reserva INT,
    @ID_Usuario INT,
    @ID_Concurrencia INT,
    @Cantidad_Personas INT,
    @Estado INT
AS
BEGIN
    SET NOCOUNT OFF;

    UPDATE Reserva
    SET 
        ID_Usuario = @ID_Usuario,
        ID_Concurrencia = @ID_Concurrencia,
        Cantidad_Personas = @Cantidad_Personas,
        Estado = @Estado
    WHERE ID_Reserva = @ID_Reserva;
END;
GO

-- Procedure: Delete a reservation
CREATE PROCEDURE sp_EliminarReserva
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
GO
select * from Usuario;
select * from reserva;
