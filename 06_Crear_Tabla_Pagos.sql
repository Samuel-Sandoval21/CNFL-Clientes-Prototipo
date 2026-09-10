USE CNFL_Clientes;
GO

-- =============================================
-- CREAR TABLA Pagos
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Pagos')
BEGIN
    CREATE TABLE Pagos (
        PagoId INT IDENTITY(1,1) PRIMARY KEY,
        FacturaId INT NOT NULL FOREIGN KEY REFERENCES Facturas(FacturaId),
        UsuarioId INT NOT NULL FOREIGN KEY REFERENCES Usuarios(UsuarioId),
        Monto DECIMAL(12,2) NOT NULL,
        Metodo NVARCHAR(30) NOT NULL,
        ReferenciaExterna NVARCHAR(100) NULL,
        Estado NVARCHAR(20) NOT NULL DEFAULT 'Pendiente',
        FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
        FechaConfirmacion DATETIME NULL,
        RawResponse NVARCHAR(MAX) NULL
    );
    CREATE INDEX IX_Pagos_Referencia ON Pagos(ReferenciaExterna);
    CREATE INDEX IX_Pagos_Usuario ON Pagos(UsuarioId);
    PRINT '✅ Tabla Pagos creada';
END
ELSE PRINT '⚠️ Tabla Pagos ya existe';
GO

-- =============================================
-- VERIFICAR
-- =============================================
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Pagos'
ORDER BY ORDINAL_POSITION;
GO