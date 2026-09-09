-- =============================================
-- DATOS DE PRUEBA - CNFL_Clientes
-- PROTOTIPO TCU - SAMUEL SANDOVAL
-- =============================================

USE CNFL_Clientes;
GO

-- Insertar más usuarios de prueba
INSERT INTO Usuarios (Cedula, Nombre, Apellidos, Correo, Telefono, Contraseña) VALUES
('1-1234-5678', 'María', 'González Pérez', 'maria.g@email.com', '8888-1234', 'cliente123'),
('3-8765-4321', 'Carlos', 'Rodríguez Mora', 'carlos.r@email.com', '7777-5678', 'cliente123');
GO

-- Insertar NISEs de prueba
INSERT INTO NISEs (UsuarioId, NumeroNise, Direccion, Provincia, Canton, Distrito, TipoServicio) VALUES
(1, 'NISE-001', 'San José, Barrio Los Ángeles', 'San José', 'San José', 'Hospital', 'Residencial'),
(2, 'NISE-002', 'Heredia, Santo Domingo', 'Heredia', 'Santo Domingo', 'San Miguel', 'Residencial'),
(3, 'NISE-003', 'Alajuela, Central', 'Alajuela', 'Alajuela', 'Central', 'Comercial');
GO

-- Insertar facturas de prueba
INSERT INTO Facturas (NiseId, NumeroFactura, Monto, FechaEmision, FechaVencimiento, Pagada, Descripcion) VALUES
(1, 'F-2026-001', 25000, DATEADD(day, -10, GETDATE()), DATEADD(day, 10, GETDATE()), 0, 'Factura del mes de agosto'),
(2, 'F-2026-002', 18000, DATEADD(month, -1, GETDATE()), DATEADD(day, -10, GETDATE()), 1, 'Factura del mes de julio'),
(3, 'F-2026-003', 32000, DATEADD(day, -5, GETDATE()), DATEADD(day, 15, GETDATE()), 0, 'Factura del mes de agosto');
GO