using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNFL_Clientes_Prototipo.Models
{
    public class DescargaUsuario
    {
        [Key]
        public int DescargaId { get; set; }

        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }

        [StringLength(200)]
        public string Nombre { get; set; }

        [StringLength(50)]
        public string Tipo { get; set; }

        [StringLength(100)]
        public string Seccion { get; set; }

        public int? TamanoKB { get; set; }

        public DateTime Fecha { get; set; }

        public DescargaUsuario()
        {
            Fecha = DateTime.Now;
        }
    }
}