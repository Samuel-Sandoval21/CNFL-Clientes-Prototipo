using System;

namespace CNFL_Clientes_Prototipo.Models
{
    public class Suscripcion
    {
        public int SuscripcionId { get; set; }
        public int UsuarioId { get; set; }
        public string Servicio { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool Activa { get; set; }
        public decimal? MontoMensual { get; set; }

        public virtual Usuario Usuario { get; set; }
    }
}