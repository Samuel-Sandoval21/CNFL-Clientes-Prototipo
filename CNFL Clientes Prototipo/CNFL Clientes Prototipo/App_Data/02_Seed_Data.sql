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