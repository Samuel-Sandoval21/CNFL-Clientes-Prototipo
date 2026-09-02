-- ==========================================================
-- ACTUALIZAR BASE DE DATOS: CNFL_App
-- ==========================================================

USE CNFL_App;
GO

-- ==========================================================
-- AGREGAR NUEVAS COLUMNAS A LA TABLA Usuarios
-- ==========================================================

-- 1. Factura Electrónica
ALTER TABLE Usuarios
ADD FacturaElectronica BIT NULL; -- 1 = Sí, 0 = No, NULL = No especificado

-- 2. Actividad Económica (Código)
ALTER TABLE Usuarios
ADD ActividadEconomicaCodigo NVARCHAR(10) NULL;

-- 3. Dirección Detallada (Provincia, Cantón, Distrito)
ALTER TABLE Usuarios
ADD Provincia NVARCHAR(50) NULL,
    Canton NVARCHAR(50) NULL,
    Distrito NVARCHAR(50) NULL;

PRINT '✅ Tabla Usuarios actualizada con nuevos campos.';
GO

-- ==========================================================
-- CREAR TABLA DE ACTIVIDADES ECONÓMICAS
-- ==========================================================

PRINT '📌 Creando tabla: ActividadesEconomicas...';
CREATE TABLE ActividadesEconomicas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Codigo NVARCHAR(10) NOT NULL UNIQUE,
    Nombre NVARCHAR(200) NOT NULL,
    Descripcion NVARCHAR(500) NULL
);
GO

-- ==========================================================
-- INSERTAR DATOS DE ACTIVIDADES ECONÓMICAS
-- ==========================================================

INSERT INTO ActividadesEconomicas (Codigo, Nombre, Descripcion) VALUES
('1391.0', 'Fabricación de tejidos de punto y ganchillo', 'N/A'),
('1512.0', 'Fabricación de maletas, bolsos de mano y de artículos similares', 'N/A'),
('1520.0', 'Fabricación de calzado', 'N/A'),
('2393.0', 'Fabricación de otros productos de cerámica y porcelana', 'N/A'),
('2511.0', 'Fabricación de productos metálicos para uso estructural', 'N/A'),
('3100.1', 'Fabricación de muebles de madera', 'N/A'),
('3212.0', 'Fabricación de bisutería y artículos conexos', 'N/A'),
('4719.9', 'Venta al por menor en bazares y otros establecimientos no especializados n.c.p', 'N/A'),
('4742.0', 'Venta al por menor de equipo de audio y video en comercios especializados', 'N/A'),
('4764.0', 'Venta al por menor de juegos y de juguetes en almacenes especializados', 'N/A'),
('4771.2', 'Venta al por menor de zapatos en comercios especializados', 'N/A'),
('5621.0', 'Suministros de comidas por encargo', 'N/A'),
('5630.0', 'Actividades de servicio de bebidas', 'N/A'),
('9523.0', 'Reparación de calzado y artículos de cuero', 'N/A'),
('1410.9', 'Fabricación de otras prendas de vestir, excepto ropa interior y prendas de piel', 'N/A');

PRINT '✅ Datos de Actividades Económicas insertados.';
GO