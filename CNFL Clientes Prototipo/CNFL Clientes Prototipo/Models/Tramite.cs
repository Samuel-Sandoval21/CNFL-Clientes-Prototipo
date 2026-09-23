using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNFL_Clientes_Prototipo.Models
{
    [Table("Tramites")]
    public class Tramite
    {
        [Key]
        public int TramiteId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        public int? NiseId { get; set; }

        [StringLength(20)]
        public string NumeroNise { get; set; }

        [Required]
        [StringLength(150)]
        public string Tipo { get; set; }

        [StringLength(100)]
        public string Categoria { get; set; }

        [Required]
        [StringLength(50)]
        public string Estado { get; set; }

        [StringLength(30)]
        public string NumeroReferencia { get; set; }

        [StringLength(2000)]
        public string Descripcion { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string DatosFormulario { get; set; }

        [Required]
        public DateTime FechaSolicitud { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public DateTime? FechaEstimadaFinalizacion { get; set; }

        public DateTime? FechaFinalizacion { get; set; }

        [Range(0, 100)]
        public int Progreso { get; set; }

        [StringLength(100)]
        public string AgenteAsignado { get; set; }

        [StringLength(1000)]
        public string ComentarioAgente { get; set; }

        [StringLength(500)]
        public string MotivoRechazo { get; set; }

        // Navegación
        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }

        [ForeignKey("NiseId")]
        public virtual NISE NISE { get; set; }

        public virtual ICollection<TramiteDocumento> TramiteDocumentos { get; set; }

        public Tramite()
        {
            FechaSolicitud = DateTime.Now;
            Estado = "Activo";
            Progreso = 10;
            TramiteDocumentos = new HashSet<TramiteDocumento>();
        }
    }
}