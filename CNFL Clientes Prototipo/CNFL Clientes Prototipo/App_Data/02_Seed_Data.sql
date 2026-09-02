-- Ver todas las tablas
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';

-- Ver roles
SELECT * FROM Roles;

-- Ver usuarios
SELECT * FROM Usuarios;

-- Ver clientes
SELECT * FROM Clientes;

-- Ver NISES
SELECT * FROM NISES;

-- Ver facturas
SELECT * FROM Facturas;

-- Ver notificaciones
SELECT * FROM Notificaciones;

USE CNFL_App;
GO

-- Ver total de actividades
SELECT COUNT(*) AS TotalActividades FROM ActividadesEconomicas;
GO

-- Ver algunas actividades
SELECT TOP 20 Codigo, Nombre FROM ActividadesEconomicas ORDER BY Codigo;
GO


USE CNFL_App;
GO

-- Verificar que la tabla existe
SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ActividadesEconomicas';

-- Verificar datos
SELECT COUNT(*) AS Total FROM ActividadesEconomicas;

-- Verificar columnas
SELECT COLUMN_NAME, DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'ActividadesEconomicas';


USE CNFL_App;
GO

-- Verificar que la tabla existe y tiene datos
SELECT COUNT(*) AS Total FROM ActividadesEconomicas;
GO

-- Ver algunos registros
SELECT TOP 5 Codigo, Nombre FROM ActividadesEconomicas;
GO

SELECT 
    Id,
    Nombre,
    Apellidos,
    Cedula,
    Telefono,
    Correo,
    NISE,
    UserName,
    FechaRegistro,
    Activo
FROM Usuarios
WHERE RolId = 1
ORDER BY FechaRegistro DESC;

