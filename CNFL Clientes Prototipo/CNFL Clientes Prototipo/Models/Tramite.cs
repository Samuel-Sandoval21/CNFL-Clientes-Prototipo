using System;
using System.Collections.Generic;

namespace CNFL_Clientes_Prototipo.Models
{
    public class Tramite
    {
        public int TramiteId { get; set; }
        public int UsuarioId { get; set; }
        public string Tipo { get; set; }
        public string Categoria { get; set; }
        public string Estado { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string Descripcion { get; set; }
        public string NumeroReferencia { get; set; }
        public string DatosFormulario { get; set; }

        // Navegación — SOLO Usuario
        public virtual Usuario Usuario { get; set; }
    }
}