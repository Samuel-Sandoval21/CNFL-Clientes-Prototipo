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

        public ActionResult Index()
        {
            var averias = _service.ListarAverias();
            return View(averias);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Averia averia, HttpPostedFileBase foto)
        {
            if (ModelState.IsValid)
            {
                if (foto != null && foto.ContentLength > 0)
                {
                    // Guardamos la imagen en la carpeta Content/ImagenesAverias
                    var fileName = Path.GetFileName(foto.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/ImagenesAverias"), fileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(path)); // Crear carpeta si no existe
                    foto.SaveAs(path);
                    averia.FotoUrl = "/Content/ImagenesAverias/" + fileName;
                }

                _service.RegistrarAveria(averia);
                return RedirectToAction("Index");
            }
            return View(averia);
        }
    }
}