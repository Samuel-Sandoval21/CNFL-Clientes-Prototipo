using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // Redirigimos a la nueva Home pública (Cuenta/Index)
            return RedirectToAction("Index", "Cuenta");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Tu aplicación de descripción.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Tu página de contacto.";
            return View();
        }
    }
}