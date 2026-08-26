using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CNFL_Clientes_Prototipo.Models
{
    public class Averia
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Direccion { get; set; }
        public string Estado { get; set; } // "Reportado", "En revisión", "En camino", "Resuelto"
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; } // "Eléctrica" o "Alumbrado público"
        public string FotoUrl { get; set; } // Nueva propiedad para guardar la ruta de la imagen
    }
}