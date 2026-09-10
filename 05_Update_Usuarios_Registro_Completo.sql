USE CNFL_Clientes;
GO

-- CorreoSecundario
IF COL_LENGTH('Usuarios', 'CorreoSecundario') IS NULL
BEGIN
    ALTER TABLE Usuarios ADD CorreoSecundario NVARCHAR(100) NULL;
    PRINT '✅ CorreoSecundario agregada.';
END
GO

-- TelefonoSecundario
IF COL_LENGTH('Usuarios', 'TelefonoSecundario') IS NULL
BEGIN
    ALTER TABLE Usuarios ADD TelefonoSecundario NVARCHAR(20) NULL;
    PRINT '✅ TelefonoSecundario agregada.';
END
GO

-- Sexo
IF COL_LENGTH('Usuarios', 'Sexo') IS NULL
BEGIN
    ALTER TABLE Usuarios ADD Sexo NVARCHAR(20) NULL;
    PRINT '✅ Sexo agregada.';
END
GO

-- Provincia
IF COL_LENGTH('Usuarios', 'Provincia') IS NULL
BEGIN
    ALTER TABLE Usuarios ADD Provincia NVARCHAR(50) NULL;
    PRINT '✅ Provincia agregada.';
END
GO

-- Canton
IF COL_LENGTH('Usuarios', 'Canton') IS NULL
BEGIN
    ALTER TABLE Usuarios ADD Canton NVARCHAR(50) NULL;
    PRINT '✅ Canton agregada.';
END
GO

-- Distrito
IF COL_LENGTH('Usuarios', 'Distrito') IS NULL
BEGIN
    ALTER TABLE Usuarios ADD Distrito NVARCHAR(50) NULL;
    PRINT '✅ Distrito agregada.';
END
GO

-- ActividadEconomicaId (FK)
IF COL_LENGTH('Usuarios', 'ActividadEconomicaId') IS NULL
BEGIN
    ALTER TABLE Usuarios ADD ActividadEconomicaId INT NULL;
    ALTER TABLE Usuarios ADD CONSTRAINT FK_Usuarios_ActividadesEconomicas
        FOREIGN KEY (ActividadEconomicaId) REFERENCES ActividadesEconomicas(Id);
    PRINT '✅ ActividadEconomicaId + FK agregada.';
END
GO

-- Verificación
SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Usuarios' ORDER BY ORDINAL_POSITION;
GO