USE CNFL_Clientes;
GO

-- =============================================
-- TABLA: CarritoItems
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CarritoItems')
BEGIN
    CREATE TABLE CarritoItems (
        CarritoItemId INT IDENTITY(1,1) PRIMARY KEY,
        UsuarioId INT NOT NULL FOREIGN KEY REFERENCES Usuarios(UsuarioId),
        ProductoId NVARCHAR(50) NOT NULL,
        Nombre NVARCHAR(200) NOT NULL,
        Descripcion NVARCHAR(500) NULL,
        Precio DECIMAL(12,2) NOT NULL,
        Cantidad INT NOT NULL DEFAULT 1,
        Imagen NVARCHAR(50) NULL,
        FechaAgregado DATETIME NOT NULL DEFAULT GETDATE()
    );
    CREATE INDEX IX_Carrito_Usuario ON CarritoItems(UsuarioId);
    PRINT '✅ Tabla CarritoItems creada';
END
ELSE PRINT '⚠️ CarritoItems ya existe';
GO

-- =============================================
-- TABLA: OrdenesCompra
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrdenesCompra')
BEGIN
    CREATE TABLE OrdenesCompra (
        OrdenId INT IDENTITY(1,1) PRIMARY KEY,
        NumeroOrden NVARCHAR(30) UNIQUE NOT NULL,
        UsuarioId INT NOT NULL FOREIGN KEY REFERENCES Usuarios(UsuarioId),
        Subtotal DECIMAL(12,2) NOT NULL,
        Impuesto DECIMAL(12,2) NOT NULL,
        Total DECIMAL(12,2) NOT NULL,
        Metodo NVARCHAR(30) NOT NULL,
        Estado NVARCHAR(20) NOT NULL DEFAULT 'Pendiente',
        ReferenciaPago NVARCHAR(100) NULL,
        FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
        FechaConfirmacion DATETIME NULL,
        Detalle NVARCHAR(MAX) NULL
    );
    CREATE INDEX IX_Orden_Usuario ON OrdenesCompra(UsuarioId);
    PRINT '✅ Tabla OrdenesCompra creada';
END
ELSE PRINT '⚠️ OrdenesCompra ya existe';
GO

-- =============================================
-- TABLA: MetodosPago
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MetodosPago')
BEGIN
    CREATE TABLE MetodosPago (
        MetodoPagoId INT IDENTITY(1,1) PRIMARY KEY,
        UsuarioId INT NOT NULL FOREIGN KEY REFERENCES Usuarios(UsuarioId),
        Tipo NVARCHAR(30) NOT NULL,
        Alias NVARCHAR(100) NULL,
        Ultimos4 NVARCHAR(4) NULL,
        Titular NVARCHAR(100) NULL,
        FechaVencimiento NVARCHAR(10) NULL,
        Banco NVARCHAR(50) NULL,
        CuentaIBAN NVARCHAR(50) NULL,
        Predeterminado BIT DEFAULT 0,
        Activo BIT DEFAULT 1,
        FechaRegistro DATETIME NOT NULL DEFAULT GETDATE()
    );
    CREATE INDEX IX_Metodos_Usuario ON MetodosPago(UsuarioId);
    PRINT '✅ Tabla MetodosPago creada';
END
ELSE PRINT '⚠️ MetodosPago ya existe';
GO

PRINT '✅ Todas las tablas verificadas';
GO