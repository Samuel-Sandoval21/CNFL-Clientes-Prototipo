using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Services;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class TramitesController : Controller
    {
        private readonly TramiteService _service = new TramiteService();

        // GET: Trámites (Página principal con catálogo y progreso)
        public ActionResult Index()
        {
            var tramites = _service.ObtenerTodos();
            return View(tramites);
        }

        // GET: Formulario genérico
        public ActionResult Formulario(string tipo)
        {
            ViewBag.Tipo = tipo;
            return View(new FormularioTramite());
        }

        // POST: Formulario genérico
        [HttpPost]
        public ActionResult Formulario(string tipo, FormularioTramite formulario)
        {
            if (ModelState.IsValid)
            {
                // Guardar en servicio
                var tramiteService = new TramiteService();
                tramiteService.CrearTramite(tipo, formulario);

                // Redirigir al Index para ver el avance
                return RedirectToAction("Index");
            }

            ViewBag.Tipo = tipo;
            return View(formulario);
        }
    }
}