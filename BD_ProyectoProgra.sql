-- TURISMORURAL DATABASE - SETUP WITH JWT & PASSWORD ENCRYPTION
-- Las contraseñas se encriptan con AES-256 en la API
-- Los tokens JWT tienen validez de 10 minutos

--CREATE DATABASE TurismoRural;

USE TurismoRural;
GO


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
    Estado BIT,
    ID_Usuario INT,
    ID_Fecha INT,
    FOREIGN KEY (ID_Usuario) REFERENCES Usuario(ID_Usuario),
    FOREIGN KEY (ID_Fecha) REFERENCES ExperienciaConcurrencia(ID_Concurrencia)
);
GO

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
	INSERT INTO Usuario
	(Nombre, Correo, Telefono, Contrasena, ID_Rol)
	VALUES
	(@Nombre, @Correo, @Telefono, @Contrasena, @ID_Rol)
END;
GO

-- Login usuario (DEPRECADO - Manejado en C#)
CREATE PROCEDURE SP_LoginUsuario
	@Correo VARCHAR(100),
	@Contrasena VARCHAR(255)
AS
BEGIN
	SELECT *
	FROM Usuario
	WHERE Correo = @Correo
	AND Contrasena = @Contrasena
END;
GO

-- Consultar usuarios
CREATE PROCEDURE SP_ConsultarUsuarios
AS
BEGIN
	SELECT 
		U.ID_Usuario,
		U.Nombre,
		U.Correo,
		U.Telefono,
		R.Rol_Nombre
	FROM Usuario U
	INNER JOIN Rol R
	ON U.ID_Rol = R.ID_Rol
END;
GO

-- Actualizar usuario
CREATE PROCEDURE SP_ActualizarUsuario
	@ID_Usuario INT,
	@Nombre VARCHAR(100),
	@Telefono VARCHAR(20)
AS
BEGIN
	UPDATE Usuario
	SET 
		Nombre = @Nombre,
		Telefono = @Telefono
	WHERE ID_Usuario = @ID_Usuario
END;
GO

-- Eliminar usuario
CREATE PROCEDURE SP_EliminarUsuario
	@ID_Usuario INT
AS
BEGIN
	DELETE FROM Usuario
	WHERE ID_Usuario = @ID_Usuario
END;
GO

-- Crear reserva
CREATE PROCEDURE SP_CrearReserva
	@Fecha_Reserva DATE,
	@Cantidad_Personas INT,
	@Estado BIT,
	@ID_Usuario INT,
	@ID_Fecha INT
AS
BEGIN
	INSERT INTO Reserva
	(Fecha_Reserva, Cantidad_Personas, Estado, ID_Usuario, ID_Fecha)
	VALUES
	(@Fecha_Reserva, @Cantidad_Personas, @Estado, @ID_Usuario, @ID_Fecha)
END;
GO

-- Consultar reservas
CREATE PROCEDURE SP_ConsultarReservas
AS
BEGIN
	SELECT
		R.ID_Reserva,
		U.Nombre,
		E.Titulo,
		FD.Fecha,
		R.Cantidad_Personas
	FROM Reserva R
	INNER JOIN Usuario U
	ON R.ID_Usuario = U.ID_Usuario
	INNER JOIN ExperienciaConcurrencia FD
	ON R.ID_Fecha = FD.ID_Concurrencia
	INNER JOIN Experiencia E
	ON FD.ID_Experiencia = E.ID_Experiencia
END;
GO

-- Guardar errores
CREATE PROCEDURE SP_RegistrarError
	@MensajeError VARCHAR(500),
	@StackTrace VARCHAR(MAX),
	@Metodo VARCHAR(100)
AS
BEGIN
	INSERT INTO ErrorSistema
	(MensajeError, StackTrace, Metodo)
	VALUES
	(@MensajeError, @StackTrace, @Metodo)
END;
GO

-- Consultar comunidades
CREATE PROCEDURE SP_ConsultarComunidades
AS
BEGIN
	SELECT ID_Comunidad, Nombre_Comunidad, Pais, Provincia, Descripcion
	FROM Comunidad;
END;
GO

-- Insertar comunidad
CREATE PROCEDURE SP_InsertarComunidad
	@Nombre_Comunidad VARCHAR(50),
	@Pais VARCHAR(50),
	@Provincia VARCHAR(30),
	@Descripcion VARCHAR(255)
AS
BEGIN
	INSERT INTO Comunidad (Nombre_Comunidad, Pais, Provincia, Descripcion)
	VALUES (@Nombre_Comunidad, @Pais, @Provincia, @Descripcion);

	PRINT 'Comunidad insertada correctamente.';
END;
GO

-- Actualizar comunidad
CREATE PROCEDURE SP_ActualizarComunidad
	@ID_Comunidad INT,
	@Nombre_Comunidad VARCHAR(50),
	@Pais VARCHAR(50),
	@Provincia VARCHAR(30),
	@Descripcion VARCHAR(255)
AS
BEGIN
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

-- Consultar experiencias
CREATE PROCEDURE SP_ConsultarExperiencias
AS
BEGIN
	SELECT ID_Experiencia, Titulo, Descripcion, Categoria, UsuarioIdRegistrador, ID_Comunidad
	FROM Experiencia;
END;
GO

-- Consultar experiencia por ID
CREATE PROCEDURE SP_ConsultarExperienciasPorId
	@ID_Experiencia INT
AS
BEGIN
	SELECT ID_Experiencia, Titulo, Descripcion, Categoria, UsuarioIdRegistrador, ID_Comunidad
	FROM Experiencia
	WHERE ID_Experiencia = @ID_Experiencia;
END;
GO

-- Insertar experiencia
CREATE PROCEDURE SP_InsertarExperiencia
	@Titulo VARCHAR(50),
	@Descripcion VARCHAR(150),
	@Categoria VARCHAR(150),
	@UsuarioIdRegistrador INT,
	@ID_Comunidad INT
AS
BEGIN
	INSERT INTO Experiencia (Titulo, Descripcion, Categoria, UsuarioIdRegistrador, ID_Comunidad)
	VALUES (@Titulo, @Descripcion, @Categoria, @UsuarioIdRegistrador, @ID_Comunidad);
END;
GO

-- Actualizar experiencia
CREATE PROCEDURE SP_ActualizarExperiencia
	@ID_Experiencia INT,
	@Titulo VARCHAR(50) = NULL,
	@Descripcion VARCHAR(150) = NULL,
	@Categoria VARCHAR(150) = NULL,
	@UsuarioIdRegistrador INT = NULL,
	@ID_Comunidad INT = NULL
AS
BEGIN
	UPDATE Experiencia
	SET Titulo = COALESCE(@Titulo, Titulo),
		Descripcion = COALESCE(@Descripcion, Descripcion),
		Categoria = COALESCE(@Categoria, Categoria),
		UsuarioIdRegistrador = COALESCE(@UsuarioIdRegistrador, UsuarioIdRegistrador),
		ID_Comunidad = COALESCE(@ID_Comunidad, ID_Comunidad)
	WHERE ID_Experiencia = @ID_Experiencia;
END;
GO

-- Eliminar experiencia
CREATE PROCEDURE SP_EliminarExperiencia
	@ID_Experiencia INT
AS
BEGIN
	DELETE FROM Experiencia
	WHERE ID_Experiencia = @ID_Experiencia;
END;
GO

-- Consultar experiencias con concurrencia
CREATE PROCEDURE SP_ConsultarExperienciasConcurrencia
AS
BEGIN
	SELECT ID_Concurrencia, Fecha, Detalle, Precio, Cupos_Disponibles, ID_Experiencia
	FROM ExperienciaConcurrencia;
END;
GO

-- Consultar experiencia concurrencia por ID
CREATE PROCEDURE SP_ConsultarExperienciaConcurrenciaPorId
	@ID_Concurrencia INT
AS
BEGIN
	SELECT ID_Concurrencia, Fecha, Detalle, Precio, Cupos_Disponibles, ID_Experiencia
	FROM ExperienciaConcurrencia
	WHERE ID_Concurrencia = @ID_Concurrencia;
END;
GO

-- Insertar experiencia concurrencia
CREATE PROCEDURE SP_InsertarExperienciaConcurrencia
	@Fecha DATE,
	@Detalle NVARCHAR(255),
	@Precio DECIMAL,
	@Cupos_Disponibles INT,
	@ID_Experiencia INT
AS
BEGIN
	INSERT INTO ExperienciaConcurrencia (Fecha, Detalle, Precio, Cupos_Disponibles, ID_Experiencia)
	VALUES (@Fecha, @Detalle, @Precio, @Cupos_Disponibles, @ID_Experiencia);
END;
GO

-- Actualizar experiencia concurrencia
CREATE PROCEDURE SP_ActualizarExperienciaConcurrencia
	@ID_Concurrencia INT,
	@Fecha DATE = NULL,
	@Detalle NVARCHAR(255) = NULL,
	@Precio DECIMAL = NULL,
	@Cupos_Disponibles INT = NULL,
	@ID_Experiencia INT = NULL
AS
BEGIN
	UPDATE ExperienciaConcurrencia
	SET Fecha = COALESCE(@Fecha, Fecha),
		Detalle = COALESCE(@Detalle, Detalle),
		Precio = COALESCE(@Precio, Precio),
		Cupos_Disponibles = COALESCE(@Cupos_Disponibles, Cupos_Disponibles),
		ID_Experiencia = COALESCE(@ID_Experiencia, ID_Experiencia)
	WHERE ID_Concurrencia = @ID_Concurrencia;
END;
GO

-- Eliminar experiencia concurrencia
CREATE PROCEDURE SP_EliminarExperienciaConcurrencia
	@ID_Concurrencia INT
AS
BEGIN
	DELETE FROM ExperienciaConcurrencia
	WHERE ID_Concurrencia = @ID_Concurrencia;
END;
GO
GO

-- Insertar roles iniciales
IF NOT EXISTS (SELECT 1 FROM Rol WHERE Rol_Nombre = 'Administrador')
BEGIN
	INSERT INTO Rol (Rol_Nombre) VALUES ('Administrador');
END;

IF NOT EXISTS (SELECT 1 FROM Rol WHERE Rol_Nombre = 'Usuario')
BEGIN
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
*/