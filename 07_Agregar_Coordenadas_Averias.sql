USE CNFL_Clientes;
GO

-- =============================================
-- 1. AGREGAR COLUMNAS Latitud y Longitud
-- =============================================
IF COL_LENGTH('Averias', 'Latitud') IS NULL
BEGIN
    ALTER TABLE Averias ADD Latitud FLOAT NULL;
    PRINT '✅ Columna Latitud agregada.';
END
ELSE PRINT '⚠️ Latitud ya existe.';
GO

IF COL_LENGTH('Averias', 'Longitud') IS NULL
BEGIN
    ALTER TABLE Averias ADD Longitud FLOAT NULL;
    PRINT '✅ Columna Longitud agregada.';
END
ELSE PRINT '⚠️ Longitud ya existe.';
GO

-- =============================================
-- 2. POBLAR COORDENADAS DE PRUEBA
-- =============================================
-- Nota: Usa UPDATE para no romper FKs. Puedes ajustar los IDs según tu data.
-- Coordenadas reales aproximadas en el GAM (Gran Área Metropolitana).

-- San José Centro
UPDATE Averias SET Latitud = 9.9325, Longitud = -84.0790
WHERE AveriaId = 1;

-- Alajuela
UPDATE Averias SET Latitud = 9.9980, Longitud = -84.2050
WHERE AveriaId = 2;

-- Cartago
UPDATE Averias SET Latitud = 9.8640, Longitud = -83.9200
WHERE AveriaId = 3;

-- Heredia
UPDATE Averias SET Latitud = 10.0170, Longitud = -84.1300
WHERE AveriaId = 4;

-- San José - Barrio Los Ángeles
UPDATE Averias SET Latitud = 9.9260, Longitud = -84.0680
WHERE AveriaId = 5;

-- Escazú
UPDATE Averias SET Latitud = 9.9180, Longitud = -84.1430
WHERE AveriaId = 6;

-- Si no hay suficientes averías, inserta algunas de prueba:
IF NOT EXISTS (SELECT 1 FROM Averias WHERE AveriaId > 6)
BEGIN
    INSERT INTO Averias (UsuarioId, NiseId, Tipo, Descripcion, Estado, FechaReporte, Latitud, Longitud)
    VALUES
    (1, 1, 'Poste caído', 'Poste en la acera frente al parque.', 'En revisión', GETDATE(), 9.9325, -84.0790),
    (2, 2, 'Transformador dañado', 'Ruido y chispas en el transformador.', 'Operador en camino', GETDATE(), 9.9980, -84.2050),
    (1, 1, 'Cable cortado', 'Cable de baja tensión caído.', 'Resuelto', GETDATE(), 9.8640, -83.9200);
    PRINT '✅ 3 averías de prueba insertadas.';
END
GO

-- =============================================
-- 3. VERIFICAR
-- =============================================
SELECT AveriaId, Tipo, Estado, Latitud, Longitud 
FROM Averias 
WHERE Latitud IS NOT NULL AND Longitud IS NOT NULL;
GO