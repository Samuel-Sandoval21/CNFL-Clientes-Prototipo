using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class CuentaController : Controller
    {
        // GET: Home Público (Sin necesidad de login)
        public ActionResult Index()
        {
            return View();
        }

        // GET: Pantalla de Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Login (Simulado)
        [HttpPost]
        public ActionResult Login(string usuario, string contrasena)
        {
            // Simulación: si el usuario es "admin" y pass "123", entra al Dashboard de Clientes
            if (usuario == "admin" && contrasena == "123")
            {
                // Redirigimos a la zona logueada
                return RedirectToAction("Index", "Clientes");
            }
            ViewBag.Error = "Credenciales incorrectas";
            return View();
        }
    }
}