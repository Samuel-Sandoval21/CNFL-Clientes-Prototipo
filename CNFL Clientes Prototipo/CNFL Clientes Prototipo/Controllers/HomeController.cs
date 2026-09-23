using CNFL_Clientes_Prototipo.Services;
using System;
using System.Web;
using System.Web.Mvc;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class HomeController : Controller
    {
        // ═══════════════════════════════════════════════════════════
        // GET: /Home/Index
        // ═══════════════════════════════════════════════════════════
        public ActionResult Index()
        {
            return View();
        }

        // ═══════════════════════════════════════════════════════════
        // GET: /Home/Contacto
        // ═══════════════════════════════════════════════════════════
        public ActionResult Contacto()
        {
            return View();
        }

        // ═══════════════════════════════════════════════════════════
        // POST: /Home/Contacto
        // ═══════════════════════════════════════════════════════════
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contacto(string nombre, string correo, string telefono, string asunto, string mensaje)
        {
            // ═══ Validaciones ═══
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(correo) ||
                string.IsNullOrWhiteSpace(asunto) || string.IsNullOrWhiteSpace(mensaje))
            {
                ViewBag.Error = "Todos los campos obligatorios deben estar completos.";
                return View();
            }

            if (nombre.Length > 100 || correo.Length > 150 || mensaje.Length > 1000)
            {
                ViewBag.Error = "Uno de los campos excede el largo permitido.";
                return View();
            }

            try
            {
                // ═══ Enviar correo al equipo de CNFL ═══
                string cuerpo =
                    "<h2>Nuevo mensaje de contacto</h2>" +
                    "<p><strong>Nombre:</strong> " + HttpUtility.HtmlEncode(nombre) + "</p>" +
                    "<p><strong>Correo:</strong> " + HttpUtility.HtmlEncode(correo) + "</p>" +
                    "<p><strong>Teléfono:</strong> " + HttpUtility.HtmlEncode(telefono ?? "No indicado") + "</p>" +
                    "<p><strong>Asunto:</strong> " + HttpUtility.HtmlEncode(asunto) + "</p>" +
                    "<hr />" +
                    "<p><strong>Mensaje:</strong></p>" +
                    "<p>" + HttpUtility.HtmlEncode(mensaje).Replace("\n", "<br />") + "</p>";

                EmailService.Enviar(
                    "800energia@cnfl.go.cr",
                    "Contacto CNFL · " + asunto,
                    "Nuevo mensaje de contacto",
                    cuerpo,
                    nombre
                );

                TempData["Mensaje"] = "Tu mensaje fue enviado correctamente. Te responderemos pronto.";
                return RedirectToAction("Contacto");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[HomeController.Contacto] Error: " + ex.Message);
                ViewBag.Error = "No se pudo enviar el mensaje. Intentá de nuevo más tarde.";
                return View();
            }
        }
    }
}