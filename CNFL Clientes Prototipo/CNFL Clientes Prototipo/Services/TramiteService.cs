using System;
using System.Collections.Generic;
using System.Linq;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Data;

namespace CNFL_Clientes_Prototipo.Services
{
    public class TramiteService
    {
        private CNFLDbContext _db = new CNFLDbContext();

        public List<Tramite> ObtenerPorUsuario(int usuarioId)
        {
            return _db.Tramites
                .Where(t => t.UsuarioId == usuarioId)
                .OrderByDescending(t => t.FechaSolicitud)
                .ToList();
        }

        public Tramite ObtenerPorId(int id)
        {
            return _db.Tramites.Find(id);
        }

        public void Agregar(Tramite tramite)
        {
            tramite.FechaSolicitud = DateTime.Now;
            tramite.Estado = "Solicitado";
            _db.Tramites.Add(tramite);
            _db.SaveChanges();
        }

        public void ActualizarEstado(int tramiteId, string nuevoEstado)
        {
            var tramite = _db.Tramites.Find(tramiteId);
            if (tramite != null)
            {
                tramite.Estado = nuevoEstado;
                _db.SaveChanges();
            }
        }

        public void AgregarTramite(int usuarioId, string tipo, string descripcion)
        {
            var tramite = new Tramite
            {
                UsuarioId = usuarioId,
                Tipo = tipo,
                Descripcion = descripcion,
                FechaSolicitud = DateTime.Now,
                Estado = "Solicitado"
            };
            _db.Tramites.Add(tramite);
            _db.SaveChanges();
        }

        public List<Tramite> ObtenerTodos()
        {
            return _db.Tramites.OrderByDescending(t => t.FechaSolicitud).ToList();
        }

        public List<Tramite> ObtenerPorEstado(string estado)
        {
            return _db.Tramites.Where(t => t.Estado == estado).OrderByDescending(t => t.FechaSolicitud).ToList();
        }
    }
}