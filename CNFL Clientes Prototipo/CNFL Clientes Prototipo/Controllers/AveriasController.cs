using System.IO;
using System.Web;
using System.Web.Mvc;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Services;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class AveriasController : Controller
    {
        private readonly AveriaService _service = new AveriaService();

        // GET: Averias (Index con lista)
        public ActionResult Index()
        {
            var averias = _service.ListarAverias();
            return View(averias);
        }

        // GET: Averias/Create (Recibe el tipo)
        public ActionResult Create(string tipo)
        {
            ViewBag.Tipo = tipo ?? "General";
            return View();
        }

        // POST: Averias/Create (Guarda foto y datos)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Averia averia, HttpPostedFileBase foto, string latitud, string longitud)
        {
            if (ModelState.IsValid)
            {
                // Guardar foto si viene
                if (foto != null && foto.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(foto.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/ImagenesAverias"), fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    foto.SaveAs(path);
                    averia.FotoUrl = "/Content/ImagenesAverias/" + fileName;
                }

                // Guardar ubicación GPS
                averia.Latitud = latitud;
                averia.Longitud = longitud;

                _service.RegistrarAveria(averia);
                return RedirectToAction("Index");
            }
            return View(averia);
        }
    }
}