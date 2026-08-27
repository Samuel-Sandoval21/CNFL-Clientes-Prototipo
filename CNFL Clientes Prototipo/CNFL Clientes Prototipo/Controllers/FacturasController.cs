using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;
using CNFL_Clientes_Prototipo.Services;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class FacturasController : Controller
    {
        private readonly FacturaService _service = new FacturaService();

        // GET: Facturas (Solo logueados)
        public ActionResult Index()
        {
            if (Session["Rol"] == null) return RedirectToAction("Login", "Cuenta");
            return View(_service.ListarFacturas());
        }

        // POST: Pagar
        [HttpPost]
        public ActionResult Pagar(int id)
        {
            _service.PagarFactura(id);
            return RedirectToAction("Index");
        }
    }
}