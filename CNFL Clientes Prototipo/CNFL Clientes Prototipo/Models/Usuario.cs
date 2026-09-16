using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNFL_Clientes_Prototipo.Models
{
    public class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }

        // ============================================================
        // IDENTIFICACIÓN
        // ============================================================
        [Required]
        [StringLength(50)]
        public string Cedula { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(100)]
        public string Apellidos { get; set; }

        [StringLength(50)]
        public string NombreUsuario { get; set; }

        [Required]
        [StringLength(200)]
        public string Correo { get; set; }

        [StringLength(200)]
        public string CorreoSecundario { get; set; }

        [StringLength(20)]
        public string Telefono { get; set; }

        [StringLength(20)]
        public string TelefonoSecundario { get; set; }

        [StringLength(50)]
        public string Sexo { get; set; }

        // ============================================================
        // SEGURIDAD
        // ============================================================
        [Required]
        [StringLength(200)]
        public string Contraseña { get; set; }

        public bool Activo { get; set; }

        public DateTime? FechaRegistro { get; set; }

        // ============================================================
        // UBICACIÓN
        // ============================================================
        [StringLength(100)]
        public string Provincia { get; set; }

        [StringLength(100)]
        public string Canton { get; set; }

        [StringLength(100)]
        public string Distrito { get; set; }

        [StringLength(300)]
        public string DireccionExacta { get; set; }

        // ============================================================
        // FACTURACIÓN ELECTRÓNICA
        // ============================================================
        public bool FacturaElectronica { get; set; }

        public int? ActividadEconomicaId { get; set; }

        [ForeignKey("ActividadEconomicaId")]
        public virtual ActividadEconomica ActividadEconomica { get; set; }

        // ============================================================
        // FOTO DE PERFIL
        // ============================================================
        [StringLength(300)]
        public string FotoPerfil { get; set; }

        // ============================================================
        // NAVEGACIÓN
        // ============================================================
        public virtual ICollection<NISE> NISEs { get; set; }
        public virtual ICollection<Averia> Averias { get; set; }
        public virtual ICollection<Notificacion> Notificaciones { get; set; }
        public virtual ICollection<Suscripcion> Suscripciones { get; set; }
        public virtual ICollection<UsuarioRol> UsuarioRoles { get; set; }
        public virtual ICollection<Tramite> Tramites { get; set; }
        public virtual ICollection<Pago> Pagos { get; set; }

        public Usuario()
        {
            NISEs = new HashSet<NISE>();
            Averias = new HashSet<Averia>();
            Notificaciones = new HashSet<Notificacion>();
            Suscripciones = new HashSet<Suscripcion>();
            UsuarioRoles = new HashSet<UsuarioRol>();
            Tramites = new HashSet<Tramite>();
            Pagos = new HashSet<Pago>();
        }
    }
}