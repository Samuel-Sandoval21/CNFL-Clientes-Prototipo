USE CNFL_Clientes;
GO

-- =============================================
-- CREAR TABLA Tramites (si no existe con los campos nuevos)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Tramites')
BEGIN
    CREATE TABLE Tramites (
        TramiteId INT IDENTITY(1,1) PRIMARY KEY,
        UsuarioId INT NOT NULL FOREIGN KEY REFERENCES Usuarios(UsuarioId),
        Tipo NVARCHAR(100) NOT NULL,
        Categoria NVARCHAR(50) NOT NULL,
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
    -- Agregar columnas nuevas si la tabla ya existía
    IF COL_LENGTH('Tramites', 'Categoria') IS NULL
        ALTER TABLE Tramites ADD Categoria NVARCHAR(50) NULL;

    IF COL_LENGTH('Tramites', 'FechaActualizacion') IS NULL
        ALTER TABLE Tramites ADD FechaActualizacion DATETIME NULL;

    IF COL_LENGTH('Tramites', 'NumeroReferencia') IS NULL
        ALTER TABLE Tramites ADD NumeroReferencia NVARCHAR(30) NULL;

    IF COL_LENGTH('Tramites', 'DatosFormulario') IS NULL
        ALTER TABLE Tramites ADD DatosFormulario NVARCHAR(MAX) NULL;

    PRINT '✅ Columnas agregadas a Tramites';
END
GO