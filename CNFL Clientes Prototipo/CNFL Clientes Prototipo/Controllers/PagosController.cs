using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class PagosController : Controller
    {
        // GET: Pagos (Index para recibir producto/monto)
        public ActionResult Index(string monto, string producto)
        {
            // 🔒 Si no está logueado, redirige al login
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            ViewBag.Monto = monto ?? "28450";
            ViewBag.Producto = producto ?? "Factura Agosto 2026";
            return View();
        }

        // GET: /Pagos/Sinpe
        public ActionResult Sinpe(string monto, string producto)
        {
            // 🔒 Si no está logueado, redirige al login
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            ViewBag.Monto = monto ?? "28450";
            ViewBag.Producto = producto ?? "Factura Agosto 2026";
            return View();
        }

        // GET: /Pagos/Iban
        public ActionResult Iban(string monto, string producto)
        {
            // 🔒 Si no está logueado, redirige al login
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            ViewBag.Monto = monto ?? "28450";
            ViewBag.Producto = producto ?? "Factura Agosto 2026";
            return View();
        }

        // GET: /Pagos/Tarjeta
        public ActionResult Tarjeta(string monto, string producto)
        {
            // 🔒 Si no está logueado, redirige al login
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            ViewBag.Monto = monto ?? "28450";
            ViewBag.Producto = producto ?? "Factura Agosto 2026";
            return View();
        }
    }
}