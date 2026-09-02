using System;
using System.Collections.Generic;
using System.Linq;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Data;

namespace CNFL_Clientes_Prototipo.Services
{
    public class AveriaService : IDisposable
    {
        private readonly CNFLDbContext _db = new CNFLDbContext();

        public List<Averia> ListarAverias()
        {
            return _db.Averias
                .OrderByDescending(a => a.FechaReporte)
                .ToList();
        }

        public void RegistrarAveria(Averia averia)
        {
            _db.Averias.Add(averia);
            _db.SaveChanges();
        }

        public void CambiarEstado(int id, string nuevoEstado)
        {
            var averia = _db.Averias.Find(id);
            if (averia != null)
            {
                averia.Estado = nuevoEstado;
                _db.SaveChanges();
            }
        }

        public Averia ObtenerPorId(int id)
        {
            return _db.Averias.Find(id);
        }

        public void EliminarAveria(int id)
        {
            var averia = _db.Averias.Find(id);
            if (averia != null)
            {
                _db.Averias.Remove(averia);
                _db.SaveChanges();
            }
        }

        public List<Averia> ListarAveriasPorEstado(string estado)
        {
            return _db.Averias
                .Where(a => a.Estado == estado)
                .OrderByDescending(a => a.FechaReporte)
                .ToList();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}