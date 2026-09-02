using System;
using System.Collections.Generic;
using System.Linq;
using CNFL_Clientes_Prototipo.Models;

namespace CNFL_Clientes_Prototipo.Repositories
{
    public class NotificacionRepository
    {
        private static List<Notificacion> _notificaciones = new List<Notificacion>
        {
            new Notificacion
            {
                Id = 1,
                UsuarioId = 2,
                Titulo = "Factura por vencer",
                Mensaje = "Su factura vence en 3 días.",
                Tipo = "Factura",
                Leida = false,
                FechaEnvio = DateTime.Now.AddDays(-2)
            },
            new Notificacion
            {
                Id = 2,
                UsuarioId = 2,
                Titulo = "Avería reportada",
                Mensaje = "Transformador dañado en Barrio Los Ángeles.",
                Tipo = "Avería",
                Leida = false,
                FechaEnvio = DateTime.Now.AddDays(-1)
            },
            new Notificacion
            {
                Id = 3,
                UsuarioId = 2,
                Titulo = "Corte programado",
                Mensaje = "Corte programado el 28 de agosto de 8am a 12pm.",
                Tipo = "Corte",
                Leida = true,
                FechaEnvio = DateTime.Now.AddDays(-3)
            }
        };

        public List<Notificacion> ObtenerTodas()
        {
            return _notificaciones;
        }

        public List<Notificacion> ObtenerPorUsuario(int usuarioId)
        {
            return _notificaciones.Where(n => n.UsuarioId == usuarioId).ToList();
        }

        public Notificacion ObtenerPorId(int id)
        {
            return _notificaciones.FirstOrDefault(n => n.Id == id);
        }

        public void Agregar(Notificacion notificacion)
        {
            notificacion.Id = _notificaciones.Count > 0 ? _notificaciones.Max(n => n.Id) + 1 : 1;
            notificacion.FechaEnvio = DateTime.Now;
            _notificaciones.Add(notificacion);
        }

        public void MarcarComoLeida(int id)
        {
            var notificacion = _notificaciones.FirstOrDefault(n => n.Id == id);
            if (notificacion != null)
            {
                notificacion.Leida = true;
            }
        }

        public void MarcarTodasComoLeidas(int usuarioId)
        {
            var notificaciones = _notificaciones.Where(n => n.UsuarioId == usuarioId && !n.Leida);
            foreach (var n in notificaciones)
            {
                n.Leida = true;
            }
        }
    }
}