using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Repositories;

namespace CNFL_Clientes_Prototipo.Services
{
    public class FacturaService
    {
        private readonly FacturaRepository _repository = new FacturaRepository();
        public List<Factura> ListarFacturas() => _repository.ObtenerTodas();
        public void PagarFactura(int id) => _repository.Pagar(id);
    }
}