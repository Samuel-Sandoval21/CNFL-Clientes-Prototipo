using System.Data.Entity;
using CNFL_Clientes_Prototipo.Models;

namespace CNFL_Clientes_Prototipo.Data
{
    public class CNFLDbContext : DbContext
    {
        public CNFLDbContext() : base("name=CNFLDbContext")
        {
            Database.SetInitializer<CNFLDbContext>(null);
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<UsuarioRol> UsuarioRoles { get; set; }
        public DbSet<NISE> NISEs { get; set; }
        public DbSet<ActividadEconomica> ActividadesEconomicas { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<Averia> Averias { get; set; }
        public DbSet<Suspension> Suspensiones { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<Tramite> Tramites { get; set; }
        public DbSet<Suscripcion> Suscripciones { get; set; }
        public DbSet<Pago> Pagos { get; set; }

        public DbSet<CarritoItem> CarritoItems { get; set; }
        public DbSet<OrdenCompra> OrdenesCompra { get; set; }
        public DbSet<MetodoPago> MetodosPago { get; set; }

        public DbSet<ActividadUsuario> ActividadUsuario { get; set; }
        public DbSet<DescargaUsuario> DescargasUsuario { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== Nombres exactos de tablas =====
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Rol>().ToTable("Roles");
            modelBuilder.Entity<UsuarioRol>().ToTable("UsuarioRoles");
            modelBuilder.Entity<NISE>().ToTable("NISEs");
            modelBuilder.Entity<Factura>().ToTable("Facturas");
            modelBuilder.Entity<Averia>().ToTable("Averias");
            modelBuilder.Entity<Suspension>().ToTable("Suspensiones");
            modelBuilder.Entity<Notificacion>().ToTable("Notificaciones");
            modelBuilder.Entity<Tramite>().ToTable("Tramites");
            modelBuilder.Entity<Suscripcion>().ToTable("Suscripciones");
            modelBuilder.Entity<Pago>().ToTable("Pagos");
            modelBuilder.Entity<ActividadEconomica>().ToTable("ActividadesEconomicas");

            modelBuilder.Entity<CarritoItem>().ToTable("CarritoItems");
            modelBuilder.Entity<OrdenCompra>().ToTable("OrdenesCompra");
            modelBuilder.Entity<MetodoPago>().ToTable("MetodosPago");

            modelBuilder.Entity<ActividadUsuario>().ToTable("ActividadUsuario");
            modelBuilder.Entity<DescargaUsuario>().ToTable("DescargasUsuario");

            // ═══════════════════════════════════════════════════════
            // CLAVES PRIMARIAS (Fluent API)
            // ═══════════════════════════════════════════════════════
            modelBuilder.Entity<Tramite>().HasKey(t => t.TramiteId);
            modelBuilder.Entity<CarritoItem>().HasKey(c => c.CarritoItemId);
            modelBuilder.Entity<OrdenCompra>().HasKey(o => o.OrdenId);
            modelBuilder.Entity<MetodoPago>().HasKey(m => m.MetodoPagoId);

            modelBuilder.Entity<ActividadUsuario>().HasKey(a => a.ActividadId);
            modelBuilder.Entity<DescargaUsuario>().HasKey(d => d.DescargaId);

            // ═══════════════════════════════════════════════════════
            // MAPEO EXPLÍCITO DE COLUMNAS
            // ═══════════════════════════════════════════════════════
            modelBuilder.Entity<Usuario>()
                .Property(u => u.DireccionExacta).HasColumnName("DireccionExacta");

            modelBuilder.Entity<Tramite>().Property(t => t.TramiteId).HasColumnName("TramiteId");
            modelBuilder.Entity<Tramite>().Property(t => t.UsuarioId).HasColumnName("UsuarioId");
            modelBuilder.Entity<Tramite>().Property(t => t.Tipo).HasColumnName("Tipo");
            modelBuilder.Entity<Tramite>().Property(t => t.Categoria).HasColumnName("Categoria");
            modelBuilder.Entity<Tramite>().Property(t => t.Estado).HasColumnName("Estado");
            modelBuilder.Entity<Tramite>().Property(t => t.FechaSolicitud).HasColumnName("FechaSolicitud");
            modelBuilder.Entity<Tramite>().Property(t => t.FechaActualizacion).HasColumnName("FechaActualizacion");
            modelBuilder.Entity<Tramite>().Property(t => t.Descripcion).HasColumnName("Descripcion");
            modelBuilder.Entity<Tramite>().Property(t => t.NumeroReferencia).HasColumnName("NumeroReferencia");
            modelBuilder.Entity<Tramite>().Property(t => t.DatosFormulario).HasColumnName("DatosFormulario");

            modelBuilder.Entity<CarritoItem>().Property(c => c.CarritoItemId).HasColumnName("CarritoItemId");
            modelBuilder.Entity<CarritoItem>().Property(c => c.UsuarioId).HasColumnName("UsuarioId");
            modelBuilder.Entity<CarritoItem>().Property(c => c.ProductoId).HasColumnName("ProductoId");
            modelBuilder.Entity<CarritoItem>().Property(c => c.Nombre).HasColumnName("Nombre");
            modelBuilder.Entity<CarritoItem>().Property(c => c.Descripcion).HasColumnName("Descripcion");
            modelBuilder.Entity<CarritoItem>().Property(c => c.Precio).HasColumnName("Precio");
            modelBuilder.Entity<CarritoItem>().Property(c => c.Cantidad).HasColumnName("Cantidad");
            modelBuilder.Entity<CarritoItem>().Property(c => c.Imagen).HasColumnName("Imagen");
            modelBuilder.Entity<CarritoItem>().Property(c => c.FechaAgregado).HasColumnName("FechaAgregado");

            modelBuilder.Entity<OrdenCompra>().Property(o => o.OrdenId).HasColumnName("OrdenId");
            modelBuilder.Entity<OrdenCompra>().Property(o => o.NumeroOrden).HasColumnName("NumeroOrden");
            modelBuilder.Entity<OrdenCompra>().Property(o => o.UsuarioId).HasColumnName("UsuarioId");
            modelBuilder.Entity<OrdenCompra>().Property(o => o.Subtotal).HasColumnName("Subtotal");
            modelBuilder.Entity<OrdenCompra>().Property(o => o.Impuesto).HasColumnName("Impuesto");
            modelBuilder.Entity<OrdenCompra>().Property(o => o.Total).HasColumnName("Total");
            modelBuilder.Entity<OrdenCompra>().Property(o => o.Metodo).HasColumnName("Metodo");
            modelBuilder.Entity<OrdenCompra>().Property(o => o.Estado).HasColumnName("Estado");
            modelBuilder.Entity<OrdenCompra>().Property(o => o.ReferenciaPago).HasColumnName("ReferenciaPago");
            modelBuilder.Entity<OrdenCompra>().Property(o => o.FechaCreacion).HasColumnName("FechaCreacion");
            modelBuilder.Entity<OrdenCompra>().Property(o => o.FechaConfirmacion).HasColumnName("FechaConfirmacion");
            modelBuilder.Entity<OrdenCompra>().Property(o => o.Detalle).HasColumnName("Detalle");

            modelBuilder.Entity<MetodoPago>().Property(m => m.MetodoPagoId).HasColumnName("MetodoPagoId");
            modelBuilder.Entity<MetodoPago>().Property(m => m.UsuarioId).HasColumnName("UsuarioId");
            modelBuilder.Entity<MetodoPago>().Property(m => m.Tipo).HasColumnName("Tipo");
            modelBuilder.Entity<MetodoPago>().Property(m => m.Alias).HasColumnName("Alias");
            modelBuilder.Entity<MetodoPago>().Property(m => m.Ultimos4).HasColumnName("Ultimos4");
            modelBuilder.Entity<MetodoPago>().Property(m => m.Titular).HasColumnName("Titular");
            modelBuilder.Entity<MetodoPago>().Property(m => m.FechaVencimiento).HasColumnName("FechaVencimiento");
            modelBuilder.Entity<MetodoPago>().Property(m => m.Banco).HasColumnName("Banco");
            modelBuilder.Entity<MetodoPago>().Property(m => m.CuentaIBAN).HasColumnName("CuentaIBAN");
            modelBuilder.Entity<MetodoPago>().Property(m => m.Predeterminado).HasColumnName("Predeterminado");
            modelBuilder.Entity<MetodoPago>().Property(m => m.Activo).HasColumnName("Activo");
            modelBuilder.Entity<MetodoPago>().Property(m => m.FechaRegistro).HasColumnName("FechaRegistro");

            modelBuilder.Entity<ActividadUsuario>().Property(a => a.ActividadId).HasColumnName("ActividadId");
            modelBuilder.Entity<ActividadUsuario>().Property(a => a.UsuarioId).HasColumnName("UsuarioId");
            modelBuilder.Entity<ActividadUsuario>().Property(a => a.Seccion).HasColumnName("Seccion");
            modelBuilder.Entity<ActividadUsuario>().Property(a => a.Accion).HasColumnName("Accion");
            modelBuilder.Entity<ActividadUsuario>().Property(a => a.Detalle).HasColumnName("Detalle");
            modelBuilder.Entity<ActividadUsuario>().Property(a => a.DuracionSegundos).HasColumnName("DuracionSegundos");
            modelBuilder.Entity<ActividadUsuario>().Property(a => a.Fecha).HasColumnName("Fecha");

            modelBuilder.Entity<DescargaUsuario>().Property(d => d.DescargaId).HasColumnName("DescargaId");
            modelBuilder.Entity<DescargaUsuario>().Property(d => d.UsuarioId).HasColumnName("UsuarioId");
            modelBuilder.Entity<DescargaUsuario>().Property(d => d.Nombre).HasColumnName("Nombre");
            modelBuilder.Entity<DescargaUsuario>().Property(d => d.Tipo).HasColumnName("Tipo");
            modelBuilder.Entity<DescargaUsuario>().Property(d => d.Seccion).HasColumnName("Seccion");
            modelBuilder.Entity<DescargaUsuario>().Property(d => d.TamanoKB).HasColumnName("TamanoKB");
            modelBuilder.Entity<DescargaUsuario>().Property(d => d.Fecha).HasColumnName("Fecha");

            // ============================================================
            // RELACIONES
            // ============================================================

            // Relaciones principales (con Usuario)
            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.UsuarioRoles)
                .WithRequired(ur => ur.Usuario)
                .HasForeignKey(ur => ur.UsuarioId);

            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.NISEs)
                .WithRequired(n => n.Usuario)
                .HasForeignKey(n => n.UsuarioId);

            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Averias)
                .WithRequired(a => a.Usuario)
                .HasForeignKey(a => a.UsuarioId);

            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Notificaciones)
                .WithRequired(n => n.Usuario)
                .HasForeignKey(n => n.UsuarioId);

            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Suscripciones)
                .WithRequired(s => s.Usuario)
                .HasForeignKey(s => s.UsuarioId);

            // ⭐ Estas 2 son las que faltaban en el modelo Usuario:
            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Tramites)
                .WithRequired(t => t.Usuario)
                .HasForeignKey(t => t.UsuarioId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Pagos)
                .WithRequired(p => p.Usuario)
                .HasForeignKey(p => p.UsuarioId);

            // Relaciones de Rol
            modelBuilder.Entity<Rol>()
                .HasMany(r => r.UsuarioRoles)
                .WithRequired(ur => ur.Rol)
                .HasForeignKey(ur => ur.RolId);

            // Relaciones de NISE
            modelBuilder.Entity<NISE>()
                .HasMany(n => n.Facturas)
                .WithRequired(f => f.NISE)
                .HasForeignKey(f => f.NiseId);

            modelBuilder.Entity<NISE>()
                .HasMany(n => n.Averias)
                .WithRequired(a => a.NISE)
                .HasForeignKey(a => a.NiseId);

            modelBuilder.Entity<NISE>()
                .HasMany(n => n.Suspensiones)
                .WithRequired(s => s.NISE)
                .HasForeignKey(s => s.NiseId);

            // Relaciones de Factura
            modelBuilder.Entity<Factura>()
                .HasMany(f => f.Pagos)
                .WithRequired(p => p.Factura)
                .HasForeignKey(p => p.FacturaId);

            // CarritoItem / OrdenCompra / MetodoPago → Usuario
            modelBuilder.Entity<CarritoItem>()
                .HasRequired(c => c.Usuario)
                .WithMany()
                .HasForeignKey(c => c.UsuarioId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OrdenCompra>()
                .HasRequired(o => o.Usuario)
                .WithMany()
                .HasForeignKey(o => o.UsuarioId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MetodoPago>()
                .HasRequired(m => m.Usuario)
                .WithMany()
                .HasForeignKey(m => m.UsuarioId)
                .WillCascadeOnDelete(false);

            // ActividadUsuario / DescargaUsuario
            modelBuilder.Entity<ActividadUsuario>()
                .HasRequired(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.UsuarioId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DescargaUsuario>()
                .HasRequired(d => d.Usuario)
                .WithMany()
                .HasForeignKey(d => d.UsuarioId)
                .WillCascadeOnDelete(false);
        }
    }
}