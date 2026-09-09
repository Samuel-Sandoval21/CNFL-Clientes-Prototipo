-- =============================================
-- CONSULTAS DE VERIFICACIÓN - CNFL_Clientes
-- PROTOTIPO TCU - SAMUEL SANDOVAL
-- =============================================

USE CNFL_Clientes;
GO

-- 1. Ver todos los usuarios
SELECT * FROM Usuarios;
GO

-- 2. Ver todos los roles
SELECT * FROM Roles;
GO

-- 3. Ver los roles asignados a los usuarios
SELECT u.Cedula, u.Nombre, u.Apellidos, r.NombreRol 
FROM Usuarios u
JOIN UsuarioRoles ur ON u.UsuarioId = ur.UsuarioId
JOIN Roles r ON ur.RolId = r.RolId;
GO

-- 4. Ver NISEs por usuario
SELECT u.Cedula, u.Nombre, u.Apellidos, n.NumeroNise, n.Direccion, n.Provincia
FROM Usuarios u
JOIN NISEs n ON u.UsuarioId = n.UsuarioId;
GO

-- 5. Ver facturas por NISE
SELECT n.NumeroNise, f.NumeroFactura, f.Monto, f.FechaEmision, f.FechaVencimiento, f.Pagada
FROM NISEs n
JOIN Facturas f ON n.NiseId = f.NiseId;
GO