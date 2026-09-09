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

        // POST: Clientes/PagarFactura
        [HttpPost]
        public JsonResult PagarFactura(int facturaId)
        {
            var factura = _db.Facturas.Find(facturaId);
            if (factura != null && !factura.Pagada)
            {
                factura.Pagada = true;
                _db.SaveChanges();

                // Crear notificación
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

        // GET: Clientes/ReportarAveria
        public ActionResult ReportarAveria()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            var nises = _db.NISEs.Where(n => n.UsuarioId == usuarioId).ToList();
            ViewBag.NISEs = new SelectList(nises, "NiseId", "NumeroNise");
            return View();
        }

        // POST: Clientes/ReportarAveria
        [HttpPost]
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

                // Crear notificación
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

        // GET: Clientes/EstadoAveria
        public ActionResult EstadoAveria(int id)
        {
            var averia = _db.Averias
                .Include("NISE")
                .FirstOrDefault(a => a.AveriaId == id);

            if (averia == null)
                return HttpNotFound();

            return View(averia);
        }

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

            // Marcar como leídas
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
        public ActionResult ReportarAlumbrado(string Tipo, string Descripcion, string Direccion)
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            // Guardado sencillo: en esta versión de prototipo sólo confirmamos recepción
            ViewBag.Mensaje = "Reporte de alumbrado enviado correctamente. Gracias por su colaboración.";
            return View();
        }

        // GET: Clientes/Dashboard
        public ActionResult Dashboard()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return RedirectToAction("Login", "Cuenta");

            // La vista Dashboard puede consumir servicios desde cliente/Script; por ahora devolvemos la vista
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _db.Dispose();
            base.Dispose(disposing);
        }
    }
}