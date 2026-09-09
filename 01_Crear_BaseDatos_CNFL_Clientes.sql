-- =============================================
-- BASE DE DATOS: CNFL_Clientes
-- PROTOTIPO TCU - SAMUEL SANDOVAL
-- =============================================

-- Crear la base de datos
CREATE DATABASE CNFL_Clientes;
GO

USE CNFL_Clientes;
GO

-- =============================================
-- TABLA: Usuarios
-- =============================================
CREATE TABLE Usuarios (
    UsuarioId INT IDENTITY(1,1) PRIMARY KEY,
    Cedula NVARCHAR(20) UNIQUE NOT NULL,
    Nombre NVARCHAR(50) NOT NULL,
    Apellidos NVARCHAR(50) NOT NULL,
    Correo NVARCHAR(100) NOT NULL,
    Telefono NVARCHAR(20) NOT NULL,
    Contraseña NVARCHAR(100) NOT NULL,
    FechaRegistro DATETIME DEFAULT GETDATE(),
    Activo BIT DEFAULT 1
);
GO

-- =============================================
-- TABLA: Roles
-- =============================================
CREATE TABLE Roles (
    RolId INT IDENTITY(1,1) PRIMARY KEY,
    NombreRol NVARCHAR(20) NOT NULL
);
GO

-- =============================================
-- TABLA: UsuarioRoles
-- =============================================
CREATE TABLE UsuarioRoles (
    UsuarioRolId INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId INT FOREIGN KEY REFERENCES Usuarios(UsuarioId),
    RolId INT FOREIGN KEY REFERENCES Roles(RolId)
);
GO

-- =============================================
-- TABLA: NISEs
-- =============================================
CREATE TABLE NISEs (
    NiseId INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId INT FOREIGN KEY REFERENCES Usuarios(UsuarioId),
    NumeroNise NVARCHAR(20) UNIQUE NOT NULL,
    Direccion NVARCHAR(200) NOT NULL,
    Provincia NVARCHAR(50) NOT NULL,
    Canton NVARCHAR(50) NOT NULL,
    Distrito NVARCHAR(50) NOT NULL,
    TipoServicio NVARCHAR(30) NOT NULL
);
GO

-- =============================================
-- TABLA: Facturas
-- =============================================
CREATE TABLE Facturas (
    FacturaId INT IDENTITY(1,1) PRIMARY KEY,
    NiseId INT FOREIGN KEY REFERENCES NISEs(NiseId),
    NumeroFactura NVARCHAR(20) UNIQUE NOT NULL,
    Monto DECIMAL(12,2) NOT NULL,
    FechaEmision DATETIME NOT NULL,
    FechaVencimiento DATETIME NOT NULL,
    Pagada BIT DEFAULT 0,
    Descripcion NVARCHAR(200)
);
GO

-- =============================================
-- TABLA: Averias
-- =============================================
CREATE TABLE Averias (
    AveriaId INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId INT FOREIGN KEY REFERENCES Usuarios(UsuarioId),
    NiseId INT FOREIGN KEY REFERENCES NISEs(NiseId),
    Tipo NVARCHAR(30) NOT NULL,
    Descripcion NVARCHAR(500) NOT NULL,
    Estado NVARCHAR(30) DEFAULT 'Ingresado',
    FechaReporte DATETIME DEFAULT GETDATE(),
    FechaActualizacion DATETIME,
    FotoUrl NVARCHAR(200)
);
GO

-- =============================================
-- TABLA: Suspensiones
-- =============================================
CREATE TABLE Suspensiones (
    SuspensionId INT IDENTITY(1,1) PRIMARY KEY,
    NiseId INT FOREIGN KEY REFERENCES NISEs(NiseId),
    FechaInicio DATETIME NOT NULL,
    FechaFin DATETIME NOT NULL,
    Motivo NVARCHAR(200) NOT NULL,
    Estado NVARCHAR(30) DEFAULT 'Programada'
);
GO

-- =============================================
-- TABLA: Notificaciones
-- =============================================
CREATE TABLE Notificaciones (
    NotificacionId INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId INT FOREIGN KEY REFERENCES Usuarios(UsuarioId),
    Titulo NVARCHAR(100) NOT NULL,
    Mensaje NVARCHAR(500) NOT NULL,
    Fecha DATETIME DEFAULT GETDATE(),
    Leida BIT DEFAULT 0,
    Tipo NVARCHAR(30)
);
GO

-- =============================================
-- TABLA: Tramites
-- =============================================
CREATE TABLE Tramites (
    TramiteId INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId INT FOREIGN KEY REFERENCES Usuarios(UsuarioId),
    Tipo NVARCHAR(50) NOT NULL,
    Estado NVARCHAR(30) DEFAULT 'Solicitado',
    FechaSolicitud DATETIME DEFAULT GETDATE(),
    Descripcion NVARCHAR(500)
);
GO

-- =============================================
-- TABLA: Suscripciones
-- =============================================
CREATE TABLE Suscripciones (
    SuscripcionId INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId INT FOREIGN KEY REFERENCES Usuarios(UsuarioId),
    Servicio NVARCHAR(50) NOT NULL,
    FechaInicio DATETIME DEFAULT GETDATE(),
    FechaFin DATETIME,
    Activa BIT DEFAULT 1,
    MontoMensual DECIMAL(10,2)
);
GO

-- =============================================
-- DATOS INICIALES
-- =============================================

-- Insertar Roles
INSERT INTO Roles (NombreRol) VALUES ('Cliente'), ('Admin');
GO

-- Insertar Usuario Admin
INSERT INTO Usuarios (Cedula, Nombre, Apellidos, Correo, Telefono, Contraseña)
VALUES ('1-1180-0989', 'Michael', 'Miranda Guevara', 'mimiranda@cnfl.go.cr', '2295-5795', 'admin123');
GO

-- Asignar rol Admin al usuario creado
INSERT INTO UsuarioRoles (UsuarioId, RolId) 
SELECT UsuarioId, (SELECT RolId FROM Roles WHERE NombreRol = 'Admin') 
FROM Usuarios WHERE Cedula = '1-1180-0989';
GO

-- Insertar un usuario cliente de ejemplo
INSERT INTO Usuarios (Cedula, Nombre, Apellidos, Correo, Telefono, Contraseña)
VALUES ('2-0874-0716', 'Samuel', 'Sandoval Ramírez', 'ssandoval40716@ufide.ac.cr', '8495-8927', 'cliente123');
GO

-- Asignar rol Cliente al usuario creado
INSERT INTO UsuarioRoles (UsuarioId, RolId) 
SELECT UsuarioId, (SELECT RolId FROM Roles WHERE NombreRol = 'Cliente') 
FROM Usuarios WHERE Cedula = '2-0874-0716';
GO