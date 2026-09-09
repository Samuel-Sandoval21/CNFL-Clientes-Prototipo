using System;
using System.Collections.Generic;
using System.Linq;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Data;

namespace CNFL_Clientes_Prototipo.Services
{
    public class SuscripcionService
    {
        private CNFLDbContext _db = new CNFLDbContext();

        public Suscripcion ObtenerPorId(int id)
        {
            return _db.Suscripciones.Find(id);
        }

        public List<Suscripcion> ObtenerPorUsuario(int usuarioId)
        {
            return _db.Suscripciones
                .Where(s => s.UsuarioId == usuarioId)
                .OrderByDescending(s => s.FechaInicio)
                .ToList();
        }

        public List<Suscripcion> ObtenerActivasPorUsuario(int usuarioId)
        {
            return _db.Suscripciones
                .Where(s => s.UsuarioId == usuarioId && s.Activa)
                .OrderByDescending(s => s.FechaInicio)
                .ToList();
        }

        public List<Suscripcion> ObtenerPorServicio(string servicio)
        {
            return _db.Suscripciones
                .Where(s => s.Servicio == servicio && s.Activa)
                .ToList();
        }

        public void Agregar(Suscripcion suscripcion)
        {
            suscripcion.FechaInicio = DateTime.Now;
            suscripcion.Activa = true;
            _db.Suscripciones.Add(suscripcion);
            _db.SaveChanges();
        }

        public void Cancelar(int suscripcionId)
        {
            var suscripcion = _db.Suscripciones.Find(suscripcionId);
            if (suscripcion != null)
            {
                suscripcion.Activa = false;
                suscripcion.FechaFin = DateTime.Now;
                _db.SaveChanges();
            }
        }

        public void Reactivar(int suscripcionId)
        {
            var suscripcion = _db.Suscripciones.Find(suscripcionId);
            if (suscripcion != null)
            {
                suscripcion.Activa = true;
                suscripcion.FechaInicio = DateTime.Now;
                suscripcion.FechaFin = null;
                _db.SaveChanges();
            }
        }

        public bool TieneSuscripcionActiva(int usuarioId, string servicio)
        {
            return _db.Suscripciones.Any(s =>
                s.UsuarioId == usuarioId &&
                s.Servicio == servicio &&
                s.Activa);
        }
    }
}