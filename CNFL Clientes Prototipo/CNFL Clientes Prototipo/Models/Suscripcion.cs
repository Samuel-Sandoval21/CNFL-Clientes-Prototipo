using System;

namespace CNFL_Clientes_Prototipo.Models
{
    public class Suscripcion
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Servicio { get; set; }
        public string Estado { get; set; } // Activa, Inactiva, Cancelada
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        // Propiedad de navegación
        public virtual Usuario Usuario { get; set; }
    }
}