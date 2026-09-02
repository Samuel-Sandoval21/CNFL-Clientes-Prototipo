-- ==========================================================
-- BASE DE DATOS: CNFL_App
-- ==========================================================
-- ELIMINAR BASE DE DATOS SI EXISTE
-- ==========================================================
USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'CNFL_App')
BEGIN
    ALTER DATABASE CNFL_App SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE CNFL_App;
    PRINT '✅ Base de datos CNFL_App eliminada';
END
GO

-- ==========================================================
-- CREAR BASE DE DATOS
-- ==========================================================
CREATE DATABASE CNFL_App;
PRINT '✅ Base de datos CNFL_App creada';
GO

USE CNFL_App;
PRINT '✅ Usando base de datos CNFL_App';
GO

-- ==========================================================
-- PASO 1: CREAR TABLA Roles (PRIMERO)
-- ==========================================================
PRINT '📌 Creando tabla: Roles...';
CREATE TABLE Roles (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL UNIQUE,
    Descripcion NVARCHAR(200)
);
GO

-- ==========================================================
-- PASO 2: CREAR TABLA Usuarios (DESPUÉS de Roles)
-- ==========================================================
PRINT '📌 Creando tabla: Usuarios...';
CREATE TABLE Usuarios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Apellidos NVARCHAR(100) NOT NULL,
    Cedula NVARCHAR(20) NOT NULL UNIQUE,
    Telefono NVARCHAR(20) NOT NULL,
    TelefonoSecundario NVARCHAR(20) NULL,
    Correo NVARCHAR(100) NOT NULL UNIQUE,
    CorreoSecundario NVARCHAR(100) NULL,
    Sexo NVARCHAR(20) NULL,
    SexoPersonalizado NVARCHAR(50) NULL,
    Direccion NVARCHAR(200) NULL,
    NISE NVARCHAR(20) NOT NULL,
    UserName NVARCHAR(50) NOT NULL UNIQUE,
    Contraseña NVARCHAR(100) NOT NULL,
    RolId INT NOT NULL DEFAULT 1,
    FechaNacimiento DATE NOT NULL,
    FechaRegistro DATETIME DEFAULT GETDATE(),
    Activo BIT DEFAULT 1,
    AceptaPolitica BIT DEFAULT 0,
    AceptaConsentimiento BIT DEFAULT 0,
    CONSTRAINT FK_Usuarios_Roles FOREIGN KEY (RolId) REFERENCES Roles(Id)
);
GO

-- ==========================================================
-- PASO 3: CREAR TABLA Clientes
-- ==========================================================
PRINT '📌 Creando tabla: Clientes...';
CREATE TABLE Clientes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId INT NOT NULL,
    Direccion NVARCHAR(200),
    Provincia NVARCHAR(50),
    Canton NVARCHAR(50),
    Distrito NVARCHAR(50),
    CONSTRAINT FK_Clientes_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id)
);
GO

-- ==========================================================
-- PASO 4: CREAR TABLA NISES
-- ==========================================================
PRINT '📌 Creando tabla: NISES...';
CREATE TABLE NISES (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ClienteId INT NOT NULL,
    Numero NVARCHAR(20) NOT NULL,
    Direccion NVARCHAR(200),
    Activo BIT DEFAULT 1,
    CONSTRAINT FK_NISES_Clientes FOREIGN KEY (ClienteId) REFERENCES Clientes(Id)
);
GO

-- ==========================================================
-- PASO 5: CREAR TABLA Facturas
-- ==========================================================
PRINT '📌 Creando tabla: Facturas...';
CREATE TABLE Facturas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NISEId INT NOT NULL,
    NumeroFactura NVARCHAR(50) NOT NULL UNIQUE,
    Periodo NVARCHAR(20) NOT NULL,
    FechaEmision DATE NOT NULL,
    FechaVencimiento DATE NOT NULL,
    Monto DECIMAL(18,2) NOT NULL,
    Saldo DECIMAL(18,2) NOT NULL,
    Estado NVARCHAR(20) DEFAULT 'Pendiente',
    CONSTRAINT FK_Facturas_NISES FOREIGN KEY (NISEId) REFERENCES NISES(Id)
);
GO

-- ==========================================================
-- PASO 6: CREAR TABLA Pagos
-- ==========================================================
PRINT '📌 Creando tabla: Pagos...';
CREATE TABLE Pagos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FacturaId INT NOT NULL,
    UsuarioId INT NOT NULL,
    MetodoPago NVARCHAR(20) NOT NULL,
    Monto DECIMAL(18,2) NOT NULL,
    FechaPago DATETIME DEFAULT GETDATE(),
    Transaccion NVARCHAR(50),
    Autorizacion NVARCHAR(50),
    Estado NVARCHAR(20) DEFAULT 'Completado',
    CONSTRAINT FK_Pagos_Facturas FOREIGN KEY (FacturaId) REFERENCES Facturas(Id),
    CONSTRAINT FK_Pagos_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id)
);
GO

-- ==========================================================
-- PASO 7: CREAR TABLA Averias
-- ==========================================================
PRINT '📌 Creando tabla: Averias...';
CREATE TABLE Averias (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId INT NOT NULL,
    NISEId INT NOT NULL,
    TipoAveria NVARCHAR(50) NOT NULL,
    Descripcion NVARCHAR(500),
    Direccion NVARCHAR(200),
    Latitud DECIMAL(10,8),
    Longitud DECIMAL(11,8),
    Estado NVARCHAR(20) DEFAULT 'Pendiente',
    FechaReporte DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Averias_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id),
    CONSTRAINT FK_Averias_NISES FOREIGN KEY (NISEId) REFERENCES NISES(Id)
);
GO

-- ==========================================================
-- PASO 8: CREAR TABLA Notificaciones
-- ==========================================================
PRINT '📌 Creando tabla: Notificaciones...';
CREATE TABLE Notificaciones (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId INT NOT NULL,
    Titulo NVARCHAR(200) NOT NULL,
    Mensaje NVARCHAR(500) NOT NULL,
    Tipo NVARCHAR(50),
    Leida BIT DEFAULT 0,
    FechaEnvio DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Notificaciones_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id)
);
GO

-- ==========================================================
-- PASO 9: CREAR TABLA Tramites
-- ==========================================================
PRINT '📌 Creando tabla: Tramites...';
CREATE TABLE Tramites (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId INT NOT NULL,
    NISEId INT NOT NULL,
    TipoTramite NVARCHAR(50) NOT NULL,
    Estado NVARCHAR(20) DEFAULT 'En Proceso',
    Detalle NVARCHAR(500),
    FechaSolicitud DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Tramites_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id),
    CONSTRAINT FK_Tramites_NISES FOREIGN KEY (NISEId) REFERENCES NISES(Id)
);
GO

-- ==========================================================
-- PASO 10: CREAR TABLA Suscripciones
-- ==========================================================
PRINT '📌 Creando tabla: Suscripciones...';
CREATE TABLE Suscripciones (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId INT NOT NULL,
    Servicio NVARCHAR(100) NOT NULL,
    Estado NVARCHAR(20) DEFAULT 'Activa',
    FechaInicio DATE NOT NULL,
    FechaFin DATE,
    CONSTRAINT FK_Suscripciones_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id)
);
GO

PRINT '✅ Todas las tablas creadas exitosamente!';
GO

-- ==========================================================
-- INSERTS INICIALES
-- ==========================================================

-- Insertar Roles
PRINT '📌 Insertando Roles...';
INSERT INTO Roles (Nombre, Descripcion) VALUES 
('Cliente', 'Usuario cliente de la CNFL'),
('Admin', 'Administrador del sistema');
GO

-- Insertar Usuarios
PRINT '📌 Insertando Usuarios...';
INSERT INTO Usuarios (
    Nombre, Apellidos, Cedula, Telefono, TelefonoSecundario, 
    Correo, CorreoSecundario, Sexo, SexoPersonalizado, Direccion,
    NISE, UserName, Contraseña, RolId, FechaNacimiento,
    AceptaPolitica, AceptaConsentimiento, FechaRegistro, Activo
)
VALUES 
(
    'Admin', 'Sistema', '1-0000-0000', '0000-0000', NULL,
    'admin@cnfl.go.cr', NULL, 'Masculino', NULL, 'San José, San José, Catedral',
    '000000000', 'admin', '123456', 2, '1990-01-01',
    1, 1, GETDATE(), 1
),
(
    'Katherine', 'Villalobos', '1-2345-6789', '8888-7777', '8888-1111',
    'k.villalobos@correo.cr', 'k.villalobos2@correo.cr', 'Femenino', NULL, 'San José, San José, Catedral',
    '402112345', 'cliente', '123456', 1, '1990-05-15',
    1, 1, GETDATE(), 1
),
(
    'Samuel', 'Sandoval Ramírez', '2-0874-0716', '8888-8888', NULL,
    'samuel@correo.cr', NULL, 'Masculino', NULL, 'San José, San José, Catedral',
    '402198765', 'samuel', '123456', 1, '2001-02-21',
    1, 1, GETDATE(), 1
);
GO

-- Insertar Clientes
PRINT '📌 Insertando Clientes...';
INSERT INTO Clientes (UsuarioId, Direccion, Provincia, Canton, Distrito)
SELECT Id, 'San José Centro', 'San José', 'San José', 'Catedral'
FROM Usuarios WHERE RolId = 1;
GO

-- Insertar NISES
PRINT '📌 Insertando NISES...';
INSERT INTO NISES (ClienteId, Numero, Direccion)
SELECT 
    c.Id, 
    u.NISE, 
    'Casa Principal'
FROM Clientes c
INNER JOIN Usuarios u ON c.UsuarioId = u.Id;
GO

-- Insertar Facturas
PRINT '📌 Insertando Facturas...';
INSERT INTO Facturas (NISEId, NumeroFactura, Periodo, FechaEmision, FechaVencimiento, Monto, Saldo, Estado)
SELECT 
    n.Id,
    'FACT-' + CAST(YEAR(GETDATE()) AS VARCHAR) + '-' + CAST(n.Id AS VARCHAR) + '-' + CAST(ROW_NUMBER() OVER (ORDER BY n.Id) AS VARCHAR),
    DATENAME(MONTH, GETDATE()) + ' ' + CAST(YEAR(GETDATE()) AS VARCHAR),
    DATEADD(DAY, -30, GETDATE()),
    GETDATE(),
    18450 + (n.Id * 500),
    18450 + (n.Id * 500),
    'Pendiente'
FROM NISES n;
GO

-- Insertar Notificaciones
PRINT '📌 Insertando Notificaciones...';
INSERT INTO Notificaciones (UsuarioId, Titulo, Mensaje, Tipo)
SELECT 
    u.Id,
    'Bienvenido a CNFL App',
    'Gracias por ser parte de la CNFL. Gestiona tu servicio desde tu celular.',
    'Bienvenida'
FROM Usuarios u;
GO
