using System;
using System.Collections.Generic;
using System.Linq;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Data;

namespace CNFL_Clientes_Prototipo.Services
{
    public class NISEService
    {
        private CNFLDbContext _db = new CNFLDbContext();

        public NISE ObtenerPorId(int id)
        {
            return _db.NISEs.Find(id);
        }

        public NISE ObtenerPorNumero(string numeroNise)
        {
            return _db.NISEs.FirstOrDefault(n => n.NumeroNise == numeroNise);
        }

        public List<NISE> ObtenerPorUsuario(int usuarioId)
        {
            return _db.NISEs.Where(n => n.UsuarioId == usuarioId).ToList();
        }

        public List<NISE> ObtenerTodos()
        {
            return _db.NISEs.ToList();
        }

        public void Agregar(NISE nise)
        {
            _db.NISEs.Add(nise);
            _db.SaveChanges();
        }

        public void Actualizar(NISE nise)
        {
            var existente = _db.NISEs.Find(nise.NiseId);
            if (existente != null)
            {
                existente.NumeroNise = nise.NumeroNise;
                existente.Direccion = nise.Direccion;
                existente.Provincia = nise.Provincia;
                existente.Canton = nise.Canton;
                existente.Distrito = nise.Distrito;
                existente.TipoServicio = nise.TipoServicio;
                _db.SaveChanges();
            }
        }

        public void Eliminar(int id)
        {
            var nise = _db.NISEs.Find(id);
            if (nise != null)
            {
                _db.NISEs.Remove(nise);
                _db.SaveChanges();
            }
        }

        public bool ExisteNumero(string numeroNise)
        {
            return _db.NISEs.Any(n => n.NumeroNise == numeroNise);
        }

        public bool TieneFacturasPendientes(int niseId)
        {
            return _db.Facturas.Any(f => f.NiseId == niseId && !f.Pagada);
        }
    }
}