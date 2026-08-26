using System.Web.Mvc;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Services;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class ClientesController : Controller
    {
        // GET: Dashboard de Bienvenida (Solo usuarios logueados)
        public ActionResult Dashboard()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        // GET: Listado de Clientes (SOLO ADMIN)
        public ActionResult Index()
        {
            // Si no está logueado, va al login. Si no es admin, va a su dashboard.
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            if (Session["Rol"].ToString() != "Admin")
                return RedirectToAction("Dashboard", "Clientes");

            // Obtenemos la lista de usuarios registrados del CuentaController
            var usuarios = CuentaController.ObtenerUsuarios();

            // Pasamos la lista a la vista (que debe ser de tipo List<CuentaController.Usuario>)
            return View(usuarios);
        }
    }
}