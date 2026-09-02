using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace CNFL_Clientes_Prototipo.Models
{
    public class Tramite
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int NISEId { get; set; }
        public string TipoTramite { get; set; }
        public string Estado { get; set; } // En Proceso, Completado, Rechazado
        public string Detalle { get; set; }
        public DateTime FechaSolicitud { get; set; }

        // Propiedades de navegación
        public virtual Usuario Usuario { get; set; }
        public virtual NISE NISE { get; set; }
    }
}