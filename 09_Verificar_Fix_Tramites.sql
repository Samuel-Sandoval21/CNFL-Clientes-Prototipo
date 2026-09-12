USE CNFL_Clientes;
GO

-- =============================================
-- VERIFICAR ESTRUCTURA DE Tramites
-- =============================================
PRINT '📊 Columnas actuales de la tabla Tramites:';
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Tramites'
ORDER BY ORDINAL_POSITION;
GO

-- =============================================
-- Si la tabla NO existe, crearla
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Tramites')
BEGIN
    CREATE TABLE Tramites (
        TramiteId INT IDENTITY(1,1) PRIMARY KEY,
        UsuarioId INT NOT NULL FOREIGN KEY REFERENCES Usuarios(UsuarioId),
        Tipo NVARCHAR(100) NOT NULL,
        Categoria NVARCHAR(50) NULL,
        Estado NVARCHAR(30) NOT NULL DEFAULT 'Solicitado',
        FechaSolicitud DATETIME NOT NULL DEFAULT GETDATE(),
        FechaActualizacion DATETIME NULL,
        Descripcion NVARCHAR(1000) NULL,
        NumeroReferencia NVARCHAR(30) NULL,
        DatosFormulario NVARCHAR(MAX) NULL
    );
    PRINT '✅ Tabla Tramites creada';
END
ELSE
BEGIN
    -- Agregar columnas nuevas si faltan
    IF COL_LENGTH('Tramites', 'Categoria') IS NULL
        ALTER TABLE Tramites ADD Categoria NVARCHAR(50) NULL;

    IF COL_LENGTH('Tramites', 'FechaActualizacion') IS NULL
        ALTER TABLE Tramites ADD FechaActualizacion DATETIME NULL;

    IF COL_LENGTH('Tramites', 'NumeroReferencia') IS NULL
        ALTER TABLE Tramites ADD NumeroReferencia NVARCHAR(30) NULL;

    IF COL_LENGTH('Tramites', 'DatosFormulario') IS NULL
        ALTER TABLE Tramites ADD DatosFormulario NVARCHAR(MAX) NULL;

    PRINT '✅ Columnas verificadas/agregadas en Tramites';
END
GO

-- =============================================
-- BORRAR COLUMNA ESPURIA NiseId SI EXISTE
-- (por si EF la creó mal en algún momento)
-- =============================================
IF COL_LENGTH('Tramites', 'NiseId') IS NOT NULL
BEGIN
    -- Primero borrar la FK si existe
    DECLARE @fkName NVARCHAR(200);
    SELECT @fkName = fk.name
    FROM sys.foreign_keys fk
    INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
    INNER JOIN sys.columns c ON fkc.parent_object_id = c.object_id AND fkc.parent_column_id = c.column_id
    WHERE fk.parent_object_id = OBJECT_ID('Tramites') AND c.name = 'NiseId';

    IF @fkName IS NOT NULL
    BEGIN
        EXEC('ALTER TABLE Tramites DROP CONSTRAINT ' + @fkName);
        PRINT '🗑️ FK de NiseId eliminada.';
    END

    ALTER TABLE Tramites DROP COLUMN NiseId;
    PRINT '🗑️ Columna NiseId eliminada de Tramites.';
END
ELSE
BEGIN
    PRINT '⚠️ La columna NiseId no existe en Tramites (correcto).';
END
GO

-- =============================================
-- VERIFICACIÓN FINAL
-- =============================================
PRINT '📊 Estructura final de Tramites:';
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Tramites'
ORDER BY ORDINAL_POSITION;
GO