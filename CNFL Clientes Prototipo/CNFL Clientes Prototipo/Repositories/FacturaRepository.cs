using System;
using System.Collections.Generic;
using System.Linq;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Data;

namespace CNFL_Clientes_Prototipo.Repositories
{
    public class FacturaRepository
    {
        private CNFLDbContext _db = new CNFLDbContext();

        public List<Factura> ObtenerTodas()
        {
            return _db.Facturas.Include("NISE").OrderByDescending(f => f.FechaEmision).ToList();
        }

        public Factura ObtenerPorId(int id)
        {
            return _db.Facturas.Include("NISE").FirstOrDefault(f => f.FacturaId == id);
        }

        public List<Factura> ObtenerPorNise(int niseId)
        {
            return _db.Facturas.Where(f => f.NiseId == niseId).OrderByDescending(f => f.FechaEmision).ToList();
        }

        public List<Factura> ObtenerPorUsuario(int usuarioId)
        {
            var nisesIds = _db.NISEs.Where(n => n.UsuarioId == usuarioId).Select(n => n.NiseId).ToList();
            return _db.Facturas.Where(f => nisesIds.Contains(f.NiseId)).OrderByDescending(f => f.FechaEmision).ToList();
        }

        public List<Factura> ObtenerPendientesPorUsuario(int usuarioId)
        {
            var nisesIds = _db.NISEs.Where(n => n.UsuarioId == usuarioId).Select(n => n.NiseId).ToList();
            return _db.Facturas.Where(f => nisesIds.Contains(f.NiseId) && !f.Pagada).OrderBy(f => f.FechaVencimiento).ToList();
        }

        public void PagarFactura(int facturaId)
        {
            var factura = _db.Facturas.Find(facturaId);
            if (factura != null && !factura.Pagada)
            {
                factura.Pagada = true;
                _db.SaveChanges();
            }
        }

        public decimal TotalPendientePorUsuario(int usuarioId)
        {
            var nisesIds = _db.NISEs.Where(n => n.UsuarioId == usuarioId).Select(n => n.NiseId).ToList();
            return _db.Facturas.Where(f => nisesIds.Contains(f.NiseId) && !f.Pagada).Sum(f => f.Monto);
        }
    }
}