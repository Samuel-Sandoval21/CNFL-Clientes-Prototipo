using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNFL_Clientes_Prototipo.Models
{
    public class ActividadUsuario
    {
        [Key]
        public int ActividadId { get; set; }

        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }

        [StringLength(100)]
        public string Seccion { get; set; }

        [StringLength(100)]
        public string Accion { get; set; }

        [StringLength(500)]
        public string Detalle { get; set; }

        public int? DuracionSegundos { get; set; }

        public DateTime Fecha { get; set; }

        public ActividadUsuario()
        {
            Fecha = DateTime.Now;
        }
    }
}