-- =============================================
-- AJUSTES PROTOTIPO CNFL - PARTE 1
-- =============================================
-- 1. Agregar FacturaElectronica a Usuarios
-- 2. Agregar Tipo y Estado a Notificaciones
-- 3. Insertar datos de prueba para Alertas
-- =============================================

USE CNFL_Clientes;
GO

-- ═══════════════════════════════════════════════════════════
-- 1. FACTURA ELECTRÓNICA EN USUARIOS
-- ═══════════════════════════════════════════════════════════
IF COL_LENGTH('Usuarios', 'FacturaElectronica') IS NULL
BEGIN
    ALTER TABLE Usuarios ADD FacturaElectronica BIT NULL DEFAULT 0;
    PRINT '✅ Columna FacturaElectronica agregada a Usuarios.';
END
ELSE
BEGIN
    PRINT '⚠️ La columna FacturaElectronica ya existe.';
END
GO

-- ═══════════════════════════════════════════════════════════
-- 2. TIPO Y ESTADO EN NOTIFICACIONES
-- ═══════════════════════════════════════════════════════════
IF COL_LENGTH('Notificaciones', 'Tipo') IS NULL
BEGIN
    ALTER TABLE Notificaciones ADD Tipo NVARCHAR(30) NULL;
    PRINT '✅ Columna Tipo agregada a Notificaciones.';
END
ELSE
BEGIN
    PRINT '⚠️ La columna Tipo ya existe en Notificaciones.';
END
GO

IF COL_LENGTH('Notificaciones', 'Estado') IS NULL
BEGIN
    ALTER TABLE Notificaciones ADD Estado NVARCHAR(30) NULL;
    PRINT '✅ Columna Estado agregada a Notificaciones.';
END
ELSE
BEGIN
    PRINT '⚠️ La columna Estado ya existe en Notificaciones.';
END
GO

-- ═══════════════════════════════════════════════════════════
-- 3. DATOS DE PRUEBA PARA ALERTAS (4 tipos)
-- ═══════════════════════════════════════════════════════════
-- Tipos: Averia | Suspension | Pago | Evento
-- Estados: Reportada | EnProceso | Finalizada
--          Programada | EnProceso
--          PorVencer | Vencida
--          VoltajeFueraRango | ConsumoFueraLimite

-- Solo insertar si el usuario 2 (Samuel) no tiene alertas aún
IF NOT EXISTS (SELECT 1 FROM Notificaciones WHERE UsuarioId = 2 AND Tipo IS NOT NULL)
BEGIN
    -- Avería reportada
    INSERT INTO Notificaciones (UsuarioId, Titulo, Mensaje, Fecha, Leida, Tipo, Estado)
    VALUES (2, 'Avería reportada · NISE 4021',
            'Poste caído en tu sector. Operador asignado.',
            DATEADD(hour, -2, GETDATE()), 0, 'Averia', 'Reportada');

    -- Avería en proceso
    INSERT INTO Notificaciones (UsuarioId, Titulo, Mensaje, Fecha, Leida, Tipo, Estado)
    VALUES (2, 'Avería en proceso · NISE 9950',
            'Cuadrilla en camino. Tiempo estimado: 4:30 pm.',
            DATEADD(hour, -5, GETDATE()), 0, 'Averia', 'EnProceso');

    -- Avería finalizada
    INSERT INTO Notificaciones (UsuarioId, Titulo, Mensaje, Fecha, Leida, Tipo, Estado)
    VALUES (2, 'Avería finalizada · NISE 4021',
            'Servicio restablecido correctamente.',
            DATEADD(day, -1, GETDATE()), 1, 'Averia', 'Finalizada');

    -- Suspensión programada
    INSERT INTO Notificaciones (UsuarioId, Titulo, Mensaje, Fecha, Leida, Tipo, Estado)
    VALUES (2, 'Suspensión programada · NISE 7788',
            'Mantenimiento de red mañana de 8:00 a 10:00 am.',
            DATEADD(hour, -8, GETDATE()), 0, 'Suspension', 'Programada');

    -- Suspensión en proceso
    INSERT INTO Notificaciones (UsuarioId, Titulo, Mensaje, Fecha, Leida, Tipo, Estado)
    VALUES (2, 'Suspensión en proceso · NISE 7788',
            'Trabajos de mantenimiento en curso.',
            DATEADD(hour, -1, GETDATE()), 0, 'Suspension', 'EnProceso');

    -- Factura por vencer
    INSERT INTO Notificaciones (UsuarioId, Titulo, Mensaje, Fecha, Leida, Tipo, Estado)
    VALUES (2, 'Factura por vencer',
            '₡18.450 vence el 28 de agosto. Evitá el corte.',
            DATEADD(day, -1, GETDATE()), 0, 'Pago', 'PorVencer');

    -- Evento de voltaje
    INSERT INTO Notificaciones (UsuarioId, Titulo, Mensaje, Fecha, Leida, Tipo, Estado)
    VALUES (2, 'Evento de voltaje · NISE 4021',
            'Voltaje fuera de rango a las 14:23. Ya fue registrado.',
            DATEADD(hour, -3, GETDATE()), 0, 'Evento', 'VoltajeFueraRango');

    -- Evento de consumo
    INSERT INTO Notificaciones (UsuarioId, Titulo, Mensaje, Fecha, Leida, Tipo, Estado)
    VALUES (2, 'Consumo fuera de límite · NISE 4021',
            'Tu consumo de este mes superó el 150% del promedio.',
            DATEADD(hour, -12, GETDATE()), 0, 'Evento', 'ConsumoFueraLimite');

    PRINT '✅ 8 alertas de prueba insertadas para el usuario 2 (Samuel).';
END
ELSE
BEGIN
    PRINT '⚠️ El usuario 2 ya tiene alertas. No se insertó nada.';
END
GO

-- ═══════════════════════════════════════════════════════════
-- VERIFICACIÓN
-- ═══════════════════════════════════════════════════════════

PRINT '📊 Columnas de Usuarios (FacturaElectronica):';
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Usuarios' AND COLUMN_NAME = 'FacturaElectronica';
GO

PRINT '📊 Columnas de Notificaciones (Tipo, Estado):';
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Notificaciones' AND COLUMN_NAME IN ('Tipo', 'Estado');
GO

PRINT '📊 Alertas por tipo del usuario 2:';
SELECT Tipo, Estado, COUNT(*) AS Cantidad
FROM Notificaciones
WHERE UsuarioId = 2 AND Tipo IS NOT NULL
GROUP BY Tipo, Estado
ORDER BY Tipo, Estado;
GO

PRINT '✅ Script completado exitosamente.';
GO