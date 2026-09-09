-- =============================================
-- ACTUALIZACIÓN DE BASE DE DATOS - CNFL_Clientes
-- PROTOTIPO TCU - SAMUEL SANDOVAL
-- =============================================
-- Script: 04_Actualizar_Actividades_Direccion.sql
-- Fecha: 09/09/2026
-- Descripción: Agrega tabla ActividadesEconomicas y columna Direccion en Averias
-- =============================================

USE CNFL_Clientes;
GO

-- =============================================
-- 1. AGREGAR COLUMNA Direccion EN Averias
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Direccion' AND object_id = OBJECT_ID('Averias'))
BEGIN
    ALTER TABLE Averias ADD Direccion NVARCHAR(200) NULL;
    PRINT '✅ Columna Direccion agregada a Averias';
END
ELSE
BEGIN
    PRINT '⚠️ La columna Direccion ya existe en Averias';
END
GO

-- =============================================
-- 2. CREAR TABLA ActividadesEconomicas
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ActividadesEconomicas')
BEGIN
    CREATE TABLE ActividadesEconomicas (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Codigo NVARCHAR(10) NOT NULL UNIQUE,
        Nombre NVARCHAR(200) NOT NULL,
        Descripcion NVARCHAR(500)
    );
    PRINT '✅ Tabla ActividadesEconomicas creada';
END
ELSE
BEGIN
    PRINT '⚠️ La tabla ActividadesEconomicas ya existe';
END
GO

-- =============================================
-- 3. INSERTAR DATOS DE EJEMPLO EN ActividadesEconomicas
-- =============================================

IF NOT EXISTS (SELECT * FROM ActividadesEconomicas)
BEGIN
    INSERT INTO ActividadesEconomicas (Codigo, Nombre, Descripcion) VALUES
    ('6201', 'Servicios de programación informática', 'Desarrollo de software y aplicaciones'),
    ('6202', 'Consultoría informática', 'Asesoría en tecnologías de la información'),
    ('6209', 'Otros servicios de TI', 'Servicios de TI no clasificados'),
    ('7020', 'Consultoría de gestión', 'Asesoría en gestión empresarial'),
    ('9609', 'Otros servicios personales', 'Servicios personales no clasificados'),
    ('4610', 'Intermediación de combustible', 'Venta de combustible y derivados'),
    ('4711', 'Venta al por menor', 'Supermercados y similares'),
    ('5610', 'Restaurantes y servicios de comida', 'Preparación de alimentos y bebidas');
    PRINT '✅ Datos insertados en ActividadesEconomicas';
END
ELSE
BEGIN
    PRINT '⚠️ ActividadesEconomicas ya tiene datos';
END
GO

-- =============================================
-- 4. VERIFICAR LOS CAMBIOS
-- =============================================

-- Verificar columna Direccion en Averias
SELECT COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Averias' AND COLUMN_NAME = 'Direccion';
PRINT '';

-- Verificar tabla ActividadesEconomicas
SELECT * FROM ActividadesEconomicas;
PRINT '';

-- Verificar estructura de Averias
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Averias'
ORDER BY ORDINAL_POSITION;
GO

-- =============================================
-- FIN DEL SCRIPT
-- =============================================
PRINT '✅ Actualización completada exitosamente';
GO