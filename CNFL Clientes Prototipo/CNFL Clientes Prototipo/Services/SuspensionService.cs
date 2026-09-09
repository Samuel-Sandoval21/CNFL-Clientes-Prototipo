using System;
using System.Collections.Generic;
using System.Linq;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Data;

namespace CNFL_Clientes_Prototipo.Services
{
    public class SuspensionService
    {
        private CNFLDbContext _db = new CNFLDbContext();

        public Suspension ObtenerPorId(int id)
        {
            return _db.Suspensiones.Find(id);
        }

        public List<Suspension> ObtenerPorNise(int niseId)
        {
            return _db.Suspensiones
                .Where(s => s.NiseId == niseId)
                .OrderBy(s => s.FechaInicio)
                .ToList();
        }

        public List<Suspension> ObtenerPorUsuario(int usuarioId)
        {
            var nisesIds = _db.NISEs.Where(n => n.UsuarioId == usuarioId).Select(n => n.NiseId).ToList();
            return _db.Suspensiones
                .Where(s => nisesIds.Contains(s.NiseId))
                .OrderBy(s => s.FechaInicio)
                .ToList();
        }

        public List<Suspension> ObtenerVigentes()
        {
            return _db.Suspensiones
                .Where(s => s.FechaInicio <= DateTime.Now && s.FechaFin >= DateTime.Now && s.Estado == "Programada")
                .OrderBy(s => s.FechaInicio)
                .ToList();
        }

        public void Agregar(Suspension suspension)
        {
            suspension.Estado = "Programada";
            _db.Suspensiones.Add(suspension);
            _db.SaveChanges();
        }

        public void ActualizarEstado(int suspensionId, string nuevoEstado)
        {
            var suspension = _db.Suspensiones.Find(suspensionId);
            if (suspension != null)
            {
                suspension.Estado = nuevoEstado;
                _db.SaveChanges();
            }
        }
    }
}