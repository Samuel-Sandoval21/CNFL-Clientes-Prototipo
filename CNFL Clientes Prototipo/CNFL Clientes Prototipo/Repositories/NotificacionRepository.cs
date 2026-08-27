using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CNFL_Clientes_Prototipo.Models;

namespace CNFL_Clientes_Prototipo.Repositories
{
    public class NotificacionRepository
    {
        private static List<Notificacion> _notifs = new List<Notificacion>()
        {
            new Notificacion { Id = 1, Titulo = "Factura próxima a vencer", Mensaje = "Su factura de Agosto 2026 vence en 3 días.", Tipo = "Factura", Fecha = "26/08/2026", Leida = false },
            new Notificacion { Id = 2, Titulo = "Avería en su zona", Mensaje = "Se reportó un transformador dañado en Barrio Los Ángeles.", Tipo = "Avería", Fecha = "26/08/2026", Leida = false },
            new Notificacion { Id = 3, Titulo = "Mantenimiento programado", Mensaje = "Habrá corte programado el 28 de agosto de 8am a 12pm.", Tipo = "Corte", Fecha = "25/08/2026", Leida = true }
        };

        public List<Notificacion> ObtenerTodas() => _notifs;
        public void MarcarComoLeidas()
        {
            foreach (var n in _notifs) n.Leida = true;
        }
    }
}