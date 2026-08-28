using System.Collections.Generic;
using System.Linq;
using CNFL_Clientes_Prototipo.Models;

namespace CNFL_Clientes_Prototipo.Repositories
{
    public class FacturaRepository
    {
        private static List<Factura> _facturas = new List<Factura>()
        {
            new Factura { Id = 1, Periodo = "Agosto 2026", Monto = 28450, Estado = "Pendiente" },
            new Factura { Id = 2, Periodo = "Julio 2026", Monto = 26980, Estado = "Pagada" }
        };

        public List<Factura> ObtenerTodas()
        {
            return _facturas;
        }

        // NUEVO: Obtener una factura por ID
        public Factura ObtenerPorId(int id)
        {
            return _facturas.FirstOrDefault(f => f.Id == id);
        }

        public void Pagar(int id)
        {
            var factura = _facturas.Find(f => f.Id == id);
            if (factura != null) factura.Estado = "Pagada";
        }
    }
}