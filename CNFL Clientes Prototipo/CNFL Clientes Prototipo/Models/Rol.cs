using System.Collections.Generic;

namespace CNFL_Clientes_Prototipo.Models
{
    public class Rol
    {
        public int RolId { get; set; }
        public string NombreRol { get; set; }

        public virtual ICollection<UsuarioRol> UsuarioRoles { get; set; }
    }
}