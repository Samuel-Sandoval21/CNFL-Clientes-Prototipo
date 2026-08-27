using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;


namespace CNFL_Clientes_Prototipo.Controllers
{
    public class PagosController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Pagos/Sinpe
        public ActionResult Sinpe(string monto, string periodo)
        {
            ViewBag.Monto = monto;
            ViewBag.Periodo = periodo;
            return View();
        }

        // GET: /Pagos/Iban
        public ActionResult Iban(string monto, string periodo)
        {
            ViewBag.Monto = monto;
            ViewBag.Periodo = periodo;
            return View();
        }

        // GET: /Pagos/Tarjeta
        public ActionResult Tarjeta(string monto, string periodo)
        {
            ViewBag.Monto = monto;
            ViewBag.Periodo = periodo;
            return View();
        }
    }
}