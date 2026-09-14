USE CNFL_Clientes;
GO

-- =============================================
-- TABLA: ActividadUsuario (tracking de uso de la app)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ActividadUsuario')
BEGIN
    CREATE TABLE ActividadUsuario (
        ActividadId INT IDENTITY(1,1) PRIMARY KEY,
        UsuarioId INT NOT NULL FOREIGN KEY REFERENCES Usuarios(UsuarioId),
        Seccion NVARCHAR(50) NOT NULL,        -- Dashboard, Facturas, Reportes, etc
        Accion NVARCHAR(50) NOT NULL,         -- Vista, Descarga, Click, etc
        Detalle NVARCHAR(200) NULL,
        DuracionSegundos INT NULL,            -- tiempo en la sección
        Fecha DATETIME NOT NULL DEFAULT GETDATE()
    );
    CREATE INDEX IX_Actividad_Usuario ON ActividadUsuario(UsuarioId);
    CREATE INDEX IX_Actividad_Fecha ON ActividadUsuario(Fecha);
    PRINT '✅ Tabla ActividadUsuario creada';
END
ELSE PRINT '⚠️ ActividadUsuario ya existe';
GO

-- =============================================
-- TABLA: DescargasUsuario (archivos descargados)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DescargasUsuario')
BEGIN
    CREATE TABLE DescargasUsuario (
        DescargaId INT IDENTITY(1,1) PRIMARY KEY,
        UsuarioId INT NOT NULL FOREIGN KEY REFERENCES Usuarios(UsuarioId),
        Nombre NVARCHAR(200) NOT NULL,
        Tipo NVARCHAR(20) NOT NULL,           -- PDF, Excel, CSV
        Seccion NVARCHAR(50) NOT NULL,        -- Dashboard, Facturas, Reportes
        TamanoKB INT NULL,
        Fecha DATETIME NOT NULL DEFAULT GETDATE()
    );
    CREATE INDEX IX_Descargas_Usuario ON DescargasUsuario(UsuarioId);
    PRINT '✅ Tabla DescargasUsuario creada';
END
ELSE PRINT '⚠️ DescargasUsuario ya existe';
GO

-- =============================================
-- DATOS DE PRUEBA (opcional, para que se vean stats)
-- =============================================
IF NOT EXISTS (SELECT 1 FROM ActividadUsuario WHERE UsuarioId = 1)
BEGIN
    INSERT INTO ActividadUsuario (UsuarioId, Seccion, Accion, Detalle, DuracionSegundos, Fecha) VALUES
    (1, 'Dashboard', 'Vista', 'Acceso al inicio', 180, DATEADD(day, -6, GETDATE())),
    (1, 'Facturas', 'Vista', 'Revisó facturas pendientes', 240, DATEADD(day, -5, GETDATE())),
    (1, 'Reportes', 'Vista', 'Consultó mapa', 320, DATEADD(day, -4, GETDATE())),
    (1, 'Trámites', 'Vista', 'Inició un trámite', 450, DATEADD(day, -3, GETDATE())),
    (1, 'Tienda', 'Vista', 'Exploró productos', 380, DATEADD(day, -2, GETDATE())),
    (1, 'Dashboard', 'Vista', 'Revisó resumen', 210, DATEADD(day, -1, GETDATE())),
    (1, 'Perfil', 'Vista', 'Actualizó datos', 150, GETDATE());
    PRINT '✅ Datos de actividad de prueba insertados';
END
GO

SELECT 'Tablas creadas correctamente' AS Resultado;
GO