using System;

namespace CNFL_Clientes_Prototipo.Models
{
    public class Notificacion
    {
        public int NotificacionId { get; set; }
        public int UsuarioId { get; set; }
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
        public bool Leida { get; set; }
        public string Tipo { get; set; }

        public virtual Usuario Usuario { get; set; }
    }
}