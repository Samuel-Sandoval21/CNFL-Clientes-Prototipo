using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class PagosController : Controller
    {
        public ActionResult Index(string monto, string producto)
        {
            ViewBag.Monto = monto ?? "28450";
            ViewBag.Producto = producto ?? "Factura Agosto 2026";
            return View();
        }

        public ActionResult Sinpe(string monto, string producto)
        {
            ViewBag.Monto = monto ?? "28450";
            ViewBag.Producto = producto ?? "Factura Agosto 2026";
            return View();
        }

        public ActionResult Iban(string monto, string producto)
        {
            ViewBag.Monto = monto ?? "28450";
            ViewBag.Producto = producto ?? "Factura Agosto 2026";
            return View();
        }

        public ActionResult Tarjeta(string monto, string producto)
        {
            ViewBag.Monto = monto ?? "28450";
            ViewBag.Producto = producto ?? "Factura Agosto 2026";
            return View();
        }
    }
}