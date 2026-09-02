
-- ==========================================================
-- VERIFICACIÓN FINAL
-- ==========================================================
PRINT '✅ Todos los datos insertados exitosamente!';
GO

-- Ver todas las tablas
PRINT '📊 Tablas creadas:';
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';
GO

-- Ver roles
PRINT '📊 Roles:';
SELECT * FROM Roles;
GO

-- Ver usuarios
PRINT '📊 Usuarios:';
SELECT Id, Nombre, Apellidos, Cedula, Correo, NISE, UserName, RolId, FechaNacimiento, AceptaPolitica, AceptaConsentimiento FROM Usuarios;
GO

-- Ver clientes
PRINT '📊 Clientes:';
SELECT * FROM Clientes;
GO

-- Ver NISES
PRINT '📊 NISES:';
SELECT * FROM NISES;
GO

-- Ver facturas
PRINT '📊 Facturas:';
SELECT * FROM Facturas;
GO

-- Ver notificaciones
PRINT '📊 Notificaciones:';
SELECT * FROM Notificaciones;
GO

-- ==========================================================
-- CONSULTAS DE PRUEBA
-- ==========================================================
PRINT '📊 Usuarios con su rol:';
SELECT u.Id, u.Nombre, u.Apellidos, r.Nombre AS Rol
FROM Usuarios u
INNER JOIN Roles r ON u.RolId = r.Id;
GO

PRINT '📊 Clientes con sus NISES:';
SELECT c.Id AS ClienteId, u.Nombre, u.Apellidos, n.Numero AS NISE
FROM Clientes c
INNER JOIN Usuarios u ON c.UsuarioId = u.Id
INNER JOIN NISES n ON n.ClienteId = c.Id;
GO