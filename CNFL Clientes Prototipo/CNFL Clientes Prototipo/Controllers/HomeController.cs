using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home/Index (Página pública sin login)
        public ActionResult Index()
        {
            // Si ya está logueado, redirigir según rol
            if (Session["Rol"] != null)
            {
                if (Session["Rol"].ToString() == "Admin")
                    return RedirectToAction("Dashboard", "Admin");
                else
                    return RedirectToAction("Inicio", "Clientes");
            }

            // Si no está logueado, mostrar la home pública
            return View();
        }
    }
}