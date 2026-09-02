using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.IO;
using System.Web.Script.Serialization;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Data;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class ClientesController : Controller
    {
        private readonly CNFLDbContext _db = new CNFLDbContext();

        // GET: /Clientes/Inicio
        public ActionResult Inicio()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            if (Session["Rol"].ToString() == "Admin")
                return RedirectToAction("Dashboard", "Admin");

            var usuarioId = (int)Session["Id"];
            var usuario = _db.Usuarios.Find(usuarioId);

            if (usuario == null)
            {
                Session.Clear();
                return RedirectToAction("Login", "Cuenta");
            }

            ViewBag.Nombre = usuario.Nombre + " " + usuario.Apellidos;
            ViewBag.NISE = usuario.NISE;
            ViewBag.Cedula = usuario.Cedula;
            ViewBag.Correo = usuario.Correo;
            ViewBag.Telefono = usuario.Telefono;

            var facturasPendientes = _db.Facturas
                .Where(f => f.NISE.Cliente.UsuarioId == usuarioId && f.Estado == "Pendiente")
                .ToList();

            ViewBag.FacturasPendientes = facturasPendientes;
            ViewBag.TotalPendiente = facturasPendientes.Sum(f => f.Saldo);

            return View();
        }

        // GET: /Clientes/Tramites
        public ActionResult Tramites()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // GET: /Clientes/Pagos
        public ActionResult Pagos()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            var usuarioId = (int)Session["Id"];
            var facturas = _db.Facturas
                .Where(f => f.NISE.Cliente.UsuarioId == usuarioId)
                .OrderByDescending(f => f.FechaVencimiento)
                .ToList();

            return View(facturas);
        }

        // GET: /Clientes/Tienda
        public ActionResult Tienda()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // GET: /Clientes/Reportes
        public ActionResult Reportes()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            var usuarioId = (int)Session["Id"];

            var averias = _db.Averias
                .Where(a => a.UsuarioId == usuarioId)
                .OrderByDescending(a => a.FechaReporte)
                .ToList();

            var fechaLimite = DateTime.Now.AddHours(-24);
            var averiasFiltradas = averias
                .Where(a => a.Estado != "Resuelto" || (a.Estado == "Resuelto" && a.FechaReporte >= fechaLimite))
                .ToList();

            return View(averiasFiltradas);
        }

        // GET: /Clientes/ObtenerAverias (AJAX - BD REAL)
        [HttpGet]
        public JsonResult ObtenerAverias()
        {
            if (Session["Rol"] == null)
            {
                return Json(new { success = false, message = "No autorizado" }, JsonRequestBehavior.AllowGet);
            }

            var usuarioId = (int)Session["Id"];
            var fechaLimite = DateTime.Now.AddHours(-24);

            var averias = _db.Averias
                .Where(a => a.UsuarioId == usuarioId)
                .ToList()
                .Where(a => a.Estado != "Resuelto" || (a.Estado == "Resuelto" && a.FechaReporte >= fechaLimite))
                .Select(a => new
                {
                    a.Id,
                    a.TipoAveria,
                    a.Descripcion,
                    a.Direccion,
                    a.Latitud,
                    a.Longitud,
                    a.Estado,
                    FechaReporte = a.FechaReporte.ToString("dd/MM/yyyy HH:mm"),
                    TiempoTranscurrido = CalcularTiempoTranscurrido(a.FechaReporte)
                })
                .ToList();

            return Json(averias, JsonRequestBehavior.AllowGet);
        }

        // POST: /Clientes/RegistrarAveria (BD REAL) - CORREGIDO SIN [FromBody]
        [HttpPost]
        public JsonResult RegistrarAveria()
        {
            if (Session["Rol"] == null)
            {
                return Json(new { success = false, message = "No autorizado" });
            }

            // Leer el cuerpo de la solicitud manualmente
            var jsonString = "";
            using (var reader = new StreamReader(Request.InputStream))
            {
                jsonString = reader.ReadToEnd();
            }

            // Deserializar JSON a objeto AveriaRequest
            var serializer = new JavaScriptSerializer();
            var request = serializer.Deserialize<AveriaRequest>(jsonString);

            if (request == null)
            {
                return Json(new { success = false, message = "Datos inválidos" });
            }

            if (string.IsNullOrEmpty(request.Titulo) || string.IsNullOrEmpty(request.Direccion))
            {
                return Json(new { success = false, message = "Título y dirección son requeridos" });
            }

            var usuarioId = (int)Session["Id"];
            var usuario = _db.Usuarios.Find(usuarioId);

            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            // Obtener el NISE
            var nise = _db.NISEs.FirstOrDefault(n => n.Cliente.UsuarioId == usuarioId && n.Numero == request.NISE);
            if (nise == null)
            {
                return Json(new { success = false, message = "NISE no encontrado" });
            }

            var nuevaAveria = new Averia
            {
                UsuarioId = usuarioId,
                NISEId = nise.Id,
                TipoAveria = request.Tipo ?? "Eléctrica",
                Descripcion = request.Descripcion ?? request.Titulo,
                Direccion = request.Direccion,
                Latitud = string.IsNullOrEmpty(request.Latitud) ? (decimal?)null : decimal.Parse(request.Latitud),
                Longitud = string.IsNullOrEmpty(request.Longitud) ? (decimal?)null : decimal.Parse(request.Longitud),
                Estado = "Reportado",
                FechaReporte = DateTime.Now
            };

            _db.Averias.Add(nuevaAveria);
            _db.SaveChanges();

            // Crear notificación
            var notificacion = new Notificacion
            {
                UsuarioId = usuarioId,
                Titulo = "Avería reportada",
                Mensaje = $"Su avería '{request.Titulo}' ha sido reportada exitosamente. Número de seguimiento: #REP-{nuevaAveria.Id}",
                Tipo = "Avería",
                Leida = false,
                FechaEnvio = DateTime.Now
            };

            _db.Notificaciones.Add(notificacion);
            _db.SaveChanges();

            return Json(new { success = true, message = "Avería reportada exitosamente", id = nuevaAveria.Id });
        }

        // Otra alternativa: usar un modelo en el parámetro SIN [FromBody] (MVC lo enlaza automáticamente desde FormData)
        // Si usas FormData en el frontend, puedes usar este método:
        [HttpPost]
        public JsonResult RegistrarAveriaForm(AveriaRequest request)
        {
            if (Session["Rol"] == null)
            {
                return Json(new { success = false, message = "No autorizado" });
            }

            if (string.IsNullOrEmpty(request.Titulo) || string.IsNullOrEmpty(request.Direccion))
            {
                return Json(new { success = false, message = "Título y dirección son requeridos" });
            }

            var usuarioId = (int)Session["Id"];
            var usuario = _db.Usuarios.Find(usuarioId);

            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            var nise = _db.NISEs.FirstOrDefault(n => n.Cliente.UsuarioId == usuarioId && n.Numero == request.NISE);
            if (nise == null)
            {
                return Json(new { success = false, message = "NISE no encontrado" });
            }

            var nuevaAveria = new Averia
            {
                UsuarioId = usuarioId,
                NISEId = nise.Id,
                TipoAveria = request.Tipo ?? "Eléctrica",
                Descripcion = request.Descripcion ?? request.Titulo,
                Direccion = request.Direccion,
                Latitud = string.IsNullOrEmpty(request.Latitud) ? (decimal?)null : decimal.Parse(request.Latitud),
                Longitud = string.IsNullOrEmpty(request.Longitud) ? (decimal?)null : decimal.Parse(request.Longitud),
                Estado = "Reportado",
                FechaReporte = DateTime.Now
            };

            _db.Averias.Add(nuevaAveria);
            _db.SaveChanges();

            var notificacion = new Notificacion
            {
                UsuarioId = usuarioId,
                Titulo = "Avería reportada",
                Mensaje = $"Su avería '{request.Titulo}' ha sido reportada exitosamente. Número de seguimiento: #REP-{nuevaAveria.Id}",
                Tipo = "Avería",
                Leida = false,
                FechaEnvio = DateTime.Now
            };

            _db.Notificaciones.Add(notificacion);
            _db.SaveChanges();

            return Json(new { success = true, message = "Avería reportada exitosamente", id = nuevaAveria.Id });
        }

        private string CalcularTiempoTranscurrido(DateTime fecha)
        {
            var diff = DateTime.Now - fecha;
            if (diff.TotalHours < 1) return $"{diff.Minutes} min";
            if (diff.TotalHours < 24) return $"{diff.Hours} h";
            if (diff.TotalDays < 7) return $"{diff.Days} días";
            if (diff.TotalDays < 30) return $"{diff.Days / 7} sem";
            return $"{diff.Days / 30} meses";
        }

        // GET: /Clientes/Perfil
        public ActionResult Perfil()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // GET: /Clientes/Chat
        public ActionResult Chat()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // GET: /Clientes/DetalleFactura
        public ActionResult DetalleFactura(int id = 1)
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            var factura = _db.Facturas.Find(id);
            if (factura == null)
                return HttpNotFound();

            ViewBag.FacturaId = id;
            return View(factura);
        }

        // GET: /Clientes/Sinpe
        public ActionResult Sinpe(string monto = "0")
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            ViewBag.Monto = monto;
            return View();
        }

        // GET: /Clientes/Iban
        public ActionResult Iban(string monto = "0")
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            ViewBag.Monto = monto;
            return View();
        }

        // GET: /Clientes/Tarjeta
        public ActionResult Tarjeta(string monto = "0")
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            ViewBag.Monto = monto;
            return View();
        }

        // GET: /Clientes/Carrito
        public ActionResult Carrito()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // GET: /Clientes/EditarDatos
        public ActionResult EditarDatos()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // GET: /Clientes/Suscripciones
        public ActionResult Suscripciones()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // GET: /Clientes/ServiciosContratados
        public ActionResult ServiciosContratados()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // GET: /Clientes/HistorialCompras
        public ActionResult HistorialCompras()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // GET: /Clientes/Calculadora
        public ActionResult Calculadora()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // GET: /Clientes/Notificaciones
        public ActionResult Notificaciones()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");

            var usuarioId = (int)Session["Id"];
            var notificaciones = _db.Notificaciones
                .Where(n => n.UsuarioId == usuarioId)
                .OrderByDescending(n => n.FechaEnvio)
                .ToList();

            return View(notificaciones);
        }

        // POST: /Clientes/MarcarNotificacionLeida
        [HttpPost]
        public JsonResult MarcarNotificacionLeida(int id)
        {
            if (Session["Rol"] == null)
            {
                return Json(new { success = false, message = "No autorizado" });
            }

            var notificacion = _db.Notificaciones.Find(id);
            if (notificacion != null)
            {
                notificacion.Leida = true;
                _db.SaveChanges();
                return Json(new { success = true, message = "Notificación marcada como leída" });
            }

            return Json(new { success = false, message = "Notificación no encontrada" });
        }

        // POST: /Clientes/MarcarTodasLeidas
        [HttpPost]
        public JsonResult MarcarTodasLeidas()
        {
            if (Session["Rol"] == null)
            {
                return Json(new { success = false, message = "No autorizado" });
            }

            var usuarioId = (int)Session["Id"];
            var notificaciones = _db.Notificaciones.Where(n => n.UsuarioId == usuarioId && n.Leida == false);

            foreach (var n in notificaciones)
            {
                n.Leida = true;
            }

            _db.SaveChanges();
            return Json(new { success = true, message = "Todas las notificaciones marcadas como leídas" });
        }

        // GET: /Clientes/ReportarAlumbrado
        public ActionResult ReportarAlumbrado()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // GET: /Clientes/ReportarPropia
        public ActionResult ReportarPropia()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        // GET: /Clientes/ReportarAjena
        public ActionResult ReportarAjena()
        {
            if (Session["Rol"] == null)
                return RedirectToAction("Login", "Cuenta");
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _db.Dispose();
            base.Dispose(disposing);
        }
    }

    // ==========================================================
    // MODELOS DE VISTA PARA AVERÍAS
    // ==========================================================

    public class AveriaRequest
    {
        public string Titulo { get; set; }
        public string Direccion { get; set; }
        public string Descripcion { get; set; }
        public string Tipo { get; set; }
        public string Estado { get; set; }
        public string NISE { get; set; }
        public string FotoUrl { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
    }
}