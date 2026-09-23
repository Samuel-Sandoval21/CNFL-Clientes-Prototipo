using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNFL_Clientes_Prototipo.Models
{
    [Table("TramiteDocumentos")]
    public class TramiteDocumento
    {
        [Key]
        public int DocumentoId { get; set; }

        [Required]
        public int TramiteId { get; set; }

        [ForeignKey("TramiteId")]
        public virtual Tramite Tramite { get; set; }

        [StringLength(200)]
        public string NombreRequisito { get; set; }

        [StringLength(300)]
        public string NombreArchivo { get; set; }

        [StringLength(500)]
        public string RutaArchivo { get; set; }

        [StringLength(20)]
        public string TipoArchivo { get; set; }

        public long TamanoBytes { get; set; }

        public DateTime FechaSubida { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string DatosFormulario { get; set; }

        public TramiteDocumento()
        {
            FechaSubida = DateTime.Now;
        }
    }
}