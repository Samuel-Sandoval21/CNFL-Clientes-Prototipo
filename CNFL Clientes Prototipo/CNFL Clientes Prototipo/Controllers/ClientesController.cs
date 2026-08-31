using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Services;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class ClientesController : Controller
    {
        private readonly AveriaService _averiaService = new AveriaService();
        private readonly FacturaService _facturaService = new FacturaService();
        private readonly TramiteService _tramiteService = new TramiteService();

        public ActionResult Inicio()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            if (Session["Rol"].ToString() == "Admin")
                return RedirectToAction("Dashboard", "Admin");

            return View();
        }

        public ActionResult Tramites()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        public ActionResult Pagos()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            var facturas = _facturaService.ListarFacturas();
            return View(facturas);
        }

        [HttpPost]
        public JsonResult PagarFactura(int id, string metodo)
        {
            if (Session["Rol"] == null)
            {
                return Json(new { success = false, message = "No autorizado" });
            }

            try
            {
                _facturaService.PagarFactura(id);
                return Json(new { success = true, message = "✅ Pago realizado con éxito" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "❌ Error al procesar el pago: " + ex.Message });
            }
        }

        public ActionResult Tienda()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        public ActionResult Reportes()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        public ActionResult Perfil()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        public ActionResult Chat()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        public ActionResult DetalleFactura(int id = 1)
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            ViewBag.FacturaId = id;
            return View();
        }

        // ===== MÉTODOS DE PAGO =====
        public ActionResult Sinpe(string monto = "0", string facturaId = "0")
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            ViewBag.Monto = monto;
            ViewBag.FacturaId = facturaId;
            return View();
        }

        public ActionResult Iban(string monto = "0", string facturaId = "0")
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            ViewBag.Monto = monto;
            ViewBag.FacturaId = facturaId;
            return View();
        }

        public ActionResult Tarjeta(string monto = "0", string facturaId = "0")
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            ViewBag.Monto = monto;
            ViewBag.FacturaId = facturaId;
            return View();
        }

        public ActionResult EditarDatos()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        public ActionResult Suscripciones()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        public ActionResult ServiciosContratados()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        public ActionResult HistorialCompras()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        public ActionResult Calculadora()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        public ActionResult Notificaciones()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        // ===== REPORTES DE AVERÍAS =====
        public ActionResult ReportarAlumbrado()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        public ActionResult ReportarPropia()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        public ActionResult ReportarAjena()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        [HttpPost]
        public JsonResult RegistrarAveria(Averia averia)
        {
            if (Session["Rol"] == null)
            {
                return Json(new { success = false, message = "No autorizado" });
            }

            try
            {
                // Asignar NISE del usuario si no viene
                if (string.IsNullOrEmpty(averia.NISE))
                {
                    averia.NISE = Session["NISE"]?.ToString() ?? "000000000";
                }

                _averiaService.RegistrarAveria(averia);
                return Json(new { success = true, id = averia.Id, message = "✅ Reporte enviado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "❌ Error al registrar la avería: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult RegistrarTramite([System.Web.Mvc.ModelBinder(typeof(JsonModelBinder))] FormularioTramite datos)
        {
            if (Session["Rol"] == null)
            {
                return Json(new { success = false, message = "No autorizado" });
            }

            try
            {
                _tramiteService.CrearTramite(datos.NombreTramite, datos);
                var tramites = _tramiteService.ObtenerTodos();
                var ultimo = tramites.LastOrDefault();
                return Json(new { success = true, id = ultimo?.Id ?? 0, message = "✅ Solicitud enviada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "❌ Error al registrar el trámite: " + ex.Message });
            }
        }
    }

    public class JsonModelBinder : System.Web.Mvc.IModelBinder
    {
        public object BindModel(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
        {
            try
            {
                var request = controllerContext.HttpContext.Request;
                request.InputStream.Position = 0;
                using (var reader = new System.IO.StreamReader(request.InputStream))
                {
                    var jsonString = reader.ReadToEnd();
                    if (string.IsNullOrEmpty(jsonString))
                        return null;

                    var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    return serializer.Deserialize(jsonString, bindingContext.ModelType);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}