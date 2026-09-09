using System;

namespace CNFL_Clientes_Prototipo.Models
{
    public class Tramite
    {
        public int TramiteId { get; set; }
        public int UsuarioId { get; set; }
        public string Tipo { get; set; }
        public string Estado { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string Descripcion { get; set; }

        public virtual Usuario Usuario { get; set; }
    }
}