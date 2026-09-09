using System;
using System.Collections.Generic;
using System.Linq;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Data;

namespace CNFL_Clientes_Prototipo.Services
{
    public class DashboardService
    {
        private CNFLDbContext _db = new CNFLDbContext();

        public int TotalUsuarios()
        {
            return _db.Usuarios.Count();
        }

        public int TotalClientes()
        {
            return _db.Usuarios
                .Count(u => u.UsuarioRoles.Any(ur => ur.Rol.NombreRol == "Cliente"));
        }

        public int TotalAverias()
        {
            return _db.Averias.Count();
        }

        public int TotalAveriasPorEstado(string estado)
        {
            return _db.Averias.Count(a => a.Estado == estado);
        }

        public decimal TotalFacturasPendientes()
        {
            return _db.Facturas.Where(f => !f.Pagada).Sum(f => f.Monto);
        }

        public decimal TotalFacturasPagadas()
        {
            return _db.Facturas.Where(f => f.Pagada).Sum(f => f.Monto);
        }

        public List<AveriaPorMes> ObtenerAveriasPorMes()
        {
            return _db.Averias
                .GroupBy(a => a.FechaReporte.Month)
                .Select(g => new AveriaPorMes
                {
                    Mes = g.Key.ToString(),
                    Cantidad = g.Count()
                }).ToList();
        }

        public List<Suscripcion> ObtenerSuscripcionesActivas()
        {
            return _db.Suscripciones.Where(s => s.Activa).ToList();
        }
    }
}