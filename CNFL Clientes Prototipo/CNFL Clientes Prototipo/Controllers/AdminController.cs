using System.Web.Mvc;
using CNFL_Clientes_Prototipo.Services;



namespace CNFL_Clientes_Prototipo.Controllers
{
    public class AdminController : Controller
    {
        private readonly AveriaService _service = new AveriaService();

        // GET: Admin (Listado de averías para el operador)
        public ActionResult Index()
        {
            // Validación de rol Admin
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Cuenta");
            }

            var averias = _service.ListarAverias();
            return View(averias);
        }

        // POST: Admin/CambiarEstado
        [HttpPost]
        public ActionResult CambiarEstado(int id, string estado)
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Cuenta");
            }

            _service.CambiarEstado(id, estado);
            return RedirectToAction("Index");
        }
    }
}