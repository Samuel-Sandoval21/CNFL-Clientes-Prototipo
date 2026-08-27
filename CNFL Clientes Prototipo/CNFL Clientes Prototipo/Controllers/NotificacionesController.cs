using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CNFL_Clientes_Prototipo.Services;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class NotificacionesController : Controller
    {
        private readonly NotificacionService _service = new NotificacionService();

        public ActionResult Index()
        {
            if (Session["Rol"] == null) return RedirectToAction("Login", "Cuenta");
            return View(_service.Listar());
        }

        [HttpPost]
        public ActionResult MarcarLeidas()
        {
            _service.LeerTodas();
            return RedirectToAction("Index");
        }
    }
}