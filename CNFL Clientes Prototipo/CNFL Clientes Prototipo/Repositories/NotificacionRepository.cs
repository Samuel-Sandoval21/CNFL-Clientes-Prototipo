using System;
using System.Collections.Generic;
using System.Linq;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Data;

namespace CNFL_Clientes_Prototipo.Repositories
{
    public class NotificacionRepository
    {
        private CNFLDbContext _db = new CNFLDbContext();

        public List<Notificacion> ObtenerPorUsuario(int usuarioId)
        {
            return _db.Notificaciones
                .Where(n => n.UsuarioId == usuarioId)
                .OrderByDescending(n => n.Fecha)
                .ToList();
        }

        public List<Notificacion> ObtenerNoLeidasPorUsuario(int usuarioId)
        {
            return _db.Notificaciones
                .Where(n => n.UsuarioId == usuarioId && !n.Leida)
                .OrderByDescending(n => n.Fecha)
                .ToList();
        }

        public Notificacion ObtenerPorId(int id)
        {
            return _db.Notificaciones.Find(id);
        }

        public void Agregar(Notificacion notificacion)
        {
            notificacion.Fecha = DateTime.Now;
            _db.Notificaciones.Add(notificacion);
            _db.SaveChanges();
        }

        public void MarcarComoLeida(int id)
        {
            var notificacion = _db.Notificaciones.Find(id);
            if (notificacion != null)
            {
                notificacion.Leida = true;
                _db.SaveChanges();
            }
        }

        public void MarcarTodasComoLeidas(int usuarioId)
        {
            var notificaciones = _db.Notificaciones.Where(n => n.UsuarioId == usuarioId && !n.Leida).ToList();
            foreach (var n in notificaciones)
            {
                n.Leida = true;
            }
            _db.SaveChanges();
        }

        public int ContarNoLeidas(int usuarioId)
        {
            return _db.Notificaciones.Count(n => n.UsuarioId == usuarioId && !n.Leida);
        }
    }
}