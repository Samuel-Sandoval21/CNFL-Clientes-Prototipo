using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace CNFL_Clientes_Prototipo.Models
{
    public class Tramite
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; } // "Iniciado", "En proceso", "Resuelto"
        public DateTime Fecha { get; set; }
    }
}