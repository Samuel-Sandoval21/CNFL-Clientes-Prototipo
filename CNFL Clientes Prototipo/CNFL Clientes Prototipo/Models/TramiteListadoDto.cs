using System;

namespace CNFL_Clientes_Prototipo.Models
{
    public class TramiteListadoDto
    {
        public int TramiteId { get; set; }
        public string Tipo { get; set; }
        public string Categoria { get; set; }
        public string NumeroReferencia { get; set; }
        public string NumeroNise { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaEstimadaFinalizacion { get; set; }
        public DateTime? FechaFinalizacion { get; set; }
        public string Estado { get; set; }
        public int Progreso { get; set; }
        public string Descripcion { get; set; }
    }
}