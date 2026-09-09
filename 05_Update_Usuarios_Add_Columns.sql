USE CNFL_App;
GO

-- Añadir columnas opcionales al tabla Usuarios si no existen
IF COL_LENGTH('Usuarios', 'CorreoSecundario') IS NULL
BEGIN
	ALTER TABLE Usuarios ADD CorreoSecundario NVARCHAR(100) NULL;
	PRINT 'Columna CorreoSecundario añadida.';
END
ELSE
	PRINT 'Columna CorreoSecundario ya existe.';

IF COL_LENGTH('Usuarios', 'TelefonoSecundario') IS NULL
BEGIN
	ALTER TABLE Usuarios ADD TelefonoSecundario NVARCHAR(20) NULL;
	PRINT 'Columna TelefonoSecundario añadida.';
END
ELSE
	PRINT 'Columna TelefonoSecundario ya existe.';

IF COL_LENGTH('Usuarios', 'Sexo') IS NULL
BEGIN
	ALTER TABLE Usuarios ADD Sexo NVARCHAR(20) NULL;
	PRINT 'Columna Sexo añadida.';
END
ELSE
	PRINT 'Columna Sexo ya existe.';

GO

-- Opcional: comprobar esquema
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Usuarios'
ORDER BY ORDINAL_POSITION;
