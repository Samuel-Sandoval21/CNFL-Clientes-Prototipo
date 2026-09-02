using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using CNFL_Clientes_Prototipo.Models;

namespace CNFL_Clientes_Prototipo.Data
{
    public class CNFLDbContext : DbContext
    {
        public CNFLDbContext() : base("name=CNFLDbContext")
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<NISE> NISEs { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Averia> Averias { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<Tramite> Tramites { get; set; }
        public DbSet<Suscripcion> Suscripciones { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==========================================================
            // CONFIGURAR RELACIÓN Usuario -> Rol
            // ==========================================================
            modelBuilder.Entity<Usuario>()
                .HasRequired(u => u.RolNavigation)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId);

            // ==========================================================
            // CONFIGURAR RELACIÓN Cliente -> Usuario
            // ==========================================================
            modelBuilder.Entity<Cliente>()
                .HasRequired(c => c.Usuario)
                .WithOptional(u => u.Cliente)
                .WillCascadeOnDelete(true);

            // ==========================================================
            // CONFIGURAR RELACIÓN NISE -> Cliente
            // ==========================================================
            modelBuilder.Entity<NISE>()
                .HasRequired(n => n.Cliente)
                .WithMany(c => c.NISEs)
                .HasForeignKey(n => n.ClienteId);

            // ==========================================================
            // CONFIGURAR RELACIÓN Factura -> NISE
            // ==========================================================
            modelBuilder.Entity<Factura>()
                .HasRequired(f => f.NISE)
                .WithMany(n => n.Facturas)
                .HasForeignKey(f => f.NISEId);

            // ==========================================================
            // CONFIGURAR RELACIÓN Pago -> Factura
            // ==========================================================
            modelBuilder.Entity<Pago>()
                .HasRequired(p => p.Factura)
                .WithMany(f => f.Pagos)
                .HasForeignKey(p => p.FacturaId);

            // ==========================================================
            // CONFIGURAR RELACIÓN Pago -> Usuario
            // ==========================================================
            modelBuilder.Entity<Pago>()
                .HasRequired(p => p.Usuario)
                .WithMany(u => u.Pagos)
                .HasForeignKey(p => p.UsuarioId);

            // ==========================================================
            // CONFIGURAR RELACIÓN Averia -> Usuario
            // ==========================================================
            modelBuilder.Entity<Averia>()
                .HasRequired(a => a.Usuario)
                .WithMany(u => u.Averias)
                .HasForeignKey(a => a.UsuarioId);

            // ==========================================================
            // CONFIGURAR RELACIÓN Averia -> NISE
            // ==========================================================
            modelBuilder.Entity<Averia>()
                .HasRequired(a => a.NISE)
                .WithMany(n => n.Averias)
                .HasForeignKey(a => a.NISEId);

            // ==========================================================
            // CONFIGURAR RELACIÓN Notificacion -> Usuario
            // ==========================================================
            modelBuilder.Entity<Notificacion>()
                .HasRequired(n => n.Usuario)
                .WithMany(u => u.Notificaciones)
                .HasForeignKey(n => n.UsuarioId);

            // ==========================================================
            // CONFIGURAR RELACIÓN Tramite -> Usuario
            // ==========================================================
            modelBuilder.Entity<Tramite>()
                .HasRequired(t => t.Usuario)
                .WithMany(u => u.Tramites)
                .HasForeignKey(t => t.UsuarioId);

            // ==========================================================
            // CONFIGURAR RELACIÓN Tramite -> NISE
            // ==========================================================
            modelBuilder.Entity<Tramite>()
                .HasRequired(t => t.NISE)
                .WithMany(n => n.Tramites)
                .HasForeignKey(t => t.NISEId);

            // ==========================================================
            // CONFIGURAR RELACIÓN Suscripcion -> Usuario
            // ==========================================================
            modelBuilder.Entity<Suscripcion>()
                .HasRequired(s => s.Usuario)
                .WithMany(u => u.Suscripciones)
                .HasForeignKey(s => s.UsuarioId);
        }
    }
}