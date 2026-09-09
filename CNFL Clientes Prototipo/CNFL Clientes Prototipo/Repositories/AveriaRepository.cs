using System;
using System.Collections.Generic;
using System.Linq;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Data;

namespace CNFL_Clientes_Prototipo.Repositories
{
    public class AveriaRepository
    {
        private CNFLDbContext _db = new CNFLDbContext();

        public List<Averia> ObtenerTodas()
        {
            return _db.Averias.Include("Usuario").Include("NISE").OrderByDescending(a => a.FechaReporte).ToList();
        }

        public Averia ObtenerPorId(int id)
        {
            return _db.Averias.Include("Usuario").Include("NISE").FirstOrDefault(a => a.AveriaId == id);
        }

        public List<Averia> ObtenerPorUsuario(int usuarioId)
        {
            return _db.Averias.Where(a => a.UsuarioId == usuarioId).OrderByDescending(a => a.FechaReporte).ToList();
        }

        public List<Averia> ObtenerPorNise(int niseId)
        {
            return _db.Averias.Where(a => a.NiseId == niseId).OrderByDescending(a => a.FechaReporte).ToList();
        }

        public List<Averia> ObtenerPorEstado(string estado)
        {
            return _db.Averias.Where(a => a.Estado == estado).OrderByDescending(a => a.FechaReporte).ToList();
        }

        public void Agregar(Averia averia)
        {
            averia.FechaReporte = DateTime.Now;
            _db.Averias.Add(averia);
            _db.SaveChanges();
        }

        public void ActualizarEstado(int averiaId, string nuevoEstado)
        {
            var averia = _db.Averias.Find(averiaId);
            if (averia != null)
            {
                averia.Estado = nuevoEstado;
                averia.FechaActualizacion = DateTime.Now;
                _db.SaveChanges();
            }
        }

        public int ContarPorEstado(string estado)
        {
            return _db.Averias.Count(a => a.Estado == estado);
        }

        public int ContarTotales()
        {
            return _db.Averias.Count();
        }

        public List<Averia> ObtenerRecientes(int cantidad)
        {
            return _db.Averias.OrderByDescending(a => a.FechaReporte).Take(cantidad).ToList();
        }
    }
}