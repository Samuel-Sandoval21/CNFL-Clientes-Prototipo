using System.Web.Mvc;
using CNFL_Clientes_Prototipo.Services;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class FacturasController : Controller
    {
        private readonly FacturaService _service = new FacturaService();

        // GET: Facturas (Listado)
        public ActionResult Index()
        {
            if (Session["Rol"] == null) return RedirectToAction("Login", "Cuenta");
            return View(_service.ListarFacturas());
        }

        // NUEVO: GET: Facturas/Detalle (Muestra una factura específica)
        public ActionResult Detalle(int id)
        {
            if (Session["Rol"] == null) return RedirectToAction("Login", "Cuenta");

            var factura = _service.ObtenerPorId(id);
            if (factura == null) return RedirectToAction("Index", "Facturas");

            return View(factura);
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