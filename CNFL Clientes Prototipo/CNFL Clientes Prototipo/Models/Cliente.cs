using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNFL_Clientes_Prototipo.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Direccion { get; set; }
        public string Provincia { get; set; }
        public string Canton { get; set; }
        public string Distrito { get; set; }

        // Propiedades de navegación
        public virtual Usuario Usuario { get; set; }
        public virtual ICollection<NISE> NISEs { get; set; }
    }
}