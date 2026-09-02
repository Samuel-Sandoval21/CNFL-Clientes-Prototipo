using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace CNFL_Clientes_Prototipo.Models
{
    public class Rol
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

        // Propiedad de navegación
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}