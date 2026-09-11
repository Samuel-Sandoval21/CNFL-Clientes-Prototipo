using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CNFL_Clientes_Prototipo.Models;
using CNFL_Clientes_Prototipo.Data;

namespace CNFL_Clientes_Prototipo.Controllers
{
    public class ClientesController : Controller
    {
        private CNFLDbContext _db = new CNFLDbContext();

        // ============================================================
        // PERFIL Y DATOS DEL USUARIO
        // ============================================================

        // GET: Clientes/MiPerfil
        public ActionResult MiPerfil()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            var usuario = _db.Usuarios
                .Include("NISEs")
                .Include("Averias")
                .Include("Notificaciones")
                .Include("Suscripciones")
                .FirstOrDefault(u => u.UsuarioId == usuarioId);

            if (usuario == null)
                return HttpNotFound();

            return View(usuario);
        }

        // GET: Clientes/EditarPerfil -> redirige al formulario de editar datos
        public ActionResult EditarPerfil()
        {
            return RedirectToAction("EditarDatos");
        }

        // GET: Clientes/EditarDatos
        public ActionResult EditarDatos()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            var usuario = _db.Usuarios
                .Include("NISEs")
                .Include("Averias")
                .Include("Notificaciones")
                .Include("Suscripciones")
                .FirstOrDefault(u => u.UsuarioId == usuarioId);

            if (usuario == null)
                return HttpNotFound();

            return View(usuario);
        }

        // POST: Clientes/EditarDatos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarDatos(Usuario model)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = _db.Usuarios.Find(usuarioId);
            if (usuario == null)
                return HttpNotFound();

            usuario.Nombre = model.Nombre;
            usuario.Apellidos = model.Apellidos;
            usuario.Correo = model.Correo;
            usuario.Telefono = model.Telefono;

            _db.SaveChanges();

            return RedirectToAction("MiPerfil");
        }

        // ============================================================
        // FACTURAS Y PAGOS
        // ============================================================

        // GET: Clientes/MisFacturas
        public ActionResult MisFacturas()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            var usuario = _db.Usuarios.Find(usuarioId);
            if (usuario == null)
                return HttpNotFound();

            var nisesIds = _db.NISEs.Where(n => n.UsuarioId == usuarioId).Select(n => n.NiseId).ToList();
            var facturas = _db.Facturas.Where(f => nisesIds.Contains(f.NiseId)).OrderByDescending(f => f.FechaEmision).ToList();

            return View(facturas);
        }

        // GET: Clientes/Pagos
        public ActionResult Pagos()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            var nisesIds = _db.NISEs.Where(n => n.UsuarioId == usuarioId).Select(n => n.NiseId).ToList();
            var facturas = _db.Facturas.Where(f => nisesIds.Contains(f.NiseId)).OrderByDescending(f => f.FechaEmision).ToList();

            return View(facturas);
        }

        // POST: Clientes/PagarFactura
        [HttpPost]
        public JsonResult PagarFactura(int facturaId)
        {
            var factura = _db.Facturas.Find(facturaId);
            if (factura != null && !factura.Pagada)
            {
                factura.Pagada = true;
                _db.SaveChanges();

                var notificacion = new Notificacion
                {
                    UsuarioId = _db.NISEs.Find(factura.NiseId).UsuarioId,
                    Titulo = "Pago realizado",
                    Mensaje = $"Se ha realizado el pago de la factura {factura.NumeroFactura} por ₡{factura.Monto:N0}.",
                    Fecha = DateTime.Now,
                    Leida = false,
                    Tipo = "Factura"
                };
                _db.Notificaciones.Add(notificacion);
                _db.SaveChanges();

                return Json(new { success = true, message = "Pago realizado exitosamente." });
            }
            return Json(new { success = false, message = "La factura no existe o ya fue pagada." });
        }

        // ============================================================
        // NOTIFICACIONES Y SUSCRIPCIONES
        // ============================================================

        // GET: Clientes/MisNotificaciones
        public ActionResult MisNotificaciones()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            var notificaciones = _db.Notificaciones
                .Where(n => n.UsuarioId == usuarioId)
                .OrderByDescending(n => n.Fecha)
                .ToList();

            foreach (var not in notificaciones.Where(n => !n.Leida))
            {
                not.Leida = true;
            }
            _db.SaveChanges();

            return View(notificaciones);
        }

        // GET: Clientes/MisSuscripciones
        public ActionResult MisSuscripciones()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            var suscripciones = _db.Suscripciones
                .Where(s => s.UsuarioId == usuarioId)
                .ToList();

            return View(suscripciones);
        }

        // POST: Clientes/SuscribirServicio
        [HttpPost]
        public JsonResult SuscribirServicio(string servicio, decimal? montoMensual)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return Json(new { success = false, message = "Debe iniciar sesión." });

            var suscripcion = new Suscripcion
            {
                UsuarioId = usuarioId.Value,
                Servicio = servicio,
                FechaInicio = DateTime.Now,
                Activa = true,
                MontoMensual = montoMensual
            };

            _db.Suscripciones.Add(suscripcion);
            _db.SaveChanges();

            return Json(new { success = true, message = "Suscripción activada correctamente." });
        }

        // ============================================================
        // DASHBOARD, REPORTES Y TIENDA
        // ============================================================

        // GET: Clientes/Dashboard
        public ActionResult Dashboard()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        // GET: Clientes/Reportes
        public ActionResult Reportes()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        // GET: Clientes/Tienda
        public ActionResult Tienda()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        // GET: Clientes/DetalleProducto?id=tienda
        public ActionResult DetalleProducto(string id)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction("Tienda");

            ViewBag.CategoriaId = id;

            var titulos = new Dictionary<string, string>
            {
                { "tienda", "Tienda CNFL" },
                { "supresores", "Supresores y Bases" },
                { "cargadores", "Cargadores Semirápidos" },
                { "bienes", "Bienes Inmuebles CNFL" },
                { "asiste", "CNFL Te Asiste" },
                { "internet", "Internet Fijo 5G" },
                { "seguro-hogar", "Seguro de Hogar" },
                { "sri", "Ingeniería Eléctrica (SIE)" },
                { "ambientales", "Servicios Ambientales" },
                { "calibracion", "Calibración" },
                { "anonos", "Taller Anonos" },
                { "reparacion", "Reparación y Mantenimiento" }
            };

            var subtitulos = new Dictionary<string, string>
            {
                { "tienda", "Comprá productos del hogar" },
                { "supresores", "Protección en cada partido" },
                { "cargadores", "Energía lista para cada jugada" },
                { "bienes", "Locales y propiedades" },
                { "asiste", "Asistencias y seguros" },
                { "internet", "Viví la velocidad" },
                { "seguro-hogar", "Contra incendio y rayo" },
                { "sri", "Soluciones profesionales" },
                { "ambientales", "Sostenibilidad y control" },
                { "calibracion", "Equipos certificados" },
                { "anonos", "Reparación especializada" },
                { "reparacion", "Servicio técnico" }
            };

            ViewBag.Titulo = titulos.ContainsKey(id) ? titulos[id] : "Producto";
            ViewBag.Subtitulo = subtitulos.ContainsKey(id) ? subtitulos[id] : "";

            return View();
        }

        // ============================================================
        // REPORTES DE AVERÍAS — FLUJO COMPLETO
        // ============================================================

        // GET: Clientes/TiposReportes
        // Vista intermedia que muestra los tipos de avería disponibles
        public ActionResult TiposReportes()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        // GET: Clientes/TiposReportesDetalle?tipo=Poste caído
        // Vista de confirmación con formulario prellenado
        public ActionResult TiposReportesDetalle(string tipo)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            ViewBag.Tipo = string.IsNullOrWhiteSpace(tipo) ? "Avería" : tipo;
            return View();
        }

        // GET: Clientes/ReportarAveria
        // Acepta ?tipo=X para prellenar el tipo de avería
        public ActionResult ReportarAveria(string tipo)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            var nises = _db.NISEs.Where(n => n.UsuarioId == usuarioId).ToList();
            ViewBag.NISEs = new SelectList(nises, "NiseId", "NumeroNise");
            ViewBag.Tipo = tipo ?? "";
            return View();
        }

        // POST: Clientes/ReportarAveria
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportarAveria(Averia averia)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            if (ModelState.IsValid)
            {
                averia.UsuarioId = usuarioId.Value;
                averia.FechaReporte = DateTime.Now;
                averia.Estado = "Ingresado";
                _db.Averias.Add(averia);
                _db.SaveChanges();

                var notificacion = new Notificacion
                {
                    UsuarioId = usuarioId.Value,
                    Titulo = "Avería reportada",
                    Mensaje = $"Su reporte de avería #{averia.AveriaId} ha sido ingresado correctamente.",
                    Fecha = DateTime.Now,
                    Leida = false,
                    Tipo = "Averia"
                };
                _db.Notificaciones.Add(notificacion);
                _db.SaveChanges();

                ViewBag.Mensaje = "Avería reportada exitosamente. Número de seguimiento: #" + averia.AveriaId;
            }

            var nises = _db.NISEs.Where(n => n.UsuarioId == usuarioId).ToList();
            ViewBag.NISEs = new SelectList(nises, "NiseId", "NumeroNise");
            return View(averia);
        }

        // GET: Clientes/EstadoAveria/5
        public ActionResult EstadoAveria(int id)
        {
            var averia = _db.Averias
                .Include("NISE")
                .FirstOrDefault(a => a.AveriaId == id);

            if (averia == null)
                return HttpNotFound();

            return View(averia);
        }

        // GET: Clientes/ConsultarAveria
        public ActionResult ConsultarAveria()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        // GET: Clientes/GenerarComprobante
        public ActionResult GenerarComprobante()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        // GET: Clientes/MapaAverias
        public ActionResult MapaAverias()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        // GET: Clientes/HistorialReportes
        public ActionResult HistorialReportes()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            var averias = _db.Averias
                .Include("NISE")
                .Where(a => a.UsuarioId == usuarioId)
                .OrderByDescending(a => a.FechaReporte)
                .ToList();

            return View(averias);
        }

        // ============================================================
        // ALUMBRADO PÚBLICO
        // ============================================================

        // GET: Clientes/ReportarAlumbrado
        public ActionResult ReportarAlumbrado()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            return View();
        }

        // POST: Clientes/ReportarAlumbrado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportarAlumbrado(string NISE, string TipoProblema, string Direccion, string Descripcion)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            // Validación mínima
            if (string.IsNullOrWhiteSpace(Direccion) || string.IsNullOrWhiteSpace(TipoProblema))
            {
                ViewBag.Error = "Debe completar la dirección y el tipo de problema.";
                return View();
            }

            // Registrar como avería tipo "Iluminación pública"
            var averia = new Averia
            {
                UsuarioId = usuarioId.Value,
                Tipo = "Iluminación pública",
                Descripcion = $"Tipo: {TipoProblema}. Dirección: {Direccion}. Detalle: {Descripcion}",
                FechaReporte = DateTime.Now,
                Estado = "Ingresado"
            };

            _db.Averias.Add(averia);
            _db.SaveChanges();

            var notificacion = new Notificacion
            {
                UsuarioId = usuarioId.Value,
                Titulo = "Reporte de alumbrado enviado",
                Mensaje = $"Su reporte de alumbrado #{averia.AveriaId} fue registrado. Gracias por su colaboración.",
                Fecha = DateTime.Now,
                Leida = false,
                Tipo = "Averia"
            };
            _db.Notificaciones.Add(notificacion);
            _db.SaveChanges();

            ViewBag.Mensaje = "Reporte de alumbrado enviado correctamente. Número de seguimiento: #" + averia.AveriaId;
            return View();
        }

        // ============================================================
        // DISPOSE
        // ============================================================
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _db.Dispose();
            base.Dispose(disposing);
        }
    }
}