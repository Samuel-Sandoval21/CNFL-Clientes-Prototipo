using System;
using System.ComponentModel.DataAnnotations;

namespace CNFL_Clientes_Prototipo.Models
{
    public class Notificacion
    {
        [Key]
        public int NotificacionId { get; set; }

        public int UsuarioId { get; set; }
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
        public bool Leida { get; set; }

        // ⭐ Tipo de notificación
        // Valores: Averia | Suspension | Pago | Evento
        public string Tipo { get; set; }

        // ⭐ Estado específico según el tipo
        // Averia:     Reportada | EnProceso | Finalizada
        // Suspension: Programada | EnProceso
        // Pago:       PorVencer | Vencida | Pagada
        // Evento:     VoltajeFueraRango | ConsumoFueraLimite
        public string Estado { get; set; }

        // Navegación
        public virtual Usuario Usuario { get; set; }
    }
}