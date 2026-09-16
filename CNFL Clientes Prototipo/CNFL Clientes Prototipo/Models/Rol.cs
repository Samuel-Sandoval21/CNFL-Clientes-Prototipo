using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CNFL_Clientes_Prototipo.Models
{
    public class Rol
    {
        [Key]
        public int RolId { get; set; }

        [Required]
        [StringLength(20)]
        public string NombreRol { get; set; }

        public virtual ICollection<UsuarioRol> UsuarioRoles { get; set; }

        public Rol()
        {
            UsuarioRoles = new HashSet<UsuarioRol>();
        }
    }
}