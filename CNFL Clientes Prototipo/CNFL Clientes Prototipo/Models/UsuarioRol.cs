using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNFL_Clientes_Prototipo.Models
{
    public class UsuarioRol
    {
        [Key]
        public int UsuarioRolId { get; set; }

        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }

        public int RolId { get; set; }

        [ForeignKey("RolId")]
        public virtual Rol Rol { get; set; }    // ← CLAVE para detectar rol
    }
}